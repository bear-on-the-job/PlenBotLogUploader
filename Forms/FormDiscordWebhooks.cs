using Newtonsoft.Json;
using PlenBotLogUploader.AppSettings;
using PlenBotLogUploader.DiscordApi;
using PlenBotLogUploader.DiscordApi.Awards;
using PlenBotLogUploader.DpsReport;
using PlenBotLogUploader.Properties;
using PlenBotLogUploader.DpsReport.ExtraJson;
using PlenBotLogUploader.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextTableFormatter;
using static System.Net.WebRequestMethods;
using System.Drawing.Text;
using Hardstuck.GuildWars2.BuildCodes.V2;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PlenBotLogUploader;

public class DiscordException : Exception
{
    public DiscordException(string message) : base(message) { }
}
public class DiscordSizeException : DiscordException
{
    public DiscordSizeException(string message) : base(message) { }
}

public partial class FormDiscordWebhooks : Form
{
    private const string NO_EMOJI = "⭕";
    private const string TEAM_FORMAT_PREFIX = "java";
    private const string STRETCH_FIELD = "";

    private const int CHILLED_ID = 722;
    private const int POISON_ID = 723;
    private const int FEARED_ID = 791;
    private const int STABILITY_ID = 1122;

    private static readonly DiscordApiJsonContentEmbedField FIELD_SPACER = new()
    {
        Name = "",
        Value = "",
        Inline = false
    };

    private List<Award> awards = new List<Award>();

    /// <summary>
    /// Parses CSV content into award display list
    /// </summary>
    private static List<Award> ParseCsvToAwardProcessors(string csvContent, IEnumerable<Skill> skills, IEnumerable<Buff> buffs)
    {
        List<Award> awardDisplays = new List<Award>();

        // Split the CSV content into lines
        var lines = csvContent.Split("\r\n");

        // Iterate over each line (skipping the header)
        for (int i = 1; i < lines.Length; i++)
        {
            // Skip empty lines
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            // Split the line into columns
            var columns = lines[i].Split(',');
            if (columns.Length < 8) continue;

            // Create a new award and add it to the list
            awardDisplays.Add(new Award
            {
                Skills = skills,
                Buffs = buffs,

                Description = columns[0],
                Name = columns[1],
                Emoji = columns[2],
                Rarity = columns[3],
                Active = columns[4],

                Category = columns[5],
                Property = columns[6],
                Qualifier = float.TryParse(columns[7], out var value) ? value : 0f,

                AbilityNames = columns.Length > 8 ? columns[8..] : null
            });
        }

        return awardDisplays;
    }

    /// <summary>
    /// Loads configured award display settings from google sheets
    /// </summary>
    private void LoadAwardDisplays(string googleSheetsUrl, IEnumerable<Skill> skills, IEnumerable<Buff> buffs)
    {
        if (string.IsNullOrEmpty(googleSheetsUrl)) return;

        var pattern = @"\/spreadsheets\/d\/([0-9a-zA-Z_-]+)\/";
        var sheetId = Regex.Match(googleSheetsUrl, pattern);
        if (sheetId is null || !sheetId.Success || sheetId.Groups.Count < 2) return;

        using var client = new HttpClient();
        var csvUrl = $"https://docs.google.com/spreadsheets/d/{sheetId.Groups[1].Value}/export?format=csv";
        var csvContent = client.GetStringAsync(csvUrl).Result;
        if (csvContent is null) return;

        awards = ParseCsvToAwardProcessors(csvContent, skills, buffs);
    }

    /// <summary>
    /// Assign awards to players, based on their qualifications and ranking. Will pick top 
    /// 3 awards randomly.
    /// </summary>
    private IEnumerable<DiscordApiJsonContentEmbedField> AssignAwards(IEnumerable<Player> players, IEnumerable<Target> targets, string googleSheetsUrl, IEnumerable<Skill> skills, IEnumerable<Buff> buffs)
    {
        LoadAwardDisplays(googleSheetsUrl, skills, buffs);

        // Only process active awards
        var awards = this.awards.Where(ad => ad.Active.Equals("yes", StringComparison.InvariantCultureIgnoreCase) || ad.Active.Equals("true", StringComparison.InvariantCultureIgnoreCase));

        // Run all awards to see which has most qualifiers
        var qualifiers = awards
            .Select
            (
                award => new
                {
                    Award = award,
                    Players = players.Where(p => award.Qualify(p, targets)).OrderByDescending(p => award.Rank(p, targets))
                }
            )
            .Where(q => q.Players.Any())
            .ToList();

        var shuffled = Shuffle(qualifiers).ToList();
        var legendary = shuffled.Where(s => s.Award.Rarity == "Legendary");
        var epic = shuffled.Where(s => s.Award.Rarity == "Epic");
        var rare = shuffled.Where(s => s.Award.Rarity == "Rare");
        var common = shuffled.Where(s => s.Award.Rarity == "Common");

        // Default the winners to those with legendary awards
        var winners = legendary.ToList();
        winners.AddRange(epic);
        winners.AddRange(rare);
        winners.AddRange(common);

        // Take top 3 after combining
        winners = winners.Take(3).ToList();
        var fields = winners
            .Select
            (
                winner => CreateAwardField(winner.Award.Name, winner.Award.Description, winner.Award.Emoji, winner.Award.Rarity, winner.Players.FirstOrDefault())
            )
            .ToList();

        fields.Insert(0, FIELD_SPACER);
        return fields;
    }

    /// <summary>
    /// Private method to create the formatted rank fields for each category.
    /// </summary>
    DiscordApiJsonContentEmbedField CreateAwardField(string name, string description, string emoji, string rarity, Player player)
    {
        var format = rarity switch
        {
            "Common" => "",
            "Rare" => "fix\n",
            "Epic" => "prolog\n",
            "Legendary" => "ml\n",
            _ => ""
        };

        var textInfo = new CultureInfo("en-US", false).TextInfo;
        var field = new DiscordApiJsonContentEmbedField();

        field.Name = $"{emoji}   {name} - {player.Name}";
        field.Value = $"```{format}{textInfo.ToTitleCase(description)}```";
        field.Inline = false;

        return field;
    }

    private List<(List<string> category, List<(int id, double coefficient)> skills)> mapping = new List<(List<string>, List<(int, double)>)>
        {
            (
                new List<string> { "Relics" },
                new List<(int, double)>
                {
                    (70491, 1), //Relic of the wizard's tower
                }
            ),
            (
                new List<string> { "Tempest", "Weaver", "Catalyst", "Elementalist"},
                new List<(int, double)>
                {
                    (51662, 4/2.5), //Transmute Lightning & Shocking Aura
                    (5671,  1), //Static Field
                    (5732,  1), //Static Field (Lightning Hammer)
                    (40794, 1), //Earthen Synergy
                    (62716, 1), //Shock Blast
                    (5687,  1), //Updraft
                    (5534,  1), //Tornado
                    (35304, 1), //Dust Charge
                    (5733,  1), //Wind Blast
                    (5690,  1), //Earthquake
                    (5562,  1), //Gale
                    (46018, 1), //Mud Slide
                    (62947, 1), //Wind Storm
                    (30864, 1), //Tidal Surge
                    (5553,  1), //Gust
                    (5754,  1), //Debris Tornado
                    (30008, 1), //Cyclone
                    (5747,  1), //Magnetic Shield
                    (5490,  1), //Comet
                    (5547,  1), //Magnetic Surge
                    (44998, 1), //Polaric Leap
                    (42321, 1), //Pile Driver
                    (5721,  1), //Deep Freeze
                    (71966,  1), //Dazing Discharge
                    (46140,  1), //Katabatic Wind
                }
            ),
            (
                new List<string> { "Specter","Daredevil","Deadeye","Thief"},
                new List<(int, double)>
                {
                    (13012, 1), //Head Shot
                    (63230, 1), //Well of Silence
                    (30568, 1), //Distracting Daggers
                    (29516, 1), //Impact Strike
                    (1131,  1), //Mace Head Crack                    
                    (63275, 1), //Shadowfall
                    (63220, 1), //Dawn's Repose
                    (1141,  1), //Skull Fear
                    (63249, 1), //Mind Shock
                    (13031, 1), //Pistol Whip
                    (13024, 1/4), //Choking Gas
                    (56880, 1), //Pitfall
                    (30077, 1), //Uppercut
                    (46335, 2), //Shadow Gust
                    (13114, 1), //Tactical Strike
                    (50484, 1), //Malicious Tactical Strike
                }
            ),
            (
                new List<string> { "Spellbreaker","Berserker","Bladesworn","Warrior"},
                new List<(int, double)>
                {
                    (14359, 1), //Staggering Blow
                    (14360, 1), //Rifle Butt
                    (14502, 1), //Kick
                    (14511, 1), //Backbreaker
                    (14415, 1), //Tremor
                    (14516, 1), //Bull's Charge
                    (29941, 1), //Wild Blow
                    (14387, 1), //Earthshaker
                    (14512, 1), //Earthshaker
                    (14513, 1), //Earthshaker
                    (14514, 1), //Earthshaker
                    (40601, 1), //Earthshaker
                    (14361, 1), //Shield Bash
                    (14414, 1), //Skull Crack
                    (14425, 1), //Skull Crack
                    (14426, 1), //Skull Crack
                    (14427, 1), //Skull Crack
                    (30343, 1), //Head Butt
                    (44165, 1), //Full Counter
                    (41243, 1), //Full Counter
                    (44937, 1), //Disrupting Stab
                    (14503, 1), //Pommel Bash
                    (14405, 1), //Banner of Strength
                    (62732, 1), //Artillery Slash
                    (14388, 1), //Stomp
                    (14409, 1), //Fear Me
                    (41919, 1), //Imminent Threat
                    (72026, 1), //Snap Pull
                    (29679, 1), //Skull Grinder
                }
            ),
            (
                new List<string> { "Scrapper","Holosmith","Mechanist","Engineer"},
                new List<(int, double)>
                {
                    (6054,  1), //Static Shield
                    (21661, 1), //Static Shock
                    (6161,  1), //Throw Mine
                    (30337, 1), //Throw Mine
                    (6162,  1), //Detonate
                    (31248, 1), //Blast Gyro
                    (5868,  1), //Supply Crate
                    (63234, 1), //Rocket Fist Prototype
                    (71888, 1), //Essence of Borrowed Time
                    (30713, 1/6), //Thunderclap
                    (5930,  1), //Air Blast
                    (6126,  1), //Magnetic Inversion
                    (5754,  1), //Debris Tornado
                    (6154,  1), //Overcharged Shot
                    (5813,  1), //Big Ol' Bomb
                    (5811,  1), //Personal Battering Ram
                    (29991, 1), //Personal Battering Ram
                    (5889,  1), //Thump
                    (5534,  1), //Tornado
                    (35304, 1), //Dust Charge
                    (42521, 1), //Holographic Shockwave
                    (63345, 1), //Core Reactor Shot
                    (31167, 1/4), //Spare Capacitor
                    (6057,  1), //Throw Shield
                    (63121, 1), //Jade Mortar
                    (5996,  1), //Magnet
                    (41843, 1), //Prismatic Singularity
                    (5982,  1), //Launch Personal Battering Ram
                    (5825,  1), //Slick Shoes
                    (30828, 1), //Slick Shoes
                    (5913,  1), //Explosive Rockets
                    (5893,  1), //Electrified Net
                    (63253, 1), //Force Signet                    
                }
            ),
            (
                new List<string> { "Firebrand","Dragonhunter","Willbender","Guardian" },
                new List<(int, double)>
                {
                    (40624, 1/5), //Symbol of Vengeance - hits / 5
                    (30628, 1/4), //Hunter's Ward - hits / 4
                    (45402, 1), //Blazing Edge
                    (42449, 1), //Chapter 3, Heated Rebuke
                    (9226,  1), //Pull (greatsword 5)
                    (33134, 1), //Hunter's Verdict
                    (41968, 1), //Chapter 2, Daring Challenge
                    (9124,  1), //Banish
                    (29630, 1), //Deflecting Shot
                    (9091,  1), //Shield of Absorption
                    (13688, 1), //Lesser Shield of Absorption
                    (9128,  1), //Sanctuary
                    (9093,  1), //Bane Signet
                    (9125,  1), //Hammer of Wisdom
                    (46170, 1), //Hammer of Wisdom
                    (30871, 1/9), //Light's Judgement
                    (30273, 1), //Dragon's Maw
                    (62549, 1), //Heel Crack
                    (62561, 1), //Heaven's Palm
                    (71817, 1), //Jurisdiction (projectile)
                    (71819, 1), //Jurisdiction (detonation)
                }
            ),
            (
                new List<string> { "Renegade","Vindicator","Herald","Revenant" },
                new List<(int, double)>
                {
                    (41820, 1), //Scorchrazor
                    (28110, 1), //Drop the Hammer
                    (27356, 1), //Energy Expulsion
                    (29114, 1), //Energy Expulsion
                    (28978, 1), //Surge of the Mists
                    (26679, 1), //Forced Engagement
                    (27917, 1), //Call to Anguish
                    (62878, 1), //Reaver's Rage
                    (41220, 1), //Darkrazor's Daring
                    (28406, 1), //Jade Winds
                    (31294, 1), //Jade Winds
                    (28075, 1), //Chaotic Release
                    (71880, 1), //Otherworldly Attaction
                }
            ),
            (
                new List<string> { "Druid","Untamed","Soulbeast","Ranger" },
                new List<(int, double)>
                {
                    (31318, 1), //Lunar Impact
                    (63075, 1), //Overbearing Smash
                    (12598, 1), //Call Lightning
                    (31658, 1), //Glyph of Equality (non-celestial)
                    (45743, 1), //Charge
                    (67179, 1), //Slam - ID seems incorrect?
                    (12476, 1), //Spike Trap
                    (63330, 1), //Thump
                    (42894, 1), //Brutal Charge
                    (46432, 1), //Brutal Charge
                    (42907, 1), //Takedown
                    (12523, 1), //Counterattack Kick
                    (31321, 1), //Wing Buffet
                    (41908, 1), //Wing Buffet
                    (12511, 1), //Point-Blank Shot
                    (30448, 1), //Glyph of the Tides (non-celestial)
                    (12475, 2), //Hilt Bash
                    (12508, 1), //Concussion Shot
                    (12638, 1), //Path of Scars
                    (29558, 1), //Glyph of the Tides (celestial)
                    (12621, 1), //Call of the Wild
                    (71963, 1), //Oaken Cudgel
                    (71002, 1), //Dimension Breach
                    (44360, 1), //Fear
                    (43375, 1), //Prelude Lash
                    (71841, 1), //Wild Strikes
                }
            ),
            (
                new List<string> { "Chronomancer","Mirage","Virtuoso","Mesmer" },
                new List<(int, double)>
                {
                    (10363, 1), //Into the Void
                    (56873, 1), //Time Sink
                    (30643, 1), //Tides of Time
                    (10232, 1), //Signet of Domination
                    (72007, 1), //Phantasmal Sharpshooter
                    (30359, 1), //Gravity Well
                    (10220, 1), //Illusionary Wave
                    (62573, 1), //Psychic Force
                    (10287, 1), //Diversion
                    (45230, 1), //Mirage Thrust
                    (62602, 1), //Bladesong Dissonance
                    (10358, 1), //Counter Blade
                    (10166, 1), //Phantasmal Mage (Backfire)
                    (10169, 1/6), //Chaos Storm 
                    (13733, 1/6), //Lesser Chaos Storm 
                    (10229, 1/2), //Magic Bullet
                    (10341, 1), //Phantasmal Defender
                    (30192, 1), //Lesser Phantasmal Defender
                    (29856, 1/4), //Well of Senility

                }
            ),
            (
                new List<string> { "Reaper","Scourge","Harbinger","Necromancer" },
                new List<(int, double)>
                {
                    (10633, 1), //Ripple of Horror
                    (29709, 1), //Terrify
                    (19115, 1), //Reaper's Mark
                    (10556, 1), //Wail of Doom
                    (10608, 1), //Spectral Ring
                    (44428, 1), //Garish Pillar
                    (30557, 1), //Executioner's Scythe
                    (10620, 1), //Spectral Grasp
                    (10647, 1), //Charge
                    (30105, 1), //Chilled to the Bone
                    (44296, 1), //Oppressive Collapse
                    (62511, 1), //Vile Blast
                    (62539, 1), //Voracious Arc
                    (62563, 1), //Vital Draw
                    (71998, 1), //Devouring Visage
                }
            )

        };

