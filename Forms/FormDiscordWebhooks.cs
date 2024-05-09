using Newtonsoft.Json;
using PlenBotLogUploader.AppSettings;
using PlenBotLogUploader.DiscordApi;
using PlenBotLogUploader.DiscordApi.Awards;
using PlenBotLogUploader.DpsReport;
using PlenBotLogUploader.DpsReport.ExtraJson;
using PlenBotLogUploader.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextTableFormatter;
using static System.Net.WebRequestMethods;

namespace PlenBotLogUploader
{
    public partial class FormDiscordWebhooks : Form
    {
        private const int RED_TEAM = 705;
        private const int BLUE_TEAM = 432;
        private const int GREEN_TEAM = 2752;
        private const string NO_EMOJI = "⭕";
        private const string TEAM_FORMAT_PREFIX = "java";
        private const string STRETCH_FIELD = "";

        private const int CHILLED_ID = 722;
        private const int POISON_ID = 723;
        private const int FEARED_ID = 791;

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

            var pattern = @"\/spreadsheets\/d\/([0-9a-zA-Z]+)\/";
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

        private void FormDiscordPings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
            DiscordWebhooks.SaveToJson(allWebhooks, DiscordWebhooks.JsonFileLocation);
        }

        internal async Task ExecuteAllActiveWebhooksAsync(DpsReportJson reportJSON, List<LogPlayer> players)
        {
            if (reportJSON.Encounter.BossId.Equals(1)) // WvW
            {
                var extraJSONFightName = (reportJSON.ExtraJson is null) ? reportJSON.Encounter.Boss : reportJSON.ExtraJson.FightName;
                var extraJSON = (reportJSON.ExtraJson is null) ? "" : $"Recorded by: {reportJSON.ExtraJson.RecordedByAccountName}\nDuration: {reportJSON.ExtraJson.Duration}\nElite Insights version: {reportJSON.ExtraJson.EliteInsightsVersion}";
                var icon = "";
                var bossData = Bosses.GetBossDataFromId(1);
                if (bossData is not null)
                {
                    icon = bossData.Icon;
                }
                const int colour = 16752238;
                var discordContentEmbedThumbnail = new DiscordApiJsonContentEmbedThumbnail()
                {
                    Url = icon,
                };
                var timestampDateTime = DateTime.UtcNow;
                if (reportJSON.ExtraJson is not null)
                {
                    timestampDateTime = reportJSON.ExtraJson.TimeStart;
                }
                var timestamp = timestampDateTime.ToString("o");
                var discordContentEmbed = new DiscordApiJsonContentEmbed()
                {
                    Title = extraJSONFightName,
                    Url = reportJSON.ConfigAwarePermalink,
                    Description = $"{extraJSON}\narcdps version: {reportJSON.Evtc.Type}{reportJSON.Evtc.Version}{STRETCH_FIELD}",
                    Colour = colour,
                    TimeStamp = timestamp,
                    // Don't  include the thumbnail, it compresses the usable space too much.
                    //Thumbnail = discordContentEmbedThumbnail,
                };

                foreach (var key in allWebhooks.Keys)
                {
                    var webhook = allWebhooks[key];

                    // Check if any emojis are available.
                    if (webhook?.ClassEmojis?.Any() is not true)
                    {
                        // If none are available, reset to the default.
                        webhook?.ResetEmojis();
                    }

                    // Private method to create the Discord fields for each team (both friendly squad and enemies)
                    DiscordApiJsonContentEmbedField CreateTeamField<T>(string name, IEnumerable<T> team)
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
                        teamSummary.AddCell($"{teamCount}", tableCellLeftAlign);

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

                    // Private method to create the formatted rank fields for each category.
                    DiscordApiJsonContentEmbedField CreateRankField(string name, IEnumerable<Player> players, Func<Player, int> getValue)
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
                            var value = getValue(player);
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

                    // BEAR
                    var rankFields = new List<DiscordApiJsonContentEmbedField>();

                    // fields
                    var discordContentEmbedSquadAndPlayers = new List<DiscordApiJsonContentEmbedField>();
                    var discordContentEmbedSquad = new List<DiscordApiJsonContentEmbedField>();
                    var discordContentEmbedPlayers = new List<DiscordApiJsonContentEmbedField>();
                    var discordContentEmbedNone = new List<DiscordApiJsonContentEmbedField>();

                    if (reportJSON.ExtraJson is not null)
                    {
                        var squadField = CreateTeamField("Squad Summary", reportJSON.ExtraJson.Players.Where(x => !x.FriendlyNpc && !x.NotInSquad));

                        // enemy summary field
                        var enemyFields = new List<DiscordApiJsonContentEmbedField>();
                        var enemyGroups = reportJSON.ExtraJson.Targets
                            .GroupBy(x => x.TeamId)
                            .Where(g => g.Count() > 1);

                        foreach (var group in enemyGroups)
                        {
                            var team = group.Key switch
                            {
                                RED_TEAM => "Red Team",
                                BLUE_TEAM => "Blue Team",
                                GREEN_TEAM => "Green Team",
                                _ => $"Team ID ({group.Key})"
                            };

                            var enemyField = CreateTeamField(team, group.Where(x => !x.IsFake));
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

                            var damageField = CreateRankField("Damage Summary", damageStats, p => p.DpsTargets.Sum(y => y[0].Damage));
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

                            var downsContributionField = CreateRankField("Downs Contribution", downsContributionStats, p => p.StatsTargets.Sum(y => y[0].DownContribution));
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

                            var healingField = CreateRankField("Healing Summary", healingStats, p => (int)(p.StatsHealing?.TotalHealingOnSquad ?? 0));
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

                            var barrierField = CreateRankField($"Barrier Summary{(webhook.AdjustBarrier ? " (Adjusted)" : string.Empty)}", barrierStats, p => (int)((p.StatsBarrier?.TotalBarrierOnSquad ?? 0) * barrierRatio));
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

                            var cleansesField = CreateRankField("Cleanses Summary", cleansesStats, p => p.Support[0].CondiCleanseTotal);
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

                            var boonStripsField = CreateRankField("Boon Strip Summary", boonStripsStats, p => p.Support[0].BoonStrips);
                            if (boonStripsField is not null) rankFields.Add(boonStripsField);
                        }

                        if (webhook.IncludeCCSummary)
                        {
                            // CC summary
                            var ccStats = reportJSON.ExtraJson.Players
                                .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.TotalDamageDist?.Any() == true))
                                .OrderByDescending
                                (
                                    player => player.TotalDamageDist.Sum
                                    (
                                        attack => attack
                                        // Filter to only skills that match profession and skill ID.
                                        .Where(
                                            skill =>
                                            mapping.Any(
                                                m =>
                                                m.category.Any(
                                                    c =>
                                                    c == player.Profession || c == "Relics"
                                                ) && m.skills.Any(
                                                    s =>
                                                    s.id == skill.Id
                                                )
                                            )
                                        )
                                        // Sum the skills multiplying by matching skill ID coefficient.
                                        .Sum(
                                            skill =>
                                            skill.ConnectedHits * mapping.FirstOrDefault(
                                                m =>
                                                m.category.Any(
                                                    c =>
                                                    c == player.Profession || c == "Relics"
                                                ) && m.skills.Any(
                                                    s =>
                                                    s.id == skill.Id
                                                )
                                            ).skills.FirstOrDefault(
                                                s =>
                                                s.id == skill.Id
                                            ).coefficient
                                        )
                                    )
                                )
                                .Take(webhook.MaxPlayers)
                                .ToArray();

                            var ccField = CreateRankField("CC Summary", ccStats, p =>
                                (int)Math.Round(p.TotalDamageDist.Sum
                                (
                                    attack => attack
                                    // Filter to only skills that match profession and skill ID.
                                    .Where(
                                        skill =>
                                        mapping.Any(
                                            m =>
                                            m.category.Any(
                                                c =>
                                                c == p.Profession || c == "Relics"
                                            ) &&
                                            m.skills.Any(
                                                s =>
                                                s.id == skill.Id
                                            )
                                        )
                                    )
                                    // Sum the skills multiplying by matching skill ID coefficient.
                                    .Sum(
                                        skill =>
                                        skill.ConnectedHits * mapping.FirstOrDefault(
                                            m =>
                                            m.category.Any(
                                                c =>
                                                c == p.Profession || c == "Relics"
                                            ) && m.skills.Any(
                                                s =>
                                                s.id == skill.Id
                                            )
                                        ).skills.FirstOrDefault(
                                            s =>
                                            s.id == skill.Id
                                        ).coefficient
                                    )
                                ))
                            );

                            if (ccField is not null) rankFields.Add(ccField);
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

                    await SendLogViaWebhooks
                    (
                        reportJSON.Encounter.Success ?? false,
                        reportJSON.Encounter.BossId,
                        false,
                        false,
                        bossData, players,
                        jsonContentWvWNone, jsonContentWvWSquad, jsonContentWvWPlayers, jsonContentWvWSquadAndPlayers
                    );
                }

                if (allWebhooks.Count > 0)
                {
                    mainLink.AddToText(">:> All active webhooks executed.");
                }
                return;
            }
            else // not WvW
            {
                var bossName = $"{reportJSON.Encounter.Boss}{(reportJSON.ChallengeMode ? " CM" : "")}";
                var successString = (reportJSON.Encounter.Success ?? false) ? ":white_check_mark:" : "❌";
                var lastTarget = (reportJSON?.ExtraJson?.PossiblyLastTarget is not null) ? $"\n{reportJSON.ExtraJson.PossiblyLastTarget.Name} ({Math.Round(100 - reportJSON.ExtraJson.PossiblyLastTarget.HealthPercentBurned, 2)}%)" : "";
                var extraJSON = (reportJSON.ExtraJson is null) ? "" : $"Recorded by: {reportJSON.ExtraJson.RecordedByAccountName}\nDuration: {reportJSON.ExtraJson.Duration}{lastTarget}\nElite Insights version: {reportJSON.ExtraJson.EliteInsightsVersion}\n";
                var icon = "";
                var bossData = Bosses.GetBossDataFromId(reportJSON.Encounter.BossId);
                if (bossData is not null)
                {
                    bossName = bossData.FightName(reportJSON);
                    icon = bossData.Icon;
                }
                var colour = (reportJSON.Encounter.Success ?? false) ? 32768 : 16711680;
                var discordContentEmbedThumbnail = new DiscordApiJsonContentEmbedThumbnail()
                {
                    Url = icon,
                };
                var timestampDateTime = DateTime.UtcNow;
                if (reportJSON.ExtraJson is not null)
                {
                    timestampDateTime = reportJSON.ExtraJson.TimeStart;
                }
                var timestamp = timestampDateTime.ToString("o");
                var discordContentEmbed = new DiscordApiJsonContentEmbed()
                {
                    Title = bossName,
                    Url = reportJSON.ConfigAwarePermalink,
                    Description = $"{extraJSON}Result: {successString}\narcdps version: {reportJSON.Evtc.Type}{reportJSON.Evtc.Version}",
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
                if (reportJSON.Players.Values.Count <= 10)
                {
                    if (reportJSON.ExtraJson is null)
                    {
                        // player list
                        var playerNames = new TextTable(2, tableStyle, tableBorders);
                        playerNames.SetColumnWidthRange(0, 21, 21);
                        playerNames.SetColumnWidthRange(1, 20, 20);
                        playerNames.AddCell("Character");
                        playerNames.AddCell("Account name");
                        foreach (var player in reportJSON.Players.Values)
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
                        foreach (var player in reportJSON.ExtraJson.Players.Where(x => !x.FriendlyNpc).OrderBy(x => x.Name))
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
                        var numberOfRealTargers = reportJSON.ExtraJson.Targets
                            .Count(x => !x.IsFake);
                        // damage summary
                        var targetDps = reportJSON.ExtraJson.GetPlayerTargetDPS();
                        var damageStats = reportJSON.ExtraJson.Players
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

                var discordContent = new DiscordApiJsonContent()
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

                await SendLogViaWebhooks(reportJSON.Encounter.Success ?? false,
                    reportJSON.Encounter.BossId,
                    reportJSON.ChallengeMode,
                    reportJSON.LegendaryChallengeMode,
                    bossData, players,
                    jsonContentNone, jsonContentSquad, jsonContentPlayers, jsonContentSquadAndPlayers);

                if (allWebhooks.Count > 0)
                {
                    mainLink.AddToText(">:> All active webhooks executed.");
                }
            }
        }

        internal async Task SendLogViaWebhooks(bool success, int bossId, bool isCm, bool isLegendaryCm, BossData bossData, List<LogPlayer> players, string jsonContentNone, string jsonContentSquad, string jsonContentPlayers, string jsonContentSquadAndPlayers)
        {
            foreach (var key in allWebhooks.Keys)
            {
                var webhook = allWebhooks[key];
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
                    continue;
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
        }

        internal async Task ExecuteSessionWebhooksAsync(List<DpsReportJson> reportsJSON, LogSessionSettings logSessionSettings)
        {
            if (logSessionSettings.UseSelectedWebhooksInstead)
            {
                foreach (var webhook in logSessionSettings.SelectedWebhooks)
                {
                    var discordEmbeds = SessionTextConstructor.ConstructSessionEmbeds(reportsJSON.Where(x => webhook.Team.IsSatisfied(x.GetLogPlayers())).ToList(), logSessionSettings);
                    await SendDiscordMessageWebhooksAsync(webhook, discordEmbeds, logSessionSettings.ContentText);
                }
            }
            else
            {
                foreach (var webhook in allWebhooks.Values.Where(x => x.Active))
                {
                    var discordEmbeds = SessionTextConstructor.ConstructSessionEmbeds(reportsJSON.Where(x => webhook.Team.IsSatisfied(x.GetLogPlayers())).ToList(), logSessionSettings);
                    await SendDiscordMessageWebhooksAsync(webhook, discordEmbeds, logSessionSettings.ContentText);
                }
            }
            if (logSessionSettings.UseSelectedWebhooksInstead && logSessionSettings.SelectedWebhooks.Count > 0)
            {
                mainLink.AddToText(">:> All selected webhooks successfully executed with finished log session.");
            }
            else if (allWebhooks.Count > 0)
            {
                mainLink.AddToText(">:> All active webhooks successfully executed with finished log session.");
            }
        }

        private async Task SendDiscordMessageWebhooksAsync(DiscordWebhookData webhook, SessionTextConstructor.DiscordEmbeds discordEmbeds, string contentText)
        {
            var jsonContentSuccessFailure = JsonConvert.SerializeObject(new DiscordApiJsonContent()
            {
                Content = contentText,
                Embeds = discordEmbeds.SuccessFailure,
            });
            var jsonContentSuccess = JsonConvert.SerializeObject(new DiscordApiJsonContent()
            {
                Content = contentText,
                Embeds = discordEmbeds.Success,
            });
            var jsonContentFailure = JsonConvert.SerializeObject(new DiscordApiJsonContent()
            {
                Content = contentText,
                Embeds = discordEmbeds.Failure,
            });
            try
            {
                string jsonContent;
                if (webhook.SuccessFailToggle.Equals(DiscordWebhookDataSuccessToggle.OnSuccessAndFailure))
                {
                    jsonContent = jsonContentSuccessFailure;
                }
                else if (webhook.SuccessFailToggle.Equals(DiscordWebhookDataSuccessToggle.OnSuccessOnly))
                {
                    jsonContent = jsonContentSuccess;
                }
                else
                {
                    jsonContent = jsonContentFailure;
                }

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
            if (int.TryParse(selected.Name, out var reservedId))
            {
                listViewDiscordWebhooks.Items.RemoveByKey(reservedId.ToString());
                allWebhooks.Remove(reservedId);
            }
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

        private void ContextMenuStripInteract_Opening(object sender, System.ComponentModel.CancelEventArgs e)
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
}