    #region definitions
    // fields
    private readonly FormMain mainLink;
    private int webhookIdsKey;
    private readonly IDictionary<int, DiscordWebhookData> allWebhooks;
    private readonly CellStyle tableCellLeftAlign = new(CellHorizontalAlignment.Left);
    private readonly CellStyle tableCellRightAlign = new(CellHorizontalAlignment.Right);
    private readonly CellStyle tableCellCenterAlign = new(CellHorizontalAlignment.Center);
    private readonly TableBordersStyle tableStyle = TableBordersStyle.HORIZONTAL_ONLY;
    private readonly TableVisibleBorders tableBorders = TableVisibleBorders.HEADER_ONLY;
    #endregion

    static List<T> Shuffle<T>(List<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    internal FormDiscordWebhooks(FormMain mainLink)
    {
        this.mainLink = mainLink;
        InitializeComponent();
        Icon = Properties.Resources.AppIcon;

        allWebhooks = DiscordWebhooks.LoadDiscordWebhooks();

        webhookIdsKey = allWebhooks.Count;

        foreach (var webHook in allWebhooks)
        {
            listViewDiscordWebhooks.Items.Add(new ListViewItem
            {
                Name = webHook.Key.ToString(),
                Text = webHook.Value.Name,
                Checked = webHook.Value.Active,
            });
        }
    }

    public class Tracking
    {
        public double Total { get; set; }
        public double Missed { get; set; }
        public double Blocked { get; set; }
    }

    private (Tracking hits, Tracking cc, Tracking strips) TrackIncoming(TotalDamageTaken[][] incData)
    {
        (Tracking hits, Tracking cc, Tracking strips) tracking = (new Tracking(), new Tracking(), new Tracking());

        for (var i = 0; i < incData.Length; i++)
        {
            for (var j = 0; j < incData[i].Length; j++)
            {
                // track total incoming hits
                if (!incData[i][j].IndirectDamage) tracking.hits.Total += incData[i][j].Hits;

                //track misses from blind
                tracking.hits.Missed += incData[i][j].Missed;

                //track total blocks
                tracking.hits.Blocked += incData[i][j].Blocked;

                switch (incData[i][j].Id)
                {
                    case 71252: //Relic of Cerus
                        //cerusHits += incData[i][j].ConnectedHits;
                        break;
                    case 5487: //Frozen Burst
                    case 29948: //Flash-Freeze
                    case 5538: //Shatterstone
                    case 5515: //Frozen Ground
                    case 5556: //Freezing Gust
                    case 34772: //Glyph of Elemental Power (water)
                    case 25471: //Frozen Ground (Lesser Ice Elemental)
                    case 5725: //Ice Storm (Glyph of Storms)
                    case 5570: //Signet of Water
                    case 5568: //Frost Fan
                    case 42271: //Twin Strike
                    case 42867: //Shearing Edge
                    case 41184: //Monsoon
                    case 42181: //Fiery Frost
                    case 45742: //Glacial Drift
                    case 62862: //Chilling Crack
                    case 62958: //Rain of Blows
                    case 62909: //Shattering Ice
                    case 71935: //Frozen Fusillade
                    case 71863: //Frostfire Flurry
                    case 73061: //Ice Beam
                        ////incSkills[0].chilled += incData[i][j].Hits;
                        ////incSkills[0].softCC += incData[i][j].Hits;
                        break;
                    case 5646: //Convergence
                    case 5519: //Stoning
                    case 24305: //Lightning Rod
                    case 62887: //Crescent Wind
                        ////incSkills[0].weakness += incData[i][j].Hits;
                        ////incSkills[0].softCC += incData[i][j].Hits;
                        break;
                    case 71898: //Purblinding Plasma
                    case 5679: //Flame Burst
                    case 5552: //Lightning Surge
                    case 5661: //Murky Water
                    case 5566: //Steam
                    case 30336: //Dust Storm
                    case 42379: //Ashen Blast
                    case 40332: //Pressure Blast
                    case 5728: //Thunderclap
                    case 5753: //Dust Tornado
                    case 5738: //Sandstorm
                    case 5572: //Signet of Air
                    case 5610: //Steam Vent
                    case 5792: //Blinding Flash
                        ////incSkills[0].blind += incData[i][j].Hits;
                        ////incSkills[0].softCC += incData[i][j].Hits;
                        break;
                    case 5694: //Blinding Flash
                        ////incSkills[0].blind += incData[i][j].Hits;
                        ////incSkills[0].weakness += incData[i][j].Hits;
                        ////incSkills[0].softCC += incData[i][j].Hits * 2;
                        break;
                    case 51662: //Transmute Lightning & Shocking Aura
                    case 5527:
                        ////incSkills[0].stun += incData[i][j].Hits + incData[i][j].Hits * 4 / 2.5;
                        tracking.cc.Total += incData[i][j].Hits + incData[i][j].Hits * 4 / 2.5;
                        tracking.cc.Missed += incData[i][j].Missed * 4 / 2.5;
                        tracking.cc.Blocked += incData[i][j].Blocked * 4 / 2.5;
                        break;
                    case 5671: //Static Field
                    case 5732: //Static Field (Lightning Hammer)
                    case 40794: //Earthen Synergy
                    case 62716: //Shock Blast
                        ////incSkills[0].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5687: //Updraft
                    case 5534: //Tornado
                    case 35304: //Dust Charge
                    case 5733: //Wind Blast
                        ////incSkills[0].launch += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5525: //Ring of Earth
                    case 5522: //Churning Earth
                    case 5528: //Eruption
                    case 5555: //Magnetic Wave
                    case 35224: //Elemental Requiem
                    case 5746: //Crippling Shield
                    case 44550: //Lahar
                    case 5495: //Earth attunement (Earthen Blast)
                    case 5795: //Shock Wave (trait skill)
                    case 72032: //Shattering Stone
                    case 72905: //Earthen Spear
                        ////incSkills[0].cripple += incData[i][j].Hits;
                        ////incSkills[0].softCC += incData[i][j].Hits;
                        break;
                    case 5696: //Dust Devil
                    case 72935: //Lesser Haboob
                        ////incSkills[0].blind += incData[i][j].Hits / 3;
                        ////incSkills[0].cripple += incData[i][j].Hits;
                        ////incSkills[0].softCC += incData[i][j].Hits * 1.33;
                        break;
                    case 5559: //Earthen Rush
                    case 30432: //Aftershock
                    case 5686: //Shock Wave
                    case 5571: //Signet of Earth
                    case 62778: //Ground Pound
                    case 71842: //Boulder Blast
                        //incSkills[0].immobilize += incData[i][j].Hits;
                        //incSkills[0].softCC += incData[i][j].Hits;
                        break;
                    case 5690: //Earthquake
                    case 5562: //Gale
                    case 46018: //Mud Slide
                    case 62947: //Wind Storm
                        //incSkills[0].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30864: //Tidal Surge
                    case 5553: //Gust
                    case 5754: //Debris Tornado
                        //incSkills[0].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30008: //Cyclone
                    case 5747: //Magnetic Shield
                        //incSkills[0].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5490: //Comet
                    case 5547: //Magnetic Surge
                    case 44998: //Polaric Leap
                    case 42321: //Pile Driver
                    case 71966: //Dazing Discharge
                    case 73060: //Lesser Derecho
                    case 73092: //Derecho
                        //incSkills[0].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5721: //Deep Freeze
                        //incSkills[0].chilled += incData[i][j].Hits;
                        //incSkills[0].softCC += incData[i][j].Hits;
                        //incSkills[0].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 46140: //Katabatic Wind
                        //incSkills[0].chilled += incData[i][j].Hits;
                        //incSkills[0].daze += incData[i][j].Hits;
                        //incSkills[0].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 29618: //Overload Earth
                        //incSkills[0].cripple += incData[i][j].Hits * .8;
                        //incSkills[0].immobilize += incData[i][j].Hits * .2;
                        //incSkills[0].softCC += incData[i][j].Hits;
                        break;
                    case 62876: //Grand Finale
                        //incSkills[0].weakness += incData[i][j].Hits * .25;
                        //incSkills[0].softCC += incData[i][j].Hits * .25;
                        break;
                    case 40183: //Primordial Stance
                        //incSkills[0].chilled += incData[i][j].Hits * .25;
                        //incSkills[0].softCC += incData[i][j].Hits * .25;
                        break;
                    case 71857: //Aerial Agility
                    case 71847: //Aerial Agility
                        //incSkills[0].weakness += incData[i][j].Hits;
                        //incSkills[0].blind += incData[i][j].Hits;
                        //incSkills[0].softCC += incData[i][j].Hits * 2;
                        break;
                    case 72023: //Enervating Earth
                    case 73010: //Fissure
                        //incSkills[0].weakness += incData[i][j].Hits;
                        //incSkills[0].cripple += incData[i][j].Hits;
                        //incSkills[0].softCC += incData[i][j].Hits * 2;
                        break;
                    case 73148: //Undertow
                        //incSkills[0].slow += incData[i][j].Hits;
                        //incSkills[0].pull += incData[i][j].Hits;
                        //incSkills[0].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 72998: //Twister
                        //incSkills[0].float += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 73017: //Haboob
                        //incSkills[0].cripple += incData[i][j].Hits;
                        //incSkills[0].blind += incData[i][j].Hits;
                        //incSkills[0].weakness += incData[i][j].Hits;
                        //incSkills[0].softCC += incData[i][j].Hits * 3;
                        break;
                    case 13040: //Shadow Shot
                    case 13113: //Black Powder
                    case 13025: //Infiltrator's Arrow
                    case 13076: //Ink Shot
                    case 30775: //Dust Strike
                    case 13044: //Blinding Powder
                    case 13065: //Smoke Screen
                    case 1148: //Blinding Tuft
                    case 1126: //Throw Feathers
                    case 40888: //Steal Precision
                    case 50408: //Burst of Shadows
                        //incSkills[5].blind += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits;
                        break;
                    case 63351: //Shadow Sap
                    case 29911: //Weakening Charge
                    case 16435: //Shadow Portal
                    case 40904: //Steal Strength
                    case 73063: //Vampiric Slash
                        //incSkills[5].weakness += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits;
                        break;
                    case 13060: //Signet of Shadows
                        //incSkills[5].blind += incData[i][j].Hits;
                        //incSkills[5].weakness += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits * 2;
                        break;
                    case 63267: //Measured Shot
                    case 13008: //Bola Shot
                    case 13015: //Infiltrator's Strike
                    case 13093: //Devourer Venom
                    case 44526: //Steal Mobility
                    case 50451: //Malicious Surprise Shot
                    case 72896: //Entangling Asp
                        //incSkills[5].immobilize += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits;
                        break;
                    case 41205: //Binding Shadow
                        //incSkills[5].immobilize += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits * 2;
                        tracking.strips.Missed += incData[i][j].Missed * 2;
                        tracking.strips.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 63128: //Endless Night
                    case 63067: //Siphon
                    case 28180: //Essence Sap
                    case 42863: //Steal Time
                        //incSkills[5].slow += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits;
                        break;
                    case 13012: //Head Shot
                    case 63230: //Well of Silence
                    case 30568: //Distracting Daggers
                    case 29516: //Impact Strike
                    case 1131: //Mace Head Crack
                        //incSkills[5].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 63292: //Well of Gloom
                    case 13083: //Disabling Shot
                    case 63167: //Grasping Shadows
                    case 30520: //Debilitating Arc
                    case 13019: //Dancing Dagger
                    case 13028: //Caltrops
                    case 13085: //Dagger Storm
                    case 41494: //Skirmisher's Shot
                    case 14136: //Lesser Caltrops
                    case 71835: //Malicious Cunning Salvo
                    case 73041: //Mantis Sting
                    case 72927: //Distracting Throw
                        //incSkills[5].cripple += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits;
                        break;
                    case 63275: //Shadowfall
                        //incSkills[5].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 63220: //Dawn's Repose
                    case 1141: //Skull Fear
                        //incSkills[5].fear += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 63160: //Eternal Night
                    case 13096: //Ice Drake Venom
                    case 1129: //Ice Shard Stab
                    case 39960: //Steal Warmth
                        //incSkills[5].chilled += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits;
                        break;
                    case 63249: //Mind Shock
                    case 13031: //Pistol Whip
                        //incSkills[5].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 13024: //Choking Gas
                    case 71841: //Wild Strikes
                        //incSkills[5].daze += incData[i][j].Hits / 4;
                        tracking.cc.Total += incData[i][j].Hits / 4;
                        tracking.cc.Missed += incData[i][j].Missed / 4;
                        tracking.cc.Blocked += incData[i][j].Blocked / 4;
                        break;
                    case 13116: //Crippling Strike
                        //incSkills[5].cripple += incData[i][j].Hits;
                        //incSkills[5].weakness += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits * 2;
                        break;
                    case 56880: //Pitfall
                        //incSkills[5].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 56898: //Thousand Needles
                        //incSkills[5].immobilize += incData[i][j].Hits * 0.2;
                        //incSkills[5].cripple += incData[i][j].Hits * 0.2;
                        //incSkills[5].softCC += incData[i][j].Hits * 0.4;
                        break;
                    case 30369: //Impairing Daggers
                        //incSkills[5].slow += incData[i][j].Hits;
                        //incSkills[5].immobilize += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits * 2;
                        break;
                    case 30077: //Uppercut
                        //incSkills[5].launch += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 44591: //Spotter's Shot
                        //incSkills[5].cripple += incData[i][j].Hits;
                        //incSkills[5].immobilize += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits * 2;
                        break;
                    case 46335: //Shadow Gust
                        //incSkills[5].launch += incData[i][j].Hits;
                        //incSkills[5].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits * 2;
                        tracking.cc.Missed += incData[i][j].Missed * 2;
                        tracking.cc.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 71864: //Harrowing Storm
                        //incSkills[5].immobilize += incData[i][j].Hits / 5;
                        //incSkills[5].softCC += incData[i][j].Hits / 5;
                        break;
                    case 71965: //Orchestrated Assault
                    case 71895: //Recall Axes
                        //incSkills[5].immobilize += incData[i][j].Hits / 5;
                        //incSkills[5].weakness += incData[i][j].Hits;
                        //incSkills[5].softCC += incData[i][j].Hits * 1.2;
                        break;
                    case 13114: //Tactical Strike
                    case 50484: //Malicious Tactical Strike
                        //incSkills[5].blind += incData[i][j].Hits / 2;
                        //incSkills[5].softCC += incData[i][j].Hits / 2;
                        //incSkills[5].daze += incData[i][j].Hits / 2;
                        tracking.cc.Total += incData[i][j].Hits / 2;
                        tracking.cc.Missed += incData[i][j].Missed / 2;
                        tracking.cc.Blocked += incData[i][j].Blocked / 2;
                        break;
                    case 14505: //Smouldering Arrow
                        //incSkills[1].blind += incData[i][j].Hits;
                        //incSkills[1].softCC += incData[i][j].Hits;
                        break;
                    case 14386: //Fierce Blow
                    case 14378: //Pulverize
                    case 71922: //Path to Victory
                    case 71932: //Path to Victory
                    case 71950: //Path to Victory
                    case 72029: //Path to Victory
                    case 72089: //Path to Victory
                    case 71860: //Line Breaker
                        //incSkills[1].weakness += incData[i][j].Hits;
                        //incSkills[1].softCC += incData[i][j].Hits;
                        break;
                    case 14482: //Hammer Shock
                    case 14363: //Hamstring
                    case 14366: //Savage Leap
                    case 14510: //Bladetrail
                    case 14472: //Explosive Shell
                    case 14398: //Throw Axe
                    case 14498: //Impale
                    case 29845: //Blaze Breaker
                    case 14407: //Banner of Discipline
                    case 29613: //Sundering Leap
                    case 62960: //Dragonspike Mine
                    case 72897: //Maiming Spear
                        //incSkills[1].cripple += incData[i][j].Hits;
                        //incSkills[1].softCC += incData[i][j].Hits;
                        break;
                    case 14359: //Staggering Blow
                    case 14360: //Rifle Butt
                    case 14502: //Kick
                    case 73009: //Spear Swipe
                        //incSkills[1].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 14511: //Backbreaker
                    case 14415: //Tremor
                    case 14516: //Bull's Charge
                    case 29941: //Wild Blow
                        //incSkills[1].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 14387: //Earthshaker
                    case 14512: //Earthshaker
                    case 14513: //Earthshaker
                    case 14514: //Earthshaker
                    case 40601: //Earthshaker
                    case 14361: //Shield Bash
                    case 14414: //Skull Crack
                    case 14425: //Skull Crack
                    case 14426: //Skull Crack
                    case 14427: //Skull Crack
                    case 30343: //Head Butt
                        //incSkills[1].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 44165: //Full Counter
                    case 41243: //Full Counter
                    case 44937: //Disrupting Stab
                    case 14503: //Pommel Bash
                    case 14405: //Banner of Strength
                    case 62732: //Artillery Slash
                        //incSkills[1].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 46233: //Aura Slicer
                        //incSkills[1].slow += incData[i][j].Hits;
                        //incSkills[1].softCC += incData[i][j].Hits;
                        break;
                    case 14367: //Flurry
                    case 14428: //Flurry
                    case 14429: //Flurry
                    case 14430: //Flurry
                    case 42494: //Flurry
                    case 14504: //Pin Down
                    case 14395: //Brutal Shot
                    case 14354: //Throw Bolas
                    case 71875: //Rampart Splitter
                    case 72959: //Disrupting Throw
                        //incSkills[1].immobilize += incData[i][j].Hits;
                        //incSkills[1].softCC += incData[i][j].Hits;
                        break;
                    case 14388: //Stomp
                        //incSkills[1].launch += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 14409: //Fear Me
                        //incSkills[1].fear += incData[i][j].Hits;
                        //incSkills[1].weakness += incData[i][j].Hits;
                        //incSkills[1].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 41919: //Imminent Threat
                        //incSkills[1].taunt += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 62804: //Electric Fence
                        //incSkills[1].cripple += incData[i][j].Hits / 2;
                        //incSkills[1].immobilize += incData[i][j].Hits / 2;
                        //incSkills[1].softCC += incData[i][j].Hits;
                        break;
                    case 72026: //Snap Pull
                        //incSkills[1].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 29679: //Skull Grinder
                        //incSkills[1].cripple += incData[i][j].Hits;
                        //incSkills[1].blind += incData[i][j].Hits;
                        //incSkills[1].softCC += incData[i][j].Hits * 2;
                        //incSkills[1].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5829: //Static Shot
                    case 5824: //Smoke Bomb
                    case 30121: //Flash Shell
                    case 6159: //Smoke Vent
                    case 5808: //Flash Grenade
                    case 6169: //Flash Grenade
                    case 10662: //Plague of Darkness
                    case 5900: //Smoke Screen
                    case 43176: //Flash Spark
                    case 13551: //Smoke Bomb
                        //incSkills[6].blind += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        break;
                    case 6054: //Static Shield
                    case 21661: //Static Shock
                    case 6161: //Throw Mine
                    case 30337: //Throw Mine
                    case 6162: //Detonate
                    case 31248: //Blast Gyro
                    case 5868: //Supply Crate
                    case 63234: //Rocket Fist Prototype
                    case 71888: //Essence of Borrowed Time
                        //incSkills[6].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30713: //Thunderclap
                        //incSkills[6].stun += incData[i][j].Hits / 6;
                        tracking.cc.Total += incData[i][j].Hits / 6;
                        tracking.cc.Missed += incData[i][j].Missed / 6;
                        tracking.cc.Blocked += incData[i][j].Blocked / 6;
                        break;
                    case 5930: //Air Blast
                    case 6126: //Magnetic Inversion
                        //incSkills[6].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 6004: //Net Shot
                    case 5837: //Net Turret
                    case 6179: //Net Attack
                    case 73143: //Electric Artillery
                        //incSkills[6].immobilize += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        break;
                    case 6154: //Overcharged Shot
                    case 5813: //Big Ol' Bomb
                    case 5811: //Personal Battering Ram
                    case 29991: //Personal Battering Ram
                    case 5889: //Thump
                    case 42521: //Holographic Shockwave
                    case 63345: //Core Reactor Shot
                        //incSkills[6].launch += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 31167: //Spare Capacitor
                        //incSkills[6].daze += incData[i][j].Hits / 4;
                        tracking.cc.Total += incData[i][j].Hits / 4;
                        tracking.cc.Missed += incData[i][j].Missed / 4;
                        tracking.cc.Blocked += incData[i][j].Blocked / 4;
                        break;
                    case 5830: //Glue Shot
                    case 5939: //Glue Bomb
                        //incSkills[6].immobilize += incData[i][j].Hits * 0.2;
                        //incSkills[6].cripple += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits * 1.2;
                        break;
                    case 6057: //Throw Shield
                    case 63121: //Jade Mortar
                        //incSkills[6].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5934: //Tranquilizer Dart
                    case 63365: //Explosive Knuckle
                        //incSkills[6].weakness += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        break;
                    case 5935: //Glob Shot
                    case 5999: //Throw Wrench
                    case 5992: //Smack
                    case 5993: //Whack
                    case 5994: //Thwack
                    case 5995: //Box of Nails
                    case 6164: //Mine Field
                    case 6166: //Detonate Mine Field
                    case 5838: //Thumper Turret
                    case 40160: //Radiant Arc
                    case 45732: //Particle Accelerator
                    case 72977: //Roiling Skies
                        //incSkills[6].cripple += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        break;
                    case 5809: //Freeze Grenade
                    case 30307: //Endothermic Shell
                    case 40507: //Coolant Blast
                        //incSkills[6].chilled += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        break;
                    case 5996: //Magnet
                    case 41843: //Prismatic Singularity
                        //incSkills[6].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5982: //Launch Personal Battering Ram
                        //incSkills[6].cripple += incData[i][j].Hits;
                        //incSkills[6].daze += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5825: //Slick Shoes
                    case 30828: //Slick Shoes
                    case 5913: //Explosive Rockets
                        //incSkills[6].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 5893: //Electrified Net
                        //incSkills[6].immobilize += incData[i][j].Hits;
                        //incSkills[6].stun += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 63367: //Discharge Array
                        //incSkills[6].slow += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        break;
                    case 63253: //Force Signet
                        //incSkills[6].cripple += incData[i][j].Hits;
                        //incSkills[6].knockback += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 9097: //Symbol of Blades
                    case 9080: //Leap of Faith
                    case 9191: //Brilliance
                    case 30025: //Purification
                    case 62521: //Roiling Light
                    case 72978: //Gleaming Disc
                        //incSkills[6].blind += incData[i][j].Hits;
                        //incSkills[6].softCC += incData[i][j].Hits;
                        break;
                    case 40624: //Symbol of Vengeance
                    case 30628: //Hunter's Ward
                    case 9168: //Sword of Justice
                    case 44846: //Sword of Justice
                    case 30553: //Fragments of Faith
                    case 29786: //Test of Faith
                    case 29887: //Spear of Justice
                    case 71918: //Hail of Justice
                        //incSkills[4].cripple += incData[i][j].Hits;
                        //incSkills[4].softCC += incData[i][j].Hits;
                        break;
                    case 45402: //Blazing Edge
                    case 42449: //Chapter 3: Heated Rebuke
                    case 9226: //Pull (greatsword 5)
                    case 33134: //Hunter's Verdict
                        //incSkills[4].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 40635: //Chapter 2: Igniting Burst
                    case 9182: //Shield of the Avenger
                    case 45171: //Shield of the Avenger
                    case 62565: //Whirling Light
                    case 72940: //Helio Rush
                    case 73132: //Symbol of Luminance
                        //incSkills[4].weakness += incData[i][j].Hits;
                        //incSkills[4].softCC += incData[i][j].Hits;
                        break;
                    case 41968: //Chapter 2: Daring Challenge
                        //incSkills[4].taunt += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 9260: //Zealot's Embrace
                    case 9099: //Chains of Light
                    case 9151: //Signet of Wrath
                    case 30255: //Lesser Signet of Wrath
                        //incSkills[4].immobilize += incData[i][j].Hits;
                        //incSkills[4].softCC += incData[i][j].Hits;
                        break;
                    case 9124: //Banish
                        //incSkills[4].launch += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30471: //Puncture Shot
                        //incSkills[4].cripple += incData[i][j].Hits * 0.5;
                        //incSkills[4].softCC += incData[i][j].Hits * 0.5;
                        break;
                    case 9091: //Shield of Absorption
                    case 13688: //Lesser Shield of Absorption
                    case 9128: //Sanctuary
                    case 71817: //Jurisdiction (projectile)
                        //incSkills[4].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 9093: //Bane Signet
                    case 9125: //Hammer of Wisdom
                    case 46170: //Hammer of Wisdom
                    case 71989: //Jursidiction (detonate)
                        //incSkills[4].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30871: //Light's Judgement
                        //incSkills[4].daze += incData[i][j].Hits / 9;
                        tracking.cc.Total += incData[i][j].Hits / 9;
                        tracking.cc.Missed += incData[i][j].Missed / 9;
                        tracking.cc.Blocked += incData[i][j].Blocked / 9;
                        break;
                    case 29630: //Deflecting Shot
                        //incSkills[4].blind += incData[i][j].Hits;
                        //incSkills[4].softCC += incData[i][j].Hits;
                        //incSkills[4].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30273: //Dragon's Maw
                        //incSkills[4].pull += incData[i][j].Hits;
                        //incSkills[4].slow += incData[i][j].Hits;
                        //incSkills[4].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 53482: //Glacial Blow
                        //incSkills[4].chilled += incData[i][j].Hits;
                        //incSkills[4].softCC += incData[i][j].Hits;
                        break;
                    case 42360: //Echo of Truth
                        //incSkills[4].blind += incData[i][j].Hits;
                        //incSkills[4].cripple += incData[i][j].Hits;
                        //incSkills[4].weakness += incData[i][j].Hits;
                        //incSkills[4].softCC += incData[i][j].Hits * 3;
                        break;
                    case 44008: //Voice of Truth
                        //incSkills[4].blind += incData[i][j].Hits;
                        //incSkills[4].immobilize += incData[i][j].Hits;
                        //incSkills[4].weakness += incData[i][j].Hits;
                        //incSkills[4].softCC += incData[i][j].Hits * 3;
                        break;
                    case 62549: //Heel Crack
                        //incSkills[4].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 62561: //Heaven's Palm
                        //incSkills[4].knockdown += incData[i][j].Hits * 0.2;
                        //incSkills[4].knockback += incData[i][j].Hits * 0.8;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 29288: //Warding Rift
                    case 27080: //Gaze of Darkness
                        //incSkills[8].blind += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        break;
                    case 43993: //Spiritcrush
                        //incSkills[8].slow += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        break;
                    case 41820: //Scorchrazor
                    case 28110: //Drop the Hammer
                    case 27356: //Energy Expulsion
                    case 29114: //Energy Expulsion
                        //incSkills[8].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 29145: //Mender's Rebuke
                    case 28516: //Inspiring Reinforcement
                    case 27964: //Echoing Eruption
                    case 27187: //Dome of the Mists
                    case 71816: //Blossoming Aura
                        //incSkills[8].weakness += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        break;
                    case 28978: //Surge of the Mists
                        //incSkills[8].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 26679: //Forced Engagement
                        //incSkills[8].taunt += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        //incSkills[8].slow += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 27505: //Banish Enchantment
                        //incSkills[8].chilled += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits;
                        tracking.strips.Missed += incData[i][j].Missed;
                        tracking.strips.Blocked += incData[i][j].Blocked;
                        break;
                    case 62752: //Arcing MIsts
                    case 62895: //Phantom's Onslaught
                    case 27976: //Phase Smash
                    case 29233: //Chilling Isolation
                    case 72972: //Abyssal Force
                        //incSkills[8].chilled += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        break;
                    case 27917: //Call to Anguish
                        //incSkills[8].chilled += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        //incSkills[8].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 72954: //Abyssal Blot
                        //incSkills[8].chilled += incData[i][j].Hits * 0.2;
                        //incSkills[8].softCC += incData[i][j].Hits * 0.2;
                        //incSkills[8].pull += incData[i][j].Hits * 0.2;
                        tracking.cc.Total += incData[i][j].Hits * 0.2;
                        tracking.cc.Missed += incData[i][j].Missed * 0.2;
                        tracking.cc.Blocked += incData[i][j].Blocked * 0.2;
                        break;
                    case 28029: //Frigid Blitz
                        //incSkills[8].chilled += incData[i][j].Hits;
                        //incSkills[8].slow += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits * 2;
                        break;
                    case 73149: //Blitz Mines
                        //incSkills[8].chilled += incData[i][j].Hits;
                        //incSkills[8].slow += incData[i][j].Hits;
                        //incSkills[8].weakness += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits * 2;
                        break;
                    case 62878: //Reaver's Rage
                    case 41220: //Darkrazor's Daring
                    case 41095: //Darkrazor's Daring
                        //incSkills[8].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 28472: //Shackling Wave
                        //incSkills[8].immobilize += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        break;
                    case 28406: //Jade Winds
                    case 31294: //Jade Winds
                        //incSkills[8].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 27162: //Elemental Blast
                        //incSkills[8].chilled += incData[i][j].Hits / 3;
                        //incSkills[8].weakness += incData[i][j].Hits / 3;
                        //incSkills[8].softCC += incData[i][j].Hits * 2 / 3;
                        break;
                    case 28075: //Chaotic Release
                        //incSkills[8].launch += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 40485: //Icerazor's Ire
                        //incSkills[8].cripple += incData[i][j].Hits;
                        //incSkills[8].softCC += incData[i][j].Hits;
                        break;
                    case 71952: //Otherworldly Bond
                        //incSkills[8].cripple += incData[i][j].Hits * 5 / 7;
                        //incSkills[8].slow += incData[i][j].Hits * 3 / 7;
                        //incSkills[8].softCC += incData[i][j].Hits * 8 / 7;
                        break;
                    case 71880: //Otherworldly Attaction
                        //incSkills[8].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 63197: //Unleashed Overbearing Smash
                    case 31406: //Seed of Life
                    case 32242: //Seed of Life
                    case 12498: //Sun Spirit
                    case 41837: //Dark Water
                    case 42180: //Blinding Roar
                    case 12723: //Blinding Slash
                    case 12747: //Dark Water
                        //incSkills[2].blind += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        break;
                    case 31700: //Vine Surge
                    case 12491: //Signet of the Wild
                    case 12580: //Entangle
                    case 44097: //Entangling Web
                        //incSkills[2].immobilize += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        break;
                    case 31318: //Lunar Impact
                    case 63075: //Overbearing Smash
                    case 12598: //Call Lightning
                    case 31658: //Glyph of Equality (non-celestial)
                    case 45743: //Charge
                    case 73150: //Predator's Ambush
                        //incSkills[2].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 31503: //Natural Convergence
                    case 12501: //Muddy Terrain
                    case 13938: //Lesser Muddy Terrain
                        //incSkills[2].cripple += incData[i][j].Hits * 4 / 5;
                        //incSkills[2].slow += incData[i][j].Hits * 4 / 5;
                        //incSkills[2].immobilize += incData[i][j].Hits / 5;
                        //incSkills[2].softCC += incData[i][j].Hits * 1.6;
                        break;
                    case 67179: //Slam - ID seems incorrect?
                    case 12476: //Spike Trap
                    case 71002: //Dimension Breach
                        //incSkills[2].launch += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 63366: //Wild Swing
                    case 12469: //Barrage
                    case 12472: //Crippling Thrust
                    case 43726: //Crippling Leap
                    case 46386: //Tail Lash
                    case 71282: //Lunge
                    case 71885: //Cultivate
                    case 72928: //Falcon's Stoop
                    case 73112: //Wyvern's Lash
                    case 72913: //Owl's Flight
                        //incSkills[2].cripple += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        break;
                    case 63073: //Savage Shock Wave
                    case 31607: //Glyph of Alignment (non-celestial)
                        //incSkills[2].weakness += incData[i][j].Hits;
                        //incSkills[2].immobilize += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits * 2;
                        break;
                    case 63330: //Thump
                    case 42894: //Brutal Charge
                    case 46432: //Brutal Charge
                    case 42907: //Takedown
                        //incSkills[2].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 12523: //Counterattack Kick
                    case 31321: //Wing Buffet
                    case 12511: //Point-Blank Shot
                    case 30448: //Glyph of the Tides (non-celestial)
                    case 43068: //Tail Lash
                    case 41908: //Wing Buffet
                        //incSkills[2].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 12475: //Hilt Bash
                        //incSkills[2].daze += incData[i][j].Hits;
                        //incSkills[2].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits * 2;
                        tracking.cc.Missed += incData[i][j].Missed * 2;
                        tracking.cc.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 12671: //Tail Swipe
                        //incSkills[2].weakness += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        break;
                    case 63258: //Rending Vines
                        //incSkills[2].slow += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits * 2;
                        tracking.strips.Missed += incData[i][j].Missed * 2;
                        tracking.strips.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 63094: //Enveloping Haze
                    case 12490: //Winter's Bite
                    case 12492: //Frost Trap
                        //incSkills[2].chilled += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        break;
                    case 12507: //Crippling Shot
                        //incSkills[2].cripple += incData[i][j].Hits;
                        //incSkills[2].immobilize += incData[i][j].flank;
                        //incSkills[2].softCC += incData[i][j].Hits + incData[i][j].flank;
                        break;
                    case 12508: //Concussion Shot
                        //incSkills[2].daze += incData[i][j].Hits - incData[i][j].flank;
                        //incSkills[2].stun += incData[i][j].flank;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 12638: //Path of Scars
                    case 29558: //Glyph of the Tides (celestial)
                        //incSkills[2].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 12482: //Serpent's Strike
                        //incSkills[2].cripple += incData[i][j].Hits * 0.5;
                        //incSkills[2].immobilize += incData[i][j].Hits * 0.5;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        break;
                    case 12621: //Call of the Wild
                        //incSkills[2].weakness += incData[i][j].Hits;
                        //incSkills[2].daze += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 12599: //Quake
                        //incSkills[2].cripple += incData[i][j].Hits;
                        //incSkills[2].weakness += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits * 2;
                        break;
                    case 44360: //Fear
                        //incSkills[2].fear += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 43375: //Prelude Lash
                        //incSkills[2].pull += incData[i][j].Hits;
                        //incSkills[2].immobilize += incData[i][j].Hits;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 71963: //Oaken Cudgel
                        //incSkills[2].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 72993: //Spider's Web
                        //incSkills[2].immobilize += incData[i][j].Hits / 5;
                        //incSkills[2].cripple += incData[i][j].Hits * 0.8;
                        //incSkills[2].softCC += incData[i][j].Hits;
                        break;
                    case 10173: //Illusionary Leap
                    case 10186: //Temporal Curtain
                    case 10218: //Mind Stab
                    case 10175: //Phantasmal Duelist
                    case 45243: //Lingering Thoughts
                    case 71897: //Journey
                    case 72946: //Phantasmal Lancer
                        //incSkills[7].cripple += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits * 2;
                        tracking.strips.Missed += incData[i][j].Missed * 2;
                        tracking.strips.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 10314: //Counterspell
                    case 10285: //The Prestige
                    case 10331: //Chaos Armor
                    case 10259: //Blinding Tide
                    case 10234: //Signet of Midnight
                    case 42851: //Mirage Advance
                        //incSkills[7].blind += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits;
                        break;
                    case 10337: //Swap
                    case 35637: //Sword of Decimation
                        //incSkills[7].immobilize += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits;
                        break;
                    case 10363: //Into the Void
                        //incSkills[7].pull += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 56873: //Time Sink
                        //incSkills[7].slow += incData[i][j].Hits;
                        //incSkills[7].daze += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30769: //Echo of Memory
                    case 29649: //Deja Vu
                    case 30814: //Well of Action
                        //incSkills[7].slow += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits;
                        break;
                    case 10185: //Arcane Thievery
                        //incSkills[7].slow += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits * 3;
                        tracking.strips.Missed += incData[i][j].Missed * 3;
                        tracking.strips.Blocked += incData[i][j].Blocked * 3;
                        break;
                    case 30643: //Tides of Time
                    case 10232: //Signet of Domination
                    case 72007: //Phantasmal Sharpshooter
                    case 72957: //Mental Collapse
                        //incSkills[7].stun += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30359: //Gravity Well
                        //incSkills[7].knockdown += incData[i][j].Hits / 3;
                        //incSkills[7].pull += incData[i][j].Hits / 3;
                        //incSkills[7].float += incData[i][j].Hits / 3;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 10220: //Illusionary Wave
                    case 62573: //Psychic Force
                        //incSkills[7].knockback += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 10287: //Diversion
                    case 45230: //Mirage Thrust
                    case 62602: //Bladesong Dissonance
                    case 10358: //Counter Blade
                    case 10166: //Phantasmal Mage (Backfire)
                        //incSkills[7].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 44677: //Mirage Mirror
                    case 73095: //Mind Pierce
                        //incSkills[7].weakness += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits;
                        break;
                    case 10169: //Chaos Storm
                    case 13733: //Lesser Chaos Storm
                        //incSkills[7].chilled += incData[i][j].Hits * 5 / 6 * 0.33;
                        //incSkills[7].weakness += incData[i][j].Hits * 5 / 6 * 0.33;
                        //incSkills[7].daze += incData[i][j].Hits / 6;
                        //incSkills[7].softCC += incData[i][j].Hits * 5 / 6 * 0.67;
                        tracking.cc.Total += incData[i][j].Hits / 6;
                        tracking.cc.Missed += incData[i][j].Missed / 6;
                        tracking.cc.Blocked += incData[i][j].Blocked / 6;
                        break;
                    case 10229: //Magic Bullet
                        //incSkills[7].daze += incData[i][j].Hits * 0.25;
                        //incSkills[7].stun += incData[i][j].Hits * 0.25;
                        //incSkills[7].blind += incData[i][j].Hits * 0.25;
                        //incSkills[7].softCC += incData[i][j].Hits * 0.25;
                        tracking.cc.Total += incData[i][j].Hits * 0.5;
                        tracking.cc.Missed += incData[i][j].Missed * 0.5;
                        tracking.cc.Blocked += incData[i][j].Blocked * 0.5;
                        break;
                    case 10341: //Phantasmal Defender
                    case 30192: //Lesser Phantasmal Defender
                        //incSkills[7].taunt += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30525: //Well of Calamity
                        //incSkills[7].cripple += incData[i][j].Hits;
                        //incSkills[7].weakness += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits * 2;
                        break;
                    case 29856: //Well of Senility
                        //incSkills[7].chilled += incData[i][j].Hits;
                        //incSkills[7].daze += incData[i][j].Hits / 4;
                        //incSkills[7].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits / 4;
                        tracking.cc.Missed += incData[i][j].Missed / 4;
                        tracking.cc.Blocked += incData[i][j].Blocked / 4;
                        tracking.strips.Total += incData[i][j].Hits / 2;
                        tracking.strips.Missed += incData[i][j].Missed / 2;
                        tracking.strips.Blocked += incData[i][j].Blocked / 2;
                        break;
                    case 10311: //Time Warp
                    case 10377: //Time Warp
                        //incSkills[7].slow += incData[i][j].Hits;
                        //incSkills[7].chilled += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits * 2;
                        break;
                    case 72076: //Abstraction
                        //incSkills[7].weakness += incData[i][j].Hits;
                        //incSkills[7].blind += incData[i][j].Hits;
                        //incSkills[7].softCC += incData[i][j].Hits * 2;
                        break;
                    case 29867: //Chilling Scythe
                    case 29740: //Grasping Darkness
                    case 10555: //Spinal Shivers
                    case 10605: //Chillblains
                    case 10607: //Well of Darkness
                    case 10672: //Well of Darkness
                    case 30670: //Suffer
                    case 72068: //Gormandize
                    case 73047: //Sinister Stab
                    case 73107: //Isolate
                    case 13906: //Lesser Spinal Shivers
                        //incSkills[3].chilled += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits * 3;
                        tracking.strips.Missed += incData[i][j].Missed * 3;
                        tracking.strips.Blocked += incData[i][j].Blocked * 3;
                        break;
                    case 10705: //Deathly Swarm
                    case 10644: //Dark Water
                    case 30825: //Death's Charge
                    case 62646: //Elixir of Ignorance
                        //incSkills[3].blind += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        break;
                    case 29855: //Nightfall
                        //incSkills[3].cripple += incData[i][j].Hits;
                        //incSkills[3].blind += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits * 2;
                        tracking.strips.Total += incData[i][j].Hits;
                        tracking.strips.Missed += incData[i][j].Missed;
                        tracking.strips.Blocked += incData[i][j].Blocked;
                        break;
                    case 10635: //Lich's Gaze
                    case 10532: //Grasping Dead
                    case 62662: //Elixir of Anguish
                        //incSkills[3].cripple += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        break;
                    case 10701: //Unholy Feast
                    case 29560: //Spiteful Spirit
                        //incSkills[3].cripple += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits * 2;
                        tracking.strips.Missed += incData[i][j].Missed * 2;
                        tracking.strips.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 10633: //Ripple of Horror
                    case 29709: //Terrify
                    case 19115: //Reaper's Mark
                    case 10556: //Wail of Doom
                    case 10608: //Spectral Ring
                    case 44428: //Garish Pillar
                    case 40071: //Garish Pillar
                    case 71998: //Devouring Visage
                        //incSkills[3].fear += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30557: //Executioner's Scythe
                        //incSkills[3].chilled += incData[i][j].Hits;
                        //incSkills[3].stun += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 10529: //Dark Pact
                        //incSkills[3].immobilize += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits * 2;
                        tracking.strips.Missed += incData[i][j].Missed * 2;
                        tracking.strips.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 10570: //Rigor Mortis
                        //incSkills[3].immobilize += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        break;
                    case 10706: //Enfeebling Blood
                    case 10689: //Corrosive Poison Cloud
                    case 29414: //You Are All Weaklings
                    case 62530: //Elixir of Risk
                    case 13907: //Lesser Enfeeble (trait skill)
                    case 71926: //Consume
                        //incSkills[3].weakness += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        break;
                    case 73007: //Extirpate
                        //incSkills[3].weakness += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.strips.Total += incData[i][j].Hits * 2;
                        tracking.strips.Missed += incData[i][j].Missed * 2;
                        tracking.strips.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 10590: //Haunt
                        //incSkills[3].blind += incData[i][j].Hits;
                        //incSkills[3].chilled += incData[i][j].Hits;
                        //incSkills[3].weakness += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits * 3;
                        break;
                    case 10622: //Signet of Spite
                        //incSkills[3].cripple += incData[i][j].Hits;
                        //incSkills[3].weakness += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits * 2;
                        break;
                    case 10620: //Spectral Grasp
                        //incSkills[3].chilled += incData[i][j].Hits;
                        //incSkills[3].pull += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 10549: //Plaguelands
                        //incSkills[3].chilled += incData[i][j].Hits / 9;
                        //incSkills[3].cripple += incData[i][j].Hits / 9;
                        //incSkills[3].weakness += incData[i][j].Hits / 9;
                        //incSkills[3].chilled += incData[i][j].Hits / 9;
                        //incSkills[3].softCC += incData[i][j].Hits * 4 / 9;
                        break;
                    case 10647: //Charge
                        //incSkills[3].launch += incData[i][j].Hits;
                        //incSkills[3].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 30105: //Chilled to the Bone
                        //incSkills[3].chilled += incData[i][j].Hits;
                        //incSkills[3].stun += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 44296: //Oppressive Collapse
                        //incSkills[3].knockdown += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 62511: //Vile Blast
                        //incSkills[3].weakness += incData[i][j].Hits;
                        //incSkills[3].stun += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 62539: //Voracious Arc
                        //incSkills[3].daze += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 62563: //Vital Draw
                        //incSkills[3].float += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 73013: //Addle
                        //incSkills[3].immobilize += incData[i][j].Hits;
                        //incSkills[3].daze += incData[i][j].Hits;
                        //incSkills[3].softCC += incData[i][j].Hits;
                        tracking.cc.Total += incData[i][j].Hits;
                        tracking.cc.Missed += incData[i][j].Missed;
                        tracking.cc.Blocked += incData[i][j].Blocked;
                        break;
                    case 10172: //Mind Spike
                    case 43123: //Break Enchantments
                    case 10203: //Null Field
                    case 10267: //Phantasmal Disenchanter
                    case 10612: //Signet of the Locust
                    case 45333: //Winds of Disenchantment
                    case 51647: //Devouring Darkness
                    case 10709: //Feast of Corruption
                    case 71871: //Gorge
                    case 71799: //Path of Gluttony
                    case 43148: //Sand Flare
                    case 10545: //Well of Corruption
                    case 10671: //Well of Corruption
                    case 42935: //Desiccate
                    case 42917: //Sand Swell
                    case 41615: //Serpent Siphon
                    case 40274: //Trail of Anguish
                    case 42355: //Ghastly Breach
                    case 100074: //Relic of Cerus
                        tracking.strips.Total += incData[i][j].Hits;
                        tracking.strips.Missed += incData[i][j].Missed;
                        tracking.strips.Blocked += incData[i][j].Blocked;
                        break;
                    case 44004: //Wastrel's Ruin
                    case 63129: //Unleashed Ambush skills
                    case 69223:
                    case 72079:
                    case 63336:
                    case 63225:
                    case 63326:
                    case 72932:
                    case 13007: //Larcenous Strike
                    case 72904: //Shattering Assault
                    case 29666: //Nothing Can Save You
                    case 51667: //True Nature (assassin)
                    case 69290: //Slicing Maelstrom
                    case 10602: //Corrupt Boon
                    case 62514: //Elixir of Bliss
                    case 72843: //Panopticon
                    case 54870: //Sandstorm Shroud
                        tracking.strips.Total += incData[i][j].Hits * 2;
                        tracking.strips.Missed += incData[i][j].Missed * 2;
                        tracking.strips.Blocked += incData[i][j].Blocked * 2;
                        break;
                    case 45252: //Breaching Strike
                        tracking.strips.Total += incData[i][j].Hits * 3;
                        tracking.strips.Missed += incData[i][j].Missed * 3;
                        tracking.strips.Blocked += incData[i][j].Blocked * 3;
                        break;
                    case 63350: //Savage Slash
                        tracking.strips.Total += incData[i][j].Hits * 4;
                        tracking.strips.Missed += incData[i][j].Missed * 4;
                        tracking.strips.Blocked += incData[i][j].Blocked * 4;
                        break;
                    case 63438: //Relentless Whirl
                        tracking.strips.Total += incData[i][j].Hits / 5 * 2;
                        tracking.strips.Missed += incData[i][j].Missed / 5 * 2;
                        tracking.strips.Blocked += incData[i][j].Blocked / 5 * 2;
                        break;
                    case 69175: //Solar Brilliance
                        tracking.strips.Total += incData[i][j].Hits / 3;
                        tracking.strips.Missed += incData[i][j].Missed / 3;
                        tracking.strips.Blocked += incData[i][j].Blocked / 3;
                        //incSkills[2].blind += incData[i][j].Hits / 6;
                        //incSkills[2].softCC += incData[i][j].Hits / 6;
                        break;
                    case 10221: //Phantasmal Berserker
                        tracking.strips.Total += incData[i][j].Hits / 2;
                        tracking.strips.Missed += incData[i][j].Missed / 2;
                        tracking.strips.Blocked += incData[i][j].Blocked / 2;
                        break;
                    default:
                        break;
                }
            }
        }

        return tracking;
    }

    private void FormDiscordPings_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
        DiscordWebhooks.SaveToJson(allWebhooks, DiscordWebhooks.JsonFileLocation);
    }

    // Private method to create the Discord fields for each team (both friendly squad and enemies)
    DiscordApiJsonContentEmbedField CreateTeamField<T>(DiscordWebhookData webhook, string name, IEnumerable<T> team, IEnumerable<T> nonSquad = null)
    {
        var teamCount = team.Count();
        var teamDamage = team switch
        {
            IEnumerable<Player> players => players.Select(x => x.DpsTargets.Sum(y => y.Sum(z => z.Damage))).Sum(),
            IEnumerable<Target> enemies => enemies.Select(x => x.DpsAll[0].Damage).Sum(),
            _ => 0
        };
        var teamDps = team switch
        {
            IEnumerable<Player> players => players.Select(x => x.DpsTargets.Sum(y => y.Sum(z => z.Dps))).Sum(),
            IEnumerable<Target> enemies => enemies.Select(x => x.DpsAll[0].Dps).Sum(),
            _ => 0
        };
        var teamDowns = team switch
        {
            IEnumerable<Player> players => players.Select(x => x.Defenses[0].DownCount).Sum(),
            IEnumerable<Target> enemies => enemies.Select(x => x.Defenses[0].DownCount).Sum(),
            _ => 0
        };
        var teamDeaths = team switch
        {
            IEnumerable<Player> players => players.Select(x => x.Defenses[0].DeadCount).Sum(),
            IEnumerable<Target> enemies => enemies.Select(x => x.Defenses[0].DeadCount).Sum(),
            _ => 0
        };

        var teamSummary = new TextTable(2, TableBordersStyle.BLANKS, TableVisibleBorders.NONE);
        teamSummary.SetColumnWidthRange(0, 8, 8);
        teamSummary.SetColumnWidthRange(1, 15, 15);

        teamSummary.AddCell("Count:", tableCellLeftAlign);

        if (nonSquad?.Any() is true)
        {
            teamSummary.AddCell($"{teamCount} (+{nonSquad.Count()})", tableCellLeftAlign);
        }
        else
        {
            teamSummary.AddCell($"{teamCount}", tableCellLeftAlign);
        }

        teamSummary.AddCell("DMG:", tableCellLeftAlign);
        teamSummary.AddCell($"{teamDamage.ParseAsK()}", tableCellLeftAlign);

        teamSummary.AddCell("DPS:", tableCellLeftAlign);
        teamSummary.AddCell($"{teamDps.ParseAsK()}", tableCellLeftAlign);

        teamSummary.AddCell("Downs:", tableCellLeftAlign);
        teamSummary.AddCell($"{teamDowns}", tableCellLeftAlign);

        teamSummary.AddCell("Deaths:", tableCellLeftAlign);
        teamSummary.AddCell($"{teamDeaths}", tableCellLeftAlign);

        var classes = string.Empty;

        if (webhook.ShowClassIcons && webhook.ClassEmojis?.Any() is true)
        {
            var teamClasses = team switch
            {
                IEnumerable<Player> players => players
                    .GroupBy(x => x.Profession.ToUpper())
                    .OrderByDescending(x => x.Count())
                    .Select(x => $"`{(x.Count() < 10 ? " " : "") + x.Count().ToString()}`{{{x.Key}}}")
                    .ToList(),
                IEnumerable<Target> enemies => enemies
                    .GroupBy(x => x.Name.Split(' ').FirstOrDefault().ToUpper())
                    .OrderByDescending(x => x.Count())
                    .Select(x => $"`{(x.Count() < 10 ? " " : "") + x.Count().ToString()}`{{{x.Key}}}")
                    .ToList(),
                _ => new List<string>()
            };

            var emojiPerLine = 4;
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < teamClasses.Count(); i++)
            {
                if (i > 0 && i % emojiPerLine == 0)
                    stringBuilder.Append("\n"); // Add a newline

                stringBuilder.Append(teamClasses.ElementAt(i));
            }

            classes = stringBuilder.ToString();
        }

        return new DiscordApiJsonContentEmbedField()
        {
            Name = $"\n{name}:",
            Value = $"```{TEAM_FORMAT_PREFIX}\n{teamSummary.Render()}``` \n{classes}",
            Inline = true
        };
    }

    // Hits:   [ 44 miss | 59 block ] /  485 = 14.86%
    // CC:     [ 93 miss | 92 block ] / 1245 = 14.86%
    // Strips: [ 93 miss | 92 block ] /  999 = 14.86%

    // Private method to create the Discord fields for each team (both friendly squad and enemies)
    DiscordApiJsonContentEmbedField CreatePreventionField(DiscordWebhookData webhook, string name, IEnumerable<Player> players)
    {
        var preventionSummary = new TextTable(5, TableBordersStyle.UNICODE_BOX, TableVisibleBorders.HEADER_AND_COLUMNS);

        preventionSummary.SetColumnWidthRange(0, 2, 2);
        preventionSummary.SetColumnWidthRange(1, 6, 6);
        preventionSummary.SetColumnWidthRange(2, 6, 6);
        preventionSummary.SetColumnWidthRange(3, 7, 7);
        preventionSummary.SetColumnWidthRange(4, 8, 8);

        //preventionSummary.SetColumnWidthRange(0, 7, 7); // Category
        //preventionSummary.SetColumnWidthRange(1, 3, 3); // [
        //preventionSummary.SetColumnWidthRange(2, 1, 7); // Miss number
        //preventionSummary.SetColumnWidthRange(3, 5, 7); // Miss label
        //preventionSummary.SetColumnWidthRange(4, 3, 3); // |
        //preventionSummary.SetColumnWidthRange(5, 1, 7); // Block number
        //preventionSummary.SetColumnWidthRange(6, 6, 8); // Block label
        //preventionSummary.SetColumnWidthRange(7, 3, 3); // ]
        //preventionSummary.SetColumnWidthRange(8, 2, 2); // /
        //preventionSummary.SetColumnWidthRange(9, 1, 7); // Total number
        //preventionSummary.SetColumnWidthRange(10, 3, 3); // =
        //preventionSummary.SetColumnWidthRange(11, 1, 5); // Percent number
        //preventionSummary.SetColumnWidthRange(12, 2, 2); // %

        var tracking = TrackIncoming(players.SelectMany(p => p.TotalDamageTaken).ToArray());

        preventionSummary.AddCell($"", tableCellLeftAlign);
        preventionSummary.AddCell($"Miss", tableCellRightAlign);
        preventionSummary.AddCell($"Block", tableCellRightAlign);
        preventionSummary.AddCell($"Total", tableCellRightAlign);
        preventionSummary.AddCell($"Percent", tableCellRightAlign);

        // Tracking format
        void addTracking(string name, Tracking tracking)
        {
            if (tracking.Total == 0) return;

            preventionSummary.AddCell($"{name}", tableCellLeftAlign);
            preventionSummary.AddCell($"{Math.Round(tracking.Missed)}", tableCellRightAlign);
            preventionSummary.AddCell($"{Math.Round(tracking.Blocked)}", tableCellRightAlign);
            preventionSummary.AddCell($"{Math.Round(tracking.Total)}", tableCellRightAlign);
            preventionSummary.AddCell($"{Math.Round((tracking.Missed + tracking.Blocked) * 100 / tracking.Total, 2)} %", tableCellRightAlign);

            //preventionSummary.AddCell($"{name}:", tableCellLeftAlign);
            //preventionSummary.AddCell(" [ ", new CellStyle(CellHorizontalAlignment.Left, CellTextTrimmingStyle.Crop, CellNullStyle.EmptyString, false));
            //preventionSummary.AddCell($"{Math.Round(tracking.Missed)}", tableCellRightAlign);
            //preventionSummary.AddCell($" missed", tableCellLeftAlign);
            //preventionSummary.AddCell(" | ", tableCellLeftAlign);
            //preventionSummary.AddCell($"{Math.Round(tracking.Blocked)}", tableCellRightAlign);
            //preventionSummary.AddCell($" blocked", tableCellLeftAlign);
            //preventionSummary.AddCell(" ] ", tableCellLeftAlign);
            //preventionSummary.AddCell("/ ", tableCellLeftAlign);
            //preventionSummary.AddCell($"{Math.Round(tracking.Total)}", tableCellRightAlign);
            //preventionSummary.AddCell(" = ", tableCellLeftAlign);
            //preventionSummary.AddCell($"{Math.Round((tracking.Missed + tracking.Blocked) * 100 / tracking.Total, 2)}", tableCellRightAlign);
            //preventionSummary.AddCell(" %", tableCellLeftAlign);
        }

        // Hits
        addTracking("A", tracking.hits);
        // CC
        addTracking("C", tracking.cc);
        // Strips
        addTracking("S", tracking.strips);

        return new DiscordApiJsonContentEmbedField()
        {
            Name = $"\n{name}:",
            Value = $"```{TEAM_FORMAT_PREFIX}\n{preventionSummary.Render()}``` \n",
            Inline = false
        };
    }

    DiscordApiJsonContentEmbedField CreatePreventionField2(DiscordWebhookData webhook, string name, int[] values)
    {
        var preventionSummary = new TextTable(2, TableBordersStyle.BLANKS, TableVisibleBorders.NONE);

        preventionSummary.SetColumnWidthRange(0, 2, 2);
        preventionSummary.SetColumnWidthRange(1, 10, 10);

        preventionSummary.AddCell($"", tableCellLeftAlign);
        preventionSummary.AddCell(name, tableCellRightAlign);

        if (values.Length >= 3)
        {
            preventionSummary.AddCell($"A:", tableCellLeftAlign);
            preventionSummary.AddCell($"{values[0]}", tableCellRightAlign);

            preventionSummary.AddCell($"C:", tableCellLeftAlign);
            preventionSummary.AddCell($"{values[1]}", tableCellRightAlign);

            preventionSummary.AddCell($"S:", tableCellLeftAlign);
            preventionSummary.AddCell($"{values[2]}", tableCellRightAlign);
        }

        return new DiscordApiJsonContentEmbedField()
        {
            Name = $"{name}",
            Value = $"```{TEAM_FORMAT_PREFIX}\n{preventionSummary.Render()}``` \n",
            Inline = true
        };
    }

    DiscordApiJsonContentEmbedField CreatePreventionField3(DiscordWebhookData webhook, string name, Tracking tracking)
    {
        var preventionSummary = new TextTable(2, TableBordersStyle.BLANKS, TableVisibleBorders.NONE);

        preventionSummary.SetColumnWidthRange(0, 8, 8);
        preventionSummary.SetColumnWidthRange(1, 8, 8);

        preventionSummary.AddCell($"Miss:", tableCellLeftAlign);
        preventionSummary.AddCell($"{Math.Round(tracking.Missed)}", tableCellLeftAlign);
        preventionSummary.AddCell($"Block:", tableCellLeftAlign);
        preventionSummary.AddCell($"{Math.Round(tracking.Blocked)}", tableCellLeftAlign);
        preventionSummary.AddCell($"Total:", tableCellLeftAlign);
        preventionSummary.AddCell($"{Math.Round(tracking.Total)}", tableCellLeftAlign);
        preventionSummary.AddCell($"", tableCellLeftAlign);
        preventionSummary.AddCell($"{Math.Round((tracking.Missed + tracking.Blocked) * 100 / tracking.Total, 2)} %", tableCellLeftAlign);


        return new DiscordApiJsonContentEmbedField()
        {
            Name = $"\n{name}:",
            Value = $"```{TEAM_FORMAT_PREFIX}\n{preventionSummary.Render()}``` \n",
            Inline = true
        };
    }

    DiscordApiJsonContentEmbedField CreateBreakField(DiscordWebhookData webhook, string name = "") => new DiscordApiJsonContentEmbedField()
    {
        Name = $"{name}",
        Value = $"```\n   ``` \n",
        Inline = false
    };

    // Private method to create the formatted rank fields for each category.
    DiscordApiJsonContentEmbedField CreateRankField(DiscordWebhookData webhook, string name, IEnumerable<Player> players, Func<Player, double> getValue)
    {
        var professionReplacement = '@';
        var field = new DiscordApiJsonContentEmbedField();
        var textTable = new TextTable(4, TableBordersStyle.BLANKS, TableVisibleBorders.NONE);
        var useEmoji = webhook.ShowClassIcons && (webhook.ClassEmojis?.Any() is true);

        if (useEmoji)
        {
            textTable.SetColumnWidthRange(0, 1, 1);
            textTable.SetColumnWidthRange(1, 4, 4);
            textTable.SetColumnWidthRange(2, 18, 18);
            textTable.SetColumnWidthRange(3, 10, 10);
        }
        else
        {
            textTable.SetColumnWidthRange(0, 7, 7);
            textTable.SetColumnWidthRange(1, 4, 4);
            textTable.SetColumnWidthRange(2, 14, 14);
            textTable.SetColumnWidthRange(3, 10, 10);
        }

        var rank = 0;
        foreach (var player in players.Where(p => getValue(p) > 0))
        {
            var value = (int)getValue(player);
            var profession = $"({player.ProfessionShort})";

            rank++;

            if (useEmoji)
            {
                var index = webhook.ClassEmojis.FindIndex(x => x.className.Equals(player.Profession, StringComparison.InvariantCultureIgnoreCase));
                textTable.AddCell(professionReplacement.ToString(), tableCellLeftAlign);
                textTable.AddCell($"`{(rank < 10 ? " " : "") + rank}", tableCellCenterAlign);
            }
            else
            {
                textTable.AddCell($"`{profession}", tableCellLeftAlign);
                textTable.AddCell($"{(rank < 10 ? " " : "") + rank}", tableCellCenterAlign);
            }

            textTable.AddCell($"{player.Name}");
            textTable.AddCell($"{value.ParseAsK()}`", tableCellRightAlign);
        }

        // If rank was not incremented, nothing was added to the table.
        if (rank == 0) return null;

        field.Name = $"{name}:";
        field.Value = $"{textTable.Render()}";
        field.Inline = true;

        var emojis = players.Select(p => webhook?.ClassEmojis?.Find(x => x.className.Equals(p.Profession, StringComparison.InvariantCultureIgnoreCase)).emojiCode ?? NO_EMOJI).ToList();

        field.Value = ReplaceCharWithStrings(field.Value, professionReplacement, emojis);

        return field;
    }

    static string ReplaceCharWithStrings(string originalString, char charToReplace, List<string> replacements)
    {
        int replacementIndex = 0; // To keep track of which replacement string to use
        string result = "";

        foreach (char c in originalString)
        {
            if (c == charToReplace)
            {
                // Add the replacement string from the list
                result += replacements[replacementIndex % replacements.Count];
                replacementIndex++; // Move to the next replacement string for the next match
            }
            else
            {
                // If no match, add the original character
                result += c;
            }
        }

        return result;
    }

    /// <summary>
    /// Executes a Discord webhook to post WvW results.
    /// </summary>
    /// <param name="webhook">The webhook data to use when generating the report</param>
    /// <param name="reportJSON">The original report from dps.reports</param>
    /// <param name="players">List of players that are involved in the fight</param>
    /// <param name="discordContentEmbed">The header to include with the report</param>
    /// <returns></returns>
    private async Task ExecuteWebhookWvW(DiscordWebhookData webhook, DpsReportJson reportJSON, List<LogPlayer> players, DiscordApiJsonContentEmbed discordContentEmbed)
    {
        // Get a copy of the max player count for the lists so we can restore it later if we change it temporarily.
        var maxPlayers = webhook.MaxPlayers;

        // Check if any emojis are available.
        if (webhook?.ClassEmojis?.Any() is not true)
        {
            // If none are available, reset to the default.
            webhook?.ResetEmojis();
        }

        while (true)
        {
            // BEAR
            var rankFields = new List<DiscordApiJsonContentEmbedField>();

            // fields
            var discordContentEmbedSquadAndPlayers = new List<DiscordApiJsonContentEmbedField>();
            var discordContentEmbedSquad = new List<DiscordApiJsonContentEmbedField>();
            var discordContentEmbedPlayers = new List<DiscordApiJsonContentEmbedField>();
            var discordContentEmbedNone = new List<DiscordApiJsonContentEmbedField>();

            if (reportJSON.ExtraJson is not null)
            {
                var squadField = CreateTeamField(webhook, "Squad Summary", reportJSON.ExtraJson.Players.Where(x => !x.FriendlyNpc && !x.NotInSquad), reportJSON.ExtraJson.Players.Where(x => !x.FriendlyNpc && x.NotInSquad));

                // enemy summary field
                var enemyFields = new List<DiscordApiJsonContentEmbedField>();
                var enemyGroups = reportJSON.ExtraJson.Targets
                    .GroupBy(x => x.TeamId)
                    .Where(g => g.Count() > 1);

                foreach (var group in enemyGroups)
                {
                    var color = webhook.TeamColorIds.Find(t => t.Ids.Any(i => i == group.Key.ToString()))?.Color;
                    var team = string.IsNullOrEmpty(color) ? $"Team ID ({group.Key})" : $"{color} Team";

                    var enemyField = CreateTeamField(webhook, team, group.Where(x => !x.IsFake));
                    if (enemyField is not null) enemyFields.Add(enemyField);
                }

                // === Ranked field summaries ===

                if (webhook.IncludeDamageSummary)
                {
                    // damage summary
                    var damageStats = reportJSON.ExtraJson.Players
                        .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.DpsTargets.Sum(y => y[0].Damage) > 0))
                        .OrderByDescending(x => x.DpsTargets.Sum(y => y[0].Damage))
                        .Take(webhook.MaxPlayers)
                        .ToArray();

                    var damageField = CreateRankField(webhook, "Damage", damageStats, p => p.DpsTargets.Sum(y => y[0].Damage));
                    if (damageField is not null) rankFields.Add(damageField);
                }

                if (webhook.IncludeDownsContributionSummary)
                {
                    // downs contribution summary
                    var downsContributionStats = reportJSON.ExtraJson.Players
                        .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.StatsTargets.Sum(y => y[0].DownContribution) > 0))
                        .OrderByDescending(x => x.StatsTargets.Sum(y => y[0].DownContribution))
                        .Take(webhook.MaxPlayers)
                        .ToArray();

                    var downsContributionField = CreateRankField(webhook, "Downs Contribution", downsContributionStats, p => p.StatsTargets.Sum(y => y[0].DownContribution));
                    if (downsContributionField is not null) rankFields.Add(downsContributionField);
                }

                if (webhook.IncludeHealingSummary)
                {
                    // healing summary
                    var healingStats = reportJSON.ExtraJson.Players
                        //.Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.ExtHealingStats?.OutgoingHealingAllies?.Any() == true))
                        //.OrderByDescending(x => x.ExtHealingStats.OutgoingHealingAllies.Sum(p => p.FirstOrDefault()?.Healing ?? 0))
                        .Where(x => !x.FriendlyNpc && !x.NotInSquad && ((x.StatsHealing?.TotalHealingOnSquad ?? 0) > 0))
                        .OrderByDescending(x => x.StatsHealing?.TotalHealingOnSquad ?? 0)
                        .Take(webhook.MaxPlayers)
                        .ToArray();

                    var healingField = CreateRankField(webhook, "Healing", healingStats, p => (int)(p.StatsHealing?.TotalHealingOnSquad ?? 0));
                    if (healingField is not null) rankFields.Add(healingField);
                }

                if (webhook.IncludeBarrierSummary)
                {
                    // barrier summary
                    var barrierStats = reportJSON.ExtraJson.Players
                        //.Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.ExtBarrierStats?.OutgoingBarrierAllies?.Any() == true))
                        //.OrderByDescending(x => x.ExtBarrierStats.OutgoingBarrierAllies.Sum(x => x.FirstOrDefault()?.Barrier ?? 0))
                        .Where(x => !x.FriendlyNpc && !x.NotInSquad && ((x.StatsBarrier?.TotalBarrierOnSquad ?? 0) > 0))
                        .OrderByDescending(x => x.StatsBarrier?.TotalBarrierOnSquad ?? 0)
                        .Take(webhook.MaxPlayers)
                        .ToArray();

                    var barrierRatio = 1.0f;

                    if (webhook.AdjustBarrier)
                    {
                        var totalBarrier = reportJSON.ExtraJson.Players.Sum(p => p.StatsBarrier?.TotalBarrierOnSquad ?? 0);
                        var absorbedBarrier = reportJSON.ExtraJson.Players.Sum(p => p.Defenses.FirstOrDefault()?.DamageBarrier ?? 0);
                        barrierRatio = (float)absorbedBarrier / (float)totalBarrier;
                    }

                    var barrierField = CreateRankField(webhook, $"Barrier {(webhook.AdjustBarrier ? " (Adjusted)" : string.Empty)}", barrierStats, p => (int)((p.StatsBarrier?.TotalBarrierOnSquad ?? 0) * barrierRatio));
                    if (barrierField is not null) rankFields.Add(barrierField);
                }

                if (webhook.IncludeCleansingSummary)
                {
                    // cleanses summary
                    var cleansesStats = reportJSON.ExtraJson.Players
                        .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.Support[0].CondiCleanseTotal > 0))
                        .OrderByDescending(x => x.Support[0].CondiCleanseTotal)
                        .Take(webhook.MaxPlayers)
                        .ToArray();

                    var cleansesField = CreateRankField(webhook, "Cleanses", cleansesStats, p => p.Support[0].CondiCleanseTotal);
                    if (cleansesField is not null) rankFields.Add(cleansesField);
                }

                if (webhook.IncludeStripSummary)
                {
                    // boon strips summary
                    var boonStripsStats = reportJSON.ExtraJson.Players
                        .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.Support[0].BoonStrips > 0))
                        .OrderByDescending(x => x.Support[0].BoonStrips)
                        .Take(webhook.MaxPlayers)
                        .ToArray();

                    var boonStripsField = CreateRankField(webhook, "Boon Strips", boonStripsStats, p => p.Support[0].BoonStrips);
                    if (boonStripsField is not null) rankFields.Add(boonStripsField);
                }

                if (webhook.IncludeCCSummary)
                {
                    double filter(Player player) => Math.Round(player.TotalDamageDist.Sum
                    (
                        attack =>
                        {
                            bool map((List<string> category, List<(int id, double coefficient)> skills) map, TotalDamageDist skill) =>
                                map.category.Any
                                (
                                    c =>
                                    c == player.Profession || c == "Relics"
                                ) &&
                                map.skills.Any
                                (
                                    s =>
                                    s.id == skill.Id
                                );

                            double selector(TotalDamageDist skill) =>
                                skill.ConnectedHits * mapping
                                .FirstOrDefault(m => map(m, skill))
                                .skills.FirstOrDefault
                                (
                                    s =>
                                    s.id == skill.Id
                                ).coefficient;

                            return attack
                                // Filter to only skills that match profession and skill ID.
                                .Where(skill => mapping.Any(m => map(m, skill)))
                                // Sum the skills multiplying by matching skill ID coefficient.
                                .Sum(selector);
                        }));

                    // CC summary
                    var ccStats = reportJSON.ExtraJson.Players
                        .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.TotalDamageDist?.Any() == true))
                        .OrderByDescending(filter)
                        .Take(webhook.MaxPlayers)
                        .ToArray();

                    var ccField = CreateRankField(webhook, "CC", ccStats, filter);

                    if (ccField is not null) rankFields.Add(ccField);
                }

                if (webhook.IncludeStabilitySummary)
                {
                    foreach (var player in reportJSON.ExtraJson.Players)
                    {
                        player.StabGeneration = 0;
                    }

                    foreach (var player in reportJSON.ExtraJson.Players)
                    {
                        foreach (var boon in player.BuffUptimes.Where(b => b.Id == STABILITY_ID))
                        {
                            foreach (var boonSource in boon.StatesPerSource.Where(b => reportJSON.ExtraJson.Players.Any(p => p.Name == b.Key)))
                            {
                                var sourcePlayer = reportJSON.ExtraJson.Players.FirstOrDefault(p => p.Name == boonSource.Key);
                                if (sourcePlayer is not null)
                                {
                                    int lastState = 0;
                                    foreach (var state in boonSource.Value)
                                    {
                                        if (state[1] > lastState)
                                        {
                                            sourcePlayer.StabGeneration += (state[1] - lastState);
                                        }
                                        lastState = state[1];
                                    }
                                }
                            }
                        }
                    }

                    // stability summary
                    var stabilityStats = reportJSON.ExtraJson.Players
                        .Where(player => !player.FriendlyNpc && !player.NotInSquad && player.StabGeneration > 0)
                        .OrderByDescending(player => player.StabGeneration)
                        .Take(webhook.MaxPlayers)
                    .ToArray();

                    var stabilityField = CreateRankField(webhook, "Stability", stabilityStats, player => player.StabGeneration);
                    if (stabilityField is not null) rankFields.Add(stabilityField);
                }



                // add the fields
                discordContentEmbed.Fields = new List<DiscordApiJsonContentEmbedField>();

                // BEAR
                // add the fields
                discordContentEmbedSquadAndPlayers.Add(squadField);
                discordContentEmbedSquad.Add(squadField);
                discordContentEmbedSquadAndPlayers.AddRange(enemyFields);
                discordContentEmbedSquad.AddRange(enemyFields);

                discordContentEmbedSquadAndPlayers.Add(FIELD_SPACER);
                discordContentEmbedPlayers.Add(FIELD_SPACER);

                if (webhook.IncludePreventionSummary)
                {
                    var tracking = TrackIncoming(reportJSON.ExtraJson.Players.SelectMany(p => p.TotalDamageTaken).ToArray());

                    var preventionField = CreatePreventionField3(webhook, "Incoming Attacks", tracking.hits);
                    discordContentEmbedSquadAndPlayers.Add(preventionField);
                    discordContentEmbedSquad.Add(preventionField);

                    preventionField = CreatePreventionField3(webhook, "Incoming CC", tracking.cc);
                    discordContentEmbedSquadAndPlayers.Add(preventionField);
                    discordContentEmbedSquad.Add(preventionField);

                    preventionField = CreatePreventionField3(webhook, "Incoming Strips", tracking.strips);
                    discordContentEmbedSquadAndPlayers.Add(preventionField);
                    discordContentEmbedSquad.Add(preventionField);

                    discordContentEmbedSquadAndPlayers.Add(FIELD_SPACER);
                    discordContentEmbedPlayers.Add(FIELD_SPACER);
                }

                // Iterate all the rank fields to be displayed, and add a non-inline spacer
                // between them so they are not all squished on a single line.
                var fieldSpacing = 2;
                var adjustedFields = new List<DiscordApiJsonContentEmbedField>();
                for (int i = 0; i < rankFields.Count; i++)
                {
                    if (i > 0 && i % fieldSpacing == 0)
                        adjustedFields.Add(FIELD_SPACER);

                    adjustedFields.Add(rankFields[i]);
                }

                rankFields = adjustedFields;

                if (webhook.ShowFightAwards)
                {
                    // Add awards
                    var awardFields = AssignAwards(reportJSON.ExtraJson.Players, reportJSON.ExtraJson.Targets, webhook.GoogleSheetsUrl, reportJSON.ExtraJson.SkillList, reportJSON.ExtraJson.BuffList);
                    if (awardFields?.Any() is true) rankFields.AddRange(awardFields);
                }

                // Add a spacer to the last rank field, so the footer is not so close.
                rankFields.Add(FIELD_SPACER);

                discordContentEmbedSquadAndPlayers.AddRange(rankFields);
                discordContentEmbedPlayers.AddRange(rankFields);
            }

            // post to discord
            var discordContentWvW = new DiscordApiJsonContent()
            {
                Embeds = [discordContentEmbed]
            };
            discordContentWvW.Embeds[0].Fields = discordContentEmbedSquadAndPlayers;
            var jsonContentWvWSquadAndPlayers = JsonConvert.SerializeObject(discordContentWvW);
            discordContentWvW.Embeds[0].Fields = discordContentEmbedSquad;
            var jsonContentWvWSquad = JsonConvert.SerializeObject(discordContentWvW);
            discordContentWvW.Embeds[0].Fields = discordContentEmbedPlayers;
            var jsonContentWvWPlayers = JsonConvert.SerializeObject(discordContentWvW);
            discordContentWvW.Embeds[0].Fields = discordContentEmbedNone;
            var jsonContentWvWNone = JsonConvert.SerializeObject(discordContentWvW);

            var bossData = Bosses.GetBossDataFromId(1);

            try
            {
                await SendLogViaWebhook
                (
                    webhook,
                    reportJSON.Encounter.Success ?? false,
                    reportJSON.Encounter.BossId,
                    false,
                    false,
                    bossData, players,
                    jsonContentWvWNone, jsonContentWvWSquad, jsonContentWvWPlayers, jsonContentWvWSquadAndPlayers
                );
            }
            catch (DiscordSizeException ex)
            {
                // Discord returned a size error when trying to post the report, adjust and retry.
                mainLink.AddToText($">:> Discord returned a size error while processing the webhook \"{webhook.Name}\": {ex.Message}");
                // Check if the list will contain at least one player after the reduction
                if (webhook.MaxPlayers > 1)
                {
                    // Reduce the player count for the list
                    webhook.MaxPlayers--;
                    mainLink.AddToText($">:> Reducing the number of players in lists to {webhook.MaxPlayers}, and retrying...");
                    // Retry
                    continue;
                }
            }
            catch (DiscordException ex)
            {
                // Discord returned an error when trying to post the report, just show it.
                mainLink.AddToText($">:> Discord returned an error while processing the webhook \"{webhook.Name}\": {ex.Message}");
            }

            // Success, break out of the loop
            break;
        }

        // Restore the player count in case it was temporariy reduced when retrying a Discord post.
        webhook.MaxPlayers = maxPlayers;
    }

    internal async Task ExecuteAllActiveWebhooksAsync(DpsReportJson reportJson, List<LogPlayer> players)
    {
        if (reportJson.Encounter.BossId.Equals(1)) // WvW
        {
            var extraJSONFightName = (reportJson.ExtraJson is null) ? reportJson.Encounter.Boss : reportJson.ExtraJson.FightName;
            var extraJSON = (reportJson.ExtraJson is null) ? "" : $"Recorded by: {reportJson.ExtraJson.RecordedByAccountName}\nDuration: {reportJson.ExtraJson.Duration}\nElite Insights version: {reportJson.ExtraJson.EliteInsightsVersion}";
            const int colour = 16752238;
            var timestampDateTime = DateTime.UtcNow;
            if (reportJson.ExtraJson is not null)
            {
                timestampDateTime = reportJson.ExtraJson.TimeStart;
            }
            var timestamp = timestampDateTime.ToString("o");
            var discordContentEmbed = new DiscordApiJsonContentEmbed()
            {
                Title = extraJSONFightName,
                Url = reportJson.ConfigAwarePermalink,
                Description = $"{extraJSON}\narcdps version: {reportJson.Evtc.Type}{reportJson.Evtc.Version}{STRETCH_FIELD}",
                Colour = colour,
                TimeStamp = timestamp,
                // Don't  include the thumbnail, it compresses the usable space too much.
            };

            foreach (var key in allWebhooks.Keys)
            {
                var webhook = allWebhooks[key];

                await ExecuteWebhookWvW(webhook, reportJson, players, discordContentEmbed);
            }

            if (allWebhooks.Count > 0)
            {
                mainLink.AddToText(">:> All active webhooks executed.");
            }
            return;
        }
        else // not WvW
        {
            var bossName = $"{reportJson.Encounter.Boss}{(reportJson.ChallengeMode ? " CM" : "")}";
            var successString = (reportJson.Encounter.Success ?? false) ? ":white_check_mark:" : "❌";
            var lastTarget = (reportJson?.ExtraJson?.PossiblyLastTarget is not null) ? $"\n{reportJson.ExtraJson.PossiblyLastTarget.Name} ({Math.Round(100 - reportJson.ExtraJson.PossiblyLastTarget.HealthPercentBurned, 2)}%)" : "";
            var extraJSON = (reportJson.ExtraJson is null) ? "" : $"Recorded by: {reportJson.ExtraJson.RecordedByAccountName}\nDuration: {reportJson.ExtraJson.Duration}{lastTarget}\nElite Insights version: {reportJson.ExtraJson.EliteInsightsVersion}\n";
            var icon = "";
            var bossData = Bosses.GetBossDataFromId(reportJson.Encounter.BossId);
            if (bossData is not null)
            {
                bossName = bossData.FightName(reportJson);
                icon = bossData.Icon;
            }
            var colour = (reportJson.Encounter.Success ?? false) ? 32768 : 16711680;
            var discordContentEmbedThumbnail = new DiscordApiJsonContentEmbedThumbnail()
            {
                Url = icon,
            };
            var timestampDateTime = DateTime.UtcNow;
            if (reportJson.ExtraJson is not null)
            {
                timestampDateTime = reportJson.ExtraJson.TimeStart;
            }
            var timestamp = timestampDateTime.ToString("o");
            var discordContentEmbed = new DiscordApiJsonContentEmbed()
            {
                Title = bossName,
                Url = reportJson.ConfigAwarePermalink,
                Description = $"{extraJSON}Result: {successString}\narcdps version: {reportJson.Evtc.Type}{reportJson.Evtc.Version}",
                Colour = colour,
                TimeStamp = timestamp,
                Thumbnail = discordContentEmbedThumbnail,
            };
            var discordContentEmbedSpacer = new DiscordApiJsonContentEmbed()
            {
                Title = "Log summary",
                Description = DiscordApiJsonContent.Spacer,
                Colour = colour,
                TimeStamp = timestamp,
            };
            var discordContentEmbedSquadAndPlayers = new List<DiscordApiJsonContentEmbedField>();
            var discordContentEmbedSquad = new List<DiscordApiJsonContentEmbedField>();
            var discordContentEmbedPlayers = new List<DiscordApiJsonContentEmbedField>();
            var discordContentEmbedNone = new List<DiscordApiJsonContentEmbedField>();
            if (reportJson.Players.Values.Count <= 10)
            {
                if (reportJson.ExtraJson is null)
                {
                    // player list
                    var playerNames = new TextTable(2, tableStyle, tableBorders);
                    playerNames.SetColumnWidthRange(0, 21, 21);
                    playerNames.SetColumnWidthRange(1, 20, 20);
                    playerNames.AddCell("Character");
                    playerNames.AddCell("Account name");
                    foreach (var player in reportJson.Players.Values)
                    {
                        playerNames.AddCell($"{player.CharacterName}");
                        playerNames.AddCell($"{player.DisplayName}");
                    }
                    var squadEmbedField = new DiscordApiJsonContentEmbedField()
                    {
                        Name = "Players in squad/group:",
                        Value = $"```{playerNames.Render()}```",
                    };
                    discordContentEmbedSquadAndPlayers.Add(squadEmbedField);
                    discordContentEmbedSquad.Add(squadEmbedField);
                }
                else
                {
                    // player list
                    var playerNames = new TextTable(2, tableStyle, tableBorders);
                    playerNames.SetColumnWidthRange(0, 23, 23);
                    playerNames.SetColumnWidthRange(1, 23, 23);
                    playerNames.AddCell("Character");
                    playerNames.AddCell("Account name");
                    foreach (var player in reportJson.ExtraJson.Players.Where(x => !x.FriendlyNpc).OrderBy(x => x.Name))
                    {
                        playerNames.AddCell($"{player.Name}");
                        playerNames.AddCell($"{player.Account}");
                    }
                    var squadEmbedField = new DiscordApiJsonContentEmbedField()
                    {
                        Name = "Players in squad/group:",
                        Value = $"```{playerNames.Render()}```",
                    };
                    discordContentEmbedSquadAndPlayers.Add(squadEmbedField);
                    discordContentEmbedSquad.Add(squadEmbedField);
                    var numberOfRealTargers = reportJson.ExtraJson.Targets
                        .Count(x => !x.IsFake);
                    // damage summary
                    var targetDps = reportJson.ExtraJson.GetPlayerTargetDps();
                    var damageStats = reportJson.ExtraJson.Players
                        .Where(x => !x.FriendlyNpc)
                        .Select(x => new
                        {
                            Player = x,
                            DPS = (numberOfRealTargers > 0) ? targetDps[x] : x.DpsAll[0].Dps,
                        })
                        .OrderByDescending(x => x.DPS)
                        .Take(10)
                        .ToArray();
                    var dpsTargetSummary = new TextTable(3, tableStyle, TableVisibleBorders.HEADER_AND_FOOTER);
                    dpsTargetSummary.SetColumnWidthRange(0, 5, 5);
                    dpsTargetSummary.SetColumnWidthRange(1, 31, 31);
                    dpsTargetSummary.SetColumnWidthRange(2, 10, 10);
                    dpsTargetSummary.AddCell("#", tableCellCenterAlign);
                    dpsTargetSummary.AddCell("Name");
                    dpsTargetSummary.AddCell("DPS", tableCellRightAlign);
                    var rank = 0;
                    foreach (var player in damageStats)
                    {
                        rank++;
                        dpsTargetSummary.AddCell($"{rank}", tableCellCenterAlign);
                        dpsTargetSummary.AddCell($"{player.Player.Name} ({player.Player.ProfessionShort})");
                        dpsTargetSummary.AddCell($"{player.DPS.ParseAsK()}", tableCellRightAlign);
                    }
                    dpsTargetSummary.AddCell("");
                    dpsTargetSummary.AddCell("Total");
                    var totalDPS = damageStats
                        .Select(x => x.DPS)
                        .Sum();
                    dpsTargetSummary.AddCell($"{totalDPS.ParseAsK()}", tableCellRightAlign);
                    var playersEmbedField = new DiscordApiJsonContentEmbedField()
                    {
                        Name = "DPS target summary:",
                        Value = $"```{dpsTargetSummary.Render()}```",
                    };
                    discordContentEmbedSquadAndPlayers.Add(playersEmbedField);
                    discordContentEmbedPlayers.Add(playersEmbedField);
                }
            }

            var discordContent = new DiscordApiJsonContent
            {
                Embeds = [discordContentEmbed],
            };
            discordContent.Embeds[0].Fields = discordContentEmbedNone;
            var jsonContentNone = JsonConvert.SerializeObject(discordContent);
            discordContent.Embeds.Add(discordContentEmbedSpacer);

            discordContent.Embeds[1].Fields = discordContentEmbedSquadAndPlayers;
            var jsonContentSquadAndPlayers = JsonConvert.SerializeObject(discordContent);
            discordContent.Embeds[1].Fields = discordContentEmbedSquad;
            var jsonContentSquad = JsonConvert.SerializeObject(discordContent);
            discordContent.Embeds[1].Fields = discordContentEmbedPlayers;
            var jsonContentPlayers = JsonConvert.SerializeObject(discordContent);

            await SendLogViaWebhooks(reportJson.Encounter.Success ?? false,
                reportJson.Encounter.BossId,
                reportJson.ChallengeMode,
                reportJson.LegendaryChallengeMode,
                bossData,
                players,
                jsonContentNone,
                jsonContentSquad,
                jsonContentPlayers,
                jsonContentSquadAndPlayers);

        }
        if (allWebhooks.Count > 0)
        {
            mainLink.AddToText(">:> All active webhooks executed.");
        }
    }

    internal async Task SendLogViaWebhook(DiscordWebhookData webhook, bool success, int bossId, bool isCm, bool isLegendaryCm, BossData bossData, List<LogPlayer> players, string jsonContentNone, string jsonContentSquad, string jsonContentPlayers, string jsonContentSquadAndPlayers)
    {
        if (!webhook.Active
            || (webhook.SuccessFailToggle.Equals(DiscordWebhookDataSuccessToggle.OnSuccessOnly) && !success)
            || (webhook.SuccessFailToggle.Equals(DiscordWebhookDataSuccessToggle.OnFailOnly) && success)
            || (!webhook.IncludeNormalLogs && !isCm)
            || (!webhook.IncludeChallengeModeLogs && isCm && !isLegendaryCm)
            || (!webhook.IncludeLegendaryChallengeModeLogs && isLegendaryCm)
            || webhook.BossesDisable.Contains(bossId)
            || (!webhook.AllowUnknownBossIds && (bossData is null))
            || (!webhook.Team.IsSatisfied(players)))
        {
            return;
        }
        try
        {
            var jsonToSend = webhook.SummaryType switch
            {
                DiscordWebhookDataLogSummaryType.None => jsonContentNone,
                DiscordWebhookDataLogSummaryType.SquadOnly => jsonContentSquad,
                DiscordWebhookDataLogSummaryType.PlayersOnly => jsonContentPlayers,
                _ => jsonContentSquadAndPlayers,
            };

            foreach (var (className, emojiCode) in webhook.ClassEmojis)
            {
                jsonToSend = jsonToSend.Replace($"{{{className}}}", emojiCode);
            }

            // In case there was a class we don't have define in the emoji map, replace it with generic symbol.
            var pattern = @"\{[A-Z]+\}";
            jsonToSend = Regex.Replace(jsonToSend, pattern, NO_EMOJI);

            using var content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");
            using var response = await mainLink.HttpClientController.PostAsync(webhook.Url, content);

            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                if (responseString.Contains("exceeds maximum size", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new DiscordSizeException($"Discord error: Embed exceeds maximum size");
                }
                else
                {
                    throw new DiscordException($"Discord error");
                }
            }
        }
        catch (DiscordException ex)
        {
            // Just forward to caller.
            throw;
        }
        catch (UriFormatException ex)
        {
            mainLink.AddToText($">:> An error has occured while processing URL for the webhook \"{webhook.Name}\": {ex.Message}");
        }
        catch (Exception ex)
        {
            mainLink.AddToText($">:> An error has occured while processing the webhook \"{webhook.Name}\": {ex.Message}");
        }
    }

    internal async Task SendLogViaWebhooks(bool success, int bossId, bool isCm, bool isLegendaryCm, BossData bossData, List<LogPlayer> players, string jsonContentNone, string jsonContentSquad, string jsonContentPlayers, string jsonContentSquadAndPlayers)
    {
        foreach (var key in allWebhooks.Keys)
        {
            var webhook = allWebhooks[key];

            try
            {
                await SendLogViaWebhook(webhook, success, bossId, isCm, isLegendaryCm, bossData, players, jsonContentNone, jsonContentSquad, jsonContentPlayers, jsonContentSquadAndPlayers);
            }
            catch (DiscordException ex)
            {
                mainLink.AddToText($">:> Discord returned an error while processing the webhook \"{webhook.Name}\": {ex.Message}");
            }
        }
    }

    internal async Task ExecuteSessionWebhooksAsync(List<DpsReportJson> reportJson, LogSessionSettings logSessionSettings)
    {
        if (logSessionSettings.UseSelectedWebhooksInstead)
        {
            foreach (var webhook in logSessionSettings.SelectedWebhooks)
            {
                var discordEmbeds = SessionTextConstructor.ConstructSessionEmbeds(reportJson.Where(x => webhook.Team.IsSatisfied(x.GetLogPlayers())).ToList(), logSessionSettings);
                await SendDiscordMessageWebhooksAsync(webhook, discordEmbeds, logSessionSettings.ContentText);
            }
        }
        else if (logSessionSettings.ExcludeSelectedWebhooksInstead)
        {
            foreach (var webhook in allWebhooks.Values.Except(logSessionSettings.SelectedWebhooks))
            {
                var discordEmbeds = SessionTextConstructor.ConstructSessionEmbeds(reportJson.Where(x => webhook.Team.IsSatisfied(x.GetLogPlayers())).ToList(), logSessionSettings);
                await SendDiscordMessageWebhooksAsync(webhook, discordEmbeds, logSessionSettings.ContentText);
            }
        }
        else
        {
            foreach (var webhook in allWebhooks.Values.Where(x => x.Active))
            {
                var discordEmbeds = SessionTextConstructor.ConstructSessionEmbeds(reportJson.Where(x => webhook.Team.IsSatisfied(x.GetLogPlayers())).ToList(), logSessionSettings);
                await SendDiscordMessageWebhooksAsync(webhook, discordEmbeds, logSessionSettings.ContentText);
            }
        }
        if (logSessionSettings.UseSelectedWebhooksInstead && logSessionSettings.SelectedWebhooks.Count > 0)
        {
            mainLink.AddToText(">:> All selected webhooks successfully executed with finished log session.");
        }
        else if (logSessionSettings.ExcludeSelectedWebhooksInstead && logSessionSettings.SelectedWebhooks.Count > 0)
        {
            mainLink.AddToText(">:> All non-selected webhooks successfully executed with finished log session.");
        }
        else if (allWebhooks.Count > 0)
        {
            mainLink.AddToText(">:> All active webhooks successfully executed with finished log session.");
        }
    }

    private async Task SendDiscordMessageWebhooksAsync(DiscordWebhookData webhook, SessionTextConstructor.DiscordEmbeds discordEmbeds, string contentText)
    {
        var jsonContentSuccessFailure = JsonConvert.SerializeObject(new DiscordApiJsonContent
        {
            Content = contentText,
            Embeds = discordEmbeds.SuccessFailure,
        });
        var jsonContentSuccess = JsonConvert.SerializeObject(new DiscordApiJsonContent
        {
            Content = contentText,
            Embeds = discordEmbeds.Success,
        });
        var jsonContentFailure = JsonConvert.SerializeObject(new DiscordApiJsonContent
        {
            Content = contentText,
            Embeds = discordEmbeds.Failure,
        });
        try
        {
            var jsonContent = webhook.SuccessFailToggle switch
            {
                DiscordWebhookDataSuccessToggle.OnSuccessAndFailure => jsonContentSuccessFailure,
                DiscordWebhookDataSuccessToggle.OnSuccessOnly => jsonContentSuccess,
                _ => jsonContentFailure,
            };

            using var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            await mainLink.HttpClientController.PostAsync(webhook.Url, content);
        }
        catch
        {
            mainLink.AddToText($">:> Unable to execute webhook \"{webhook.Name}\" with a finished log session.");
        }
    }

    private void ToolStripMenuItemDelete_Click(object sender, EventArgs e)
    {
        if (listViewDiscordWebhooks.SelectedItems.Count == 0)
        {
            return;
        }
        var selected = listViewDiscordWebhooks.SelectedItems[0];
        if (!int.TryParse(selected.Name, out var reservedId))
        {
            return;
        }
        listViewDiscordWebhooks.Items.RemoveByKey(reservedId.ToString());
        allWebhooks.Remove(reservedId);
    }

    private void ToolStripMenuItemEdit_Click(object sender, EventArgs e)
    {
        if (listViewDiscordWebhooks.SelectedItems.Count == 0)
        {
            return;
        }
        var selected = listViewDiscordWebhooks.SelectedItems[0];
        if (int.TryParse(selected.Name, out var reservedId))
        {
            new FormEditDiscordWebhook(this, allWebhooks[reservedId], reservedId).ShowDialog();
        }
    }

    private void ListViewDiscordWebhooks_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
        if (!int.TryParse(e.Item.Name, out var reservedId))
        {
            return;
        }
        allWebhooks[reservedId].Active = e.Item.Checked;
    }

    private void ContextMenuStripInteract_Opening(object sender, CancelEventArgs e)
    {
        var toggle = listViewDiscordWebhooks.SelectedItems.Count > 0;
        toolStripMenuItemEdit.Enabled = toggle;
        toolStripMenuItemDelete.Enabled = toggle;
        toolStripMenuItemTest.Enabled = toggle;
    }

    private async void ToolStripMenuItemTest_Click(object sender, EventArgs e)
    {
        if (listViewDiscordWebhooks.SelectedItems.Count == 0)
        {
            return;
        }
        var selected = listViewDiscordWebhooks.SelectedItems[0];
        if (!int.TryParse(selected.Name, out var reservedId))
        {
            return;
        }
        if (await allWebhooks[reservedId].TestWebhookAsync(mainLink.HttpClientController))
        {
            MessageBox.Show("Webhook is valid.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show("Webhook is not valid.\nCheck your URL.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddNewClick()
    {
        webhookIdsKey++;
        new FormEditDiscordWebhook(this, null, webhookIdsKey).ShowDialog();
    }

    private void ButtonAddNew_Click(object sender, EventArgs e) => AddNewClick();

    private void ToolStripMenuItemAdd_Click(object sender, EventArgs e) => AddNewClick();

    internal void CheckBoxShortenThousands_CheckedChanged(object sender, EventArgs e)
    {
        ApplicationSettings.Current.ShortenThousands = checkBoxShortenThousands.Checked;
        ApplicationSettings.Current.Save();
    }
}
