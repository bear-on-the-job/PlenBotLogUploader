using Hardstuck.GuildWars2.BuildCodes.V2;
using Newtonsoft.Json;
using PlenBotLogUploader.AppSettings;
using PlenBotLogUploader.DiscordApi;
using PlenBotLogUploader.DpsReport;
using PlenBotLogUploader.DpsReport.ExtraJson;
using PlenBotLogUploader.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextTableFormatter;

namespace PlenBotLogUploader
{
    public partial class FormDiscordWebhooks : Form
    {
        private List<(List<string> professions, List<(int id, double coefficient)> skills)> mapping = new List<(List<string>, List<(int, double)>)>
        {
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
                    (50484, 1), //Malicious Tactical Strike
                    (63275, 1), //Shadowfall
                    (63220, 1), //Dawn's Repose
                    (1141,  1), //Skull Fear
                    (63249, 1), //Mind Shock
                    (13031, 1), //Pistol Whip
                    (13024, 1/4), //Choking Gas
                    (56880, 1), //Pitfall
                    (30077, 1), //Uppercut
                    (46335, 2), //Shadow Gust
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
                    (9093,  1), //Bane Signet
                    (9125,  1), //Hammer of Wisdom
                    (46170, 1), //Hammer of Wisdom
                    (30871, 1/9), //Light's Judgement
                    (30273, 1), //Dragon's Maw
                    (62549, 1), //Heel Crack
                    (62561, 1), //Heaven's Palm
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
                    (27917, 1), //Call to Anguish
                    (62878, 1), //Reaver's Rage
                    (41220, 1), //Darkrazor's Daring
                    (28406, 1), //Jade Winds
                    (31294, 1), //Jade Winds
                    (28075, 1), //Chaotic Release
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
                    (67179, 1), //Slam - ID seems incorrect?
                    (12476, 1), //Spike Trap
                    (63330, 1), //Thump
                    (12523, 1), //Counterattack Kick
                    (31321, 1), //Wing Buffet
                    (12511, 1), //Point-Blank Shot
                    (30448, 1), //Glyph of the Tides (non-celestial)
                    (12475, 2), //Hilt Bash
                    (12508, 1), //Concussion Shot
                    (12638, 1), //Path of Scars
                    (29558, 1), //Glyph of the Tides (celestial)
                    (12621, 1), //Call of the Wild
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
                }
            )

        };

        #region definitions
        // fields
        private readonly FormMain mainLink;
        private int webhookIdsKey;
        private readonly IDictionary<int, DiscordWebhookData> allWebhooks;
        private readonly CellStyle tableCellRightAlign = new(CellHorizontalAlignment.Right);
        private readonly CellStyle tableCellCenterAlign = new(CellHorizontalAlignment.Center);
        private readonly TableBordersStyle tableStyle = TableBordersStyle.HORIZONTAL_ONLY;
        private readonly TableVisibleBorders tableBorders = TableVisibleBorders.HEADER_ONLY;
        #endregion

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
                    Checked = webHook.Value.Active
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
                var extraJSON = (reportJSON.ExtraJson is null) ? "" : $"Recorded by: {reportJSON.ExtraJson.RecordedBy}\nDuration: {reportJSON.ExtraJson.Duration}\nElite Insights version: {reportJSON.ExtraJson.EliteInsightsVersion}";
                var icon = "";
                var bossData = Bosses.GetBossDataFromId(1);
                if (bossData is not null)
                {
                    icon = bossData.Icon;
                }
                const int colour = 16752238;
                var discordContentEmbedThumbnail = new DiscordApiJsonContentEmbedThumbnail()
                {
                    Url = icon
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
                    Description = $"{extraJSON}\narcdps version: {reportJSON.Evtc.Type}{reportJSON.Evtc.Version}",
                    Colour = colour,
                    TimeStamp = timestamp,
                    Thumbnail = discordContentEmbedThumbnail
                };

                foreach (var key in allWebhooks.Keys)
                {
                    var webhook = allWebhooks[key];

                    // BEAR
                    var damageField = new DiscordApiJsonContentEmbedField();
                    var healingField = new DiscordApiJsonContentEmbedField();
                    var barrierField = new DiscordApiJsonContentEmbedField();
                    var cleansesField = new DiscordApiJsonContentEmbedField();
                    var boonStripsField = new DiscordApiJsonContentEmbedField();
                    var ccField = new DiscordApiJsonContentEmbedField();

                    // fields
                    if (reportJSON.ExtraJson is not null)
                    {
                        // squad summary
                        var squadPlayers = reportJSON.ExtraJson.Players
                            .Count(x => !x.FriendlyNpc && !x.NotInSquad);
                        var squadDamage = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad)
                            .Select(x => x.DpsTargets.Sum(y => y.Sum(z => z.Damage)))
                            .Sum();
                        var squadDps = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad)
                            .Select(x => x.DpsTargets.Sum(y => y.Sum(z => z.Dps)))
                            .Sum();
                        var squadDowns = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad)
                            .Select(x => x.Defenses[0].DownCount)
                            .Sum();
                        var squadDeaths = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad)
                            .Select(x => x.Defenses[0].DeadCount)
                            .Sum();
                        var squadSummary = new TextTable(5, tableStyle, tableBorders);
                        squadSummary.SetColumnWidthRange(0, 3, 3);
                        squadSummary.SetColumnWidthRange(1, 10, 10);
                        squadSummary.SetColumnWidthRange(2, 10, 10);
                        squadSummary.SetColumnWidthRange(3, 8, 8);
                        squadSummary.SetColumnWidthRange(4, 8, 8);
                        squadSummary.AddCell("#", tableCellCenterAlign);
                        squadSummary.AddCell("DMG", tableCellCenterAlign);
                        squadSummary.AddCell("DPS", tableCellCenterAlign);
                        squadSummary.AddCell("Downs", tableCellCenterAlign);
                        squadSummary.AddCell("Deaths", tableCellCenterAlign);
                        squadSummary.AddCell($"{squadPlayers}", tableCellCenterAlign);
                        squadSummary.AddCell($"{squadDamage.ParseAsK()}", tableCellCenterAlign);
                        squadSummary.AddCell($"{squadDps.ParseAsK()}", tableCellCenterAlign);
                        squadSummary.AddCell($"{squadDowns}", tableCellCenterAlign);
                        squadSummary.AddCell($"{squadDeaths}", tableCellCenterAlign);
                        var squadField = new DiscordApiJsonContentEmbedField()
                        {
                            Name = "Squad summary:",
                            Value = $"```{squadSummary.Render()}```"
                        };
                        // enemy summary field
                        var enemyField = new DiscordApiJsonContentEmbedField()
                        {
                            Name = "Enemy summary:",
                            Value = "```Summary could not have been generated.\nToggle detailed WvW to enable this feature.```"
                        };

                        var enemyClasses = new List<string>();

                        if (reportJSON.ExtraJson.Targets.Length > 1)
                        {
                            var enemyPlayers = reportJSON.ExtraJson.Targets
                                .Length - 1;
                            var enemyDamage = reportJSON.ExtraJson.Targets
                                .Where(x => !x.IsFake)
                                .Select(x => x.DpsAll[0].Damage)
                                .Sum();
                            var enemyDps = reportJSON.ExtraJson.Targets
                                .Where(x => !x.IsFake)
                                .Select(x => x.DpsAll[0].Dps)
                                .Sum();
                            var enemyDowns = reportJSON.ExtraJson.Players
                                .Where(x => !x.FriendlyNpc && !x.NotInSquad)
                                .Select(x => x.StatsTargets.Select(y => y[0].Downed).Sum())
                                .Sum();
                            var enemyDeaths = reportJSON.ExtraJson.Players
                                .Where(x => !x.FriendlyNpc && !x.NotInSquad)
                                .Select(x => x.StatsTargets.Select(y => y[0].Killed).Sum())
                                .Sum();
                            var enemySummary = new TextTable(5, tableStyle, tableBorders);
                            enemySummary.SetColumnWidthRange(0, 3, 3);
                            enemySummary.SetColumnWidthRange(1, 10, 10);
                            enemySummary.SetColumnWidthRange(2, 10, 10);
                            enemySummary.SetColumnWidthRange(3, 8, 8);
                            enemySummary.SetColumnWidthRange(4, 8, 8);
                            enemySummary.AddCell("#", tableCellCenterAlign);
                            enemySummary.AddCell("DMG", tableCellCenterAlign);
                            enemySummary.AddCell("DPS", tableCellCenterAlign);
                            enemySummary.AddCell("Downs", tableCellCenterAlign);
                            enemySummary.AddCell("Deaths", tableCellCenterAlign);
                            enemySummary.AddCell($"{enemyPlayers}", tableCellCenterAlign);
                            enemySummary.AddCell($"{enemyDamage.ParseAsK()}", tableCellCenterAlign);
                            enemySummary.AddCell($"{enemyDps.ParseAsK()}", tableCellCenterAlign);
                            enemySummary.AddCell($"{enemyDowns}", tableCellCenterAlign);
                            enemySummary.AddCell($"{enemyDeaths}", tableCellCenterAlign);
                            enemyField = new DiscordApiJsonContentEmbedField()
                            {
                                Name = "Enemy summary:",
                                Value = $"```{enemySummary.Render()}```"
                            };

                            if (allWebhooks?.Any(w => w.Value?.ClassEmojis?.Any() == true) == true)
                            {
                                enemyClasses = reportJSON.ExtraJson.Targets
                                .Where(x => x.EnemyPlayer)
                                .GroupBy(x => x.Name.Split(' ').FirstOrDefault().ToUpper())
                                .OrderByDescending(x => x.Count())
                                .Select(x => $"{x.Count()} {{{x.Key}}}")
                                .ToList();
                            }
                        }
                        // damage summary
                        var damageStats = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.DpsTargets.Sum(y => y[0].Damage) > 0))
                            .OrderByDescending(x => x.DpsTargets.Sum(y => y[0].Damage))
                            .Take(webhook.MaxPlayers)
                            .ToArray();
                        var damageSummary = new TextTable(webhook.ShowDpsColumn ? 4 : 3, tableStyle, tableBorders);
                        
                        if (webhook.ShowDpsColumn)
                        {
                            damageSummary.SetColumnWidthRange(0, 3, 3);
                            damageSummary.SetColumnWidthRange(1, 25, 25);
                            damageSummary.SetColumnWidthRange(2, 7, 7);
                            damageSummary.SetColumnWidthRange(3, 6, 6);
                            damageSummary.AddCell("#", tableCellCenterAlign);
                            damageSummary.AddCell("Name");
                            damageSummary.AddCell("DMG", tableCellRightAlign);
                            damageSummary.AddCell("DPS", tableCellRightAlign);
                        }
                        else
                        {
                            damageSummary.SetColumnWidthRange(0, 3, 3);
                            damageSummary.SetColumnWidthRange(1, 27, 27);
                            damageSummary.SetColumnWidthRange(2, 12, 12);
                            damageSummary.AddCell("#", tableCellCenterAlign);
                            damageSummary.AddCell("Name");
                            damageSummary.AddCell("DMG", tableCellRightAlign);
                        }
                        
                        var rank = 0;
                        foreach (var player in damageStats)
                        {
                            rank++;
                            damageSummary.AddCell($"{rank}", tableCellCenterAlign);
                            damageSummary.AddCell($"{player.Name} ({player.ProfessionShort})");
                            damageSummary.AddCell($"{player.DpsTargets.Sum(y => y[0].Damage).ParseAsK()}", tableCellRightAlign);

                            if (webhook.ShowDpsColumn)
                            {
                                damageSummary.AddCell($"{player.DpsTargets.Sum(y => y[0].Dps).ParseAsK()}", tableCellRightAlign);
                            }
                        }

                        damageField.Name = "Damage summary:";
                        damageField.Value = $"```{damageSummary.Render()}```";

                        // cleanses summary
                        var cleansesStats = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.Support[0].CondiCleanseTotal > 0))
                            .OrderByDescending(x => x.Support[0].CondiCleanseTotal)
                            .Take(webhook.MaxPlayers)
                            .ToArray();
                        var cleansesSummary = new TextTable(3, tableStyle, tableBorders);
                        cleansesSummary.SetColumnWidthRange(0, 3, 3);
                        cleansesSummary.SetColumnWidthRange(1, 27, 27);
                        cleansesSummary.SetColumnWidthRange(2, 12, 12);
                        cleansesSummary.AddCell("#", tableCellCenterAlign);
                        cleansesSummary.AddCell("Name");
                        cleansesSummary.AddCell("Cleanses", tableCellRightAlign);
                        rank = 0;
                        foreach (var player in cleansesStats)
                        {
                            rank++;
                            cleansesSummary.AddCell($"{rank}", tableCellCenterAlign);
                            cleansesSummary.AddCell($"{player.Name} ({player.ProfessionShort})");
                            cleansesSummary.AddCell($"{player.Support[0].CondiCleanseTotal}", tableCellRightAlign);
                        }

                        cleansesField.Name = "Cleanses summary:";
                        cleansesField.Value = $"```{cleansesSummary.Render()}```";

                        // boon strips summary
                        var boonStripsStats = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.Support[0].BoonStrips > 0))
                            .OrderByDescending(x => x.Support[0].BoonStrips)
                            .Take(webhook.MaxPlayers)
                            .ToArray();
                        var boonStripsSummary = new TextTable(3, tableStyle, tableBorders);
                        boonStripsSummary.SetColumnWidthRange(0, 3, 3);
                        boonStripsSummary.SetColumnWidthRange(1, 27, 27);
                        boonStripsSummary.SetColumnWidthRange(2, 12, 12);
                        boonStripsSummary.AddCell("#", tableCellCenterAlign);
                        boonStripsSummary.AddCell("Name");
                        boonStripsSummary.AddCell("Strips", tableCellRightAlign);
                        rank = 0;
                        foreach (var player in boonStripsStats)
                        {
                            rank++;
                            boonStripsSummary.AddCell($"{rank}", tableCellCenterAlign);
                            boonStripsSummary.AddCell($"{player.Name} ({player.ProfessionShort})");
                            boonStripsSummary.AddCell($"{player.Support[0].BoonStrips}", tableCellRightAlign);
                        }

                        boonStripsField.Name = "Boon strips summary:";
                        boonStripsField.Value = $"```{boonStripsSummary.Render()}```";

                        // healing summary
                        var healingStats = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.ExtHealingStats?.OutgoingHealingAllies?.Any() == true))
                            .OrderByDescending(x => x.ExtHealingStats.OutgoingHealingAllies.Sum(p => p.FirstOrDefault()?.Healing ?? 0))
                            .Take(webhook.MaxPlayers)
                            .ToArray();
                        var healingSummary = new TextTable(3, tableStyle, tableBorders);
                        healingSummary.SetColumnWidthRange(0, 3, 3);
                        healingSummary.SetColumnWidthRange(1, 27, 27);
                        healingSummary.SetColumnWidthRange(2, 12, 12);
                        healingSummary.AddCell("#", tableCellCenterAlign);
                        healingSummary.AddCell("Name");
                        healingSummary.AddCell("Healing", tableCellRightAlign);
                        rank = 0;
                        foreach (var player in healingStats)
                        {
                            var healing = player.ExtHealingStats.OutgoingHealingAllies.Sum(p => p.FirstOrDefault()?.Healing ?? 0);

                            if (healing < 1) break;

                            rank++;
                            healingSummary.AddCell($"{rank}", tableCellCenterAlign);
                            healingSummary.AddCell($"{player.Name} ({player.ProfessionShort})");
                            healingSummary.AddCell($"{player.ExtHealingStats.OutgoingHealingAllies.Aggregate(0, (sum, next) => sum + (next.FirstOrDefault()?.Healing ?? 0), sum => sum)}", tableCellRightAlign);
                        }

                        healingField.Name = "Healing summary:";
                        healingField.Value = $"```{healingSummary.Render()}```";

                        // barrier summary
                        var barrierStats = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.ExtBarrierStats?.OutgoingBarrierAllies?.Any() == true))
                            .OrderByDescending(x => x.ExtBarrierStats.OutgoingBarrierAllies.Sum(x => x.FirstOrDefault()?.Barrier ?? 0))
                            .Take(webhook.MaxPlayers)
                            .ToArray();
                        var barrierSummary = new TextTable(3, tableStyle, tableBorders);
                        barrierSummary.SetColumnWidthRange(0, 3, 3);
                        barrierSummary.SetColumnWidthRange(1, 27, 27);
                        barrierSummary.SetColumnWidthRange(2, 12, 12);
                        barrierSummary.AddCell("#", tableCellCenterAlign);
                        barrierSummary.AddCell("Name");
                        barrierSummary.AddCell("Barrier", tableCellRightAlign);
                        rank = 0;
                        foreach (var player in barrierStats)
                        {   
                            var barrier = player.ExtBarrierStats.OutgoingBarrierAllies.Sum(x => x.FirstOrDefault()?.Barrier ?? 0);

                            if (webhook.AdjustBarrier)
                            {
                                var totalBarrier = reportJSON.ExtraJson.Players.Sum(p => p.ExtBarrierStats.OutgoingBarrierAllies.Sum(x => x.FirstOrDefault()?.Barrier ?? 0));
                                var absorbedBarrier = reportJSON.ExtraJson.Players.Sum(p => p.Defenses.FirstOrDefault()?.DamageBarrier ?? 0);
                                barrier = (int)((double)barrier * ((double)absorbedBarrier / (double)totalBarrier));
                            }

                            if (barrier < 1) break;

                            rank++;
                            barrierSummary.AddCell($"{rank}", tableCellCenterAlign);
                            barrierSummary.AddCell($"{player.Name} ({player.ProfessionShort})");
                            barrierSummary.AddCell($"{barrier}", tableCellRightAlign);
                        }

                        if (webhook.AdjustBarrier)
                        {
                            barrierField.Name = "Barrier summary (adjusted):";
                        }
                        else
                        {
                            barrierField.Name = "Barrier summary:";
                        }   

                        barrierField.Value = $"```{barrierSummary.Render()}```";

                        // CC summary
                        var ccStats = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc && !x.NotInSquad && (x.TotalDamageDist?.Any() == true))
                            .OrderByDescending
                            (
                                player => player.TotalDamageDist.Sum
                                (
                                    attack => attack
                                    // Filter to only skills that match profession and skill ID.
                                    .Where(skill => mapping.Any(m => m.professions.Any(p => p == player.Profession) && m.skills.Any(s => s.id == skill.Id)))
                                    // Sum the skills multiplying by matching skill ID coefficient.
                                    .Sum(skill => skill.ConnectedHits * mapping.FirstOrDefault(m => m.professions.Any(p => p == player.Profession)).skills.FirstOrDefault(s => s.id == skill.Id).coefficient)
                                )
                            )
                            .Take(webhook.MaxPlayers)
                            .ToArray();
                        var ccSummary = new TextTable(3, tableStyle, tableBorders);
                        ccSummary.SetColumnWidthRange(0, 3, 3);
                        ccSummary.SetColumnWidthRange(1, 27, 27);
                        ccSummary.SetColumnWidthRange(2, 12, 12);
                        ccSummary.AddCell("#", tableCellCenterAlign);
                        ccSummary.AddCell("Name");
                        ccSummary.AddCell("CC hits", tableCellRightAlign);
                        rank = 0;
                        foreach (var player in ccStats)
                        {
                            var hitCount = player.TotalDamageDist.Sum
                            (
                                attack => attack
                                // Filter to only skills that match profession and skill ID.
                                .Where(skill => mapping.Any(m => m.professions.Any(p => p == player.Profession) && m.skills.Any(s => s.id == skill.Id)))
                                // Sum the skills multiplying by matching skill ID coefficient.
                                .Sum(skill => skill.ConnectedHits * mapping.FirstOrDefault(m => m.professions.Any(p => p == player.Profession)).skills.FirstOrDefault(s => s.id == skill.Id).coefficient)
                            );

                            if (hitCount < 1) break;

                            rank++;
                            ccSummary.AddCell($"{rank}", tableCellCenterAlign);
                            ccSummary.AddCell($"{player.Name} ({player.ProfessionShort})");
                            ccSummary.AddCell($"{hitCount}", tableCellRightAlign);
                        }

                        ccField.Name = "CC summary:";
                        ccField.Value = $"```{ccSummary.Render()}```";

                        // add the fields
                        discordContentEmbed.Fields = new List<DiscordApiJsonContentEmbedField>();

                        discordContentEmbed.Fields.Add(squadField);
                        discordContentEmbed.Fields.Add(enemyField);

                        if (webhook.ShowOpponentIcons)
                        {
                            if (enemyClasses.Count > 0)
                            {
                                discordContentEmbed.Fields.Add(new DiscordApiJsonContentEmbedField
                                {
                                    Name = string.Join("    ", enemyClasses.Take(4)),
                                    Value = "",
                                    Inline = true
                                });
                            }
                            if (enemyClasses.Count > 4)
                            {
                                discordContentEmbed.Fields.Add(new DiscordApiJsonContentEmbedField
                                {
                                    Name = string.Join("    ", enemyClasses.Skip(4).Take(4)),
                                    Value = "",
                                    Inline = true
                                });
                            }
                            if (enemyClasses.Count > 8)
                            {
                                discordContentEmbed.Fields.Add(new DiscordApiJsonContentEmbedField
                                {
                                    Name = "",
                                    Value = "",
                                    Inline = false
                                });
                                discordContentEmbed.Fields.Add(new DiscordApiJsonContentEmbedField
                                {
                                    Name = $"  {string.Join("     ", enemyClasses.Skip(8).Take(4))}",
                                    Value = "",
                                    Inline = true
                                });
                            }
                            if (enemyClasses.Count > 12)
                            {
                                discordContentEmbed.Fields.Add(new DiscordApiJsonContentEmbedField
                                {
                                    Name = string.Join("    ", enemyClasses.Skip(12).Take(4)),
                                    Value = "",
                                    Inline = true
                                });
                            }
                        }
                    }

                    if (!webhook.Active
                        || (webhook.SuccessFailToggle.Equals(DiscordWebhookDataSuccessToggle.OnSuccessOnly) && !(reportJSON.Encounter.Success ?? false))
                        || (webhook.SuccessFailToggle.Equals(DiscordWebhookDataSuccessToggle.OnFailOnly) && (reportJSON.Encounter.Success ?? false))
                        || (webhook.BossesDisable.Contains(reportJSON.Encounter.BossId))
                        || (!webhook.Team.IsSatisfied(players)))
                    {
                        continue;
                    }
                    try
                    {
                        // BEAR
                        if (webhook.IncludeDamageSummary) discordContentEmbed.Fields.Add(damageField);
                        if (webhook.IncludeHealingSummary) discordContentEmbed.Fields.Add(healingField);
                        if (webhook.IncludeBarrierSummary) discordContentEmbed.Fields.Add(barrierField);
                        if (webhook.IncludeCleansingSummary) discordContentEmbed.Fields.Add(cleansesField);
                        if (webhook.IncludeStripSummary) discordContentEmbed.Fields.Add(boonStripsField);
                        if (webhook.IncludeCCSummary) discordContentEmbed.Fields.Add(ccField);

                        // post to discord
                        var discordContentWvW = new DiscordApiJsonContent()
                        {
                            Embeds = new List<DiscordApiJsonContentEmbed>() { discordContentEmbed }
                        };
                        var jsonContentWvW = JsonConvert.SerializeObject(discordContentWvW);

                        foreach (var (className, emojiCode) in webhook.ClassEmojis)
                        {
                            jsonContentWvW = jsonContentWvW.Replace($"{{{className}}}", emojiCode);
                        }
                        var uri = new Uri(webhook.Url);
                        using var content = new StringContent(jsonContentWvW, Encoding.UTF8, "application/json");
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
                var extraJSON = (reportJSON.ExtraJson is null) ? "" : $"Recorded by: {reportJSON.ExtraJson.RecordedBy}\nDuration: {reportJSON.ExtraJson.Duration}\nElite Insights version: {reportJSON.ExtraJson.EliteInsightsVersion}\n";
                var icon = "";
                var bossData = Bosses.GetBossDataFromId(reportJSON.Encounter.BossId);
                if (bossData is not null)
                {
                    bossName = bossData.Name + (reportJSON.ChallengeMode ? " CM" : "");
                    icon = bossData.Icon;
                }
                var colour = (reportJSON.Encounter.Success ?? false) ? 32768 : 16711680;
                var discordContentEmbedThumbnail = new DiscordApiJsonContentEmbedThumbnail()
                {
                    Url = icon
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
                    Thumbnail = discordContentEmbedThumbnail
                };
                var discordContentWithoutPlayers = new DiscordApiJsonContent()
                {
                    Embeds = new List<DiscordApiJsonContentEmbed>() { discordContentEmbed }
                };
                var discordContentEmbedForPlayers = new DiscordApiJsonContentEmbed()
                {
                    Title = bossName,
                    Url = reportJSON.ConfigAwarePermalink,
                    Description = $"{extraJSON}Result: {successString}\narcdps version: {reportJSON.Evtc.Type}{reportJSON.Evtc.Version}",
                    Colour = colour,
                    TimeStamp = timestamp,
                    Thumbnail = discordContentEmbedThumbnail
                };
                if (reportJSON.Players.Values.Count <= 10)
                {
                    var fields = new List<DiscordApiJsonContentEmbedField>();
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
                        fields.Add(new DiscordApiJsonContentEmbedField()
                        {
                            Name = "Players in squad/group:",
                            Value = $"```{playerNames.Render()}```"
                        });
                    }
                    else
                    {
                        // player list
                        var playerNames = new TextTable(2, tableStyle, tableBorders);
                        playerNames.SetColumnWidthRange(0, 21, 21);
                        playerNames.SetColumnWidthRange(1, 20, 20);
                        playerNames.AddCell("Character");
                        playerNames.AddCell("Account name");
                        foreach (var player in reportJSON.ExtraJson.Players.Where(x => !x.FriendlyNpc).OrderBy(x => x.Name))
                        {
                            playerNames.AddCell($"{player.Name}");
                            playerNames.AddCell($"{player.Account}");
                        }
                        fields.Add(new DiscordApiJsonContentEmbedField()
                        {
                            Name = "Players in squad/group:",
                            Value = $"```{playerNames.Render()}```"
                        });
                        var numberOfRealTargers = reportJSON.ExtraJson.Targets
                            .Count(x => !x.IsFake);
                        // damage summary
                        var targetDps = reportJSON.ExtraJson.GetPlayerTargetDPS();
                        var damageStats = reportJSON.ExtraJson.Players
                            .Where(x => !x.FriendlyNpc)
                            .Select(x => new
                            {
                                Player = x,
                                DPS = (numberOfRealTargers > 0) ? targetDps[x] : x.DpsAll[0].Dps
                            })
                            .OrderByDescending(x => x.DPS)
                            .Take(10)
                            .ToArray();
                        var dpsTargetSummary = new TextTable(3, tableStyle, TableVisibleBorders.HEADER_AND_FOOTER);
                        dpsTargetSummary.SetColumnWidthRange(0, 5, 5);
                        dpsTargetSummary.SetColumnWidthRange(1, 27, 27);
                        dpsTargetSummary.SetColumnWidthRange(2, 8, 8);
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
                        fields.Add(new DiscordApiJsonContentEmbedField()
                        {
                            Name = "DPS target summary:",
                            Value = $"```{dpsTargetSummary.Render()}```"
                        });
                    }
                    discordContentEmbedForPlayers.Fields = fields;
                }
                var discordContentWithPlayers = new DiscordApiJsonContent()
                {
                    Embeds = new List<DiscordApiJsonContentEmbed>() { discordContentEmbedForPlayers }
                };
                var jsonContentWithoutPlayers = JsonConvert.SerializeObject(discordContentWithoutPlayers);
                var jsonContentWithPlayers = JsonConvert.SerializeObject(discordContentWithPlayers);
                foreach (var key in allWebhooks.Keys)
                {
                    var webhook = allWebhooks[key];
                    if (!webhook.Active
                        || (webhook.SuccessFailToggle.Equals(DiscordWebhookDataSuccessToggle.OnSuccessOnly) && !(reportJSON.Encounter.Success ?? false))
                        || (webhook.SuccessFailToggle.Equals(DiscordWebhookDataSuccessToggle.OnFailOnly) && (reportJSON.Encounter.Success ?? false))
                        || (webhook.BossesDisable.Contains(reportJSON.Encounter.BossId))
                        || (!webhook.AllowUnknownBossIds && (bossData is null))
                        || (!webhook.Team.IsSatisfied(players)))
                    {
                        continue;
                    }
                    try
                    {
                        if (webhook.ShowPlayers)
                        {
                            using var content = new StringContent(jsonContentWithPlayers, Encoding.UTF8, "application/json");
                            await mainLink.HttpClientController.PostAsync(webhook.Url, content);
                        }
                        else
                        {
                            using var content = new StringContent(jsonContentWithoutPlayers, Encoding.UTF8, "application/json");
                            await mainLink.HttpClientController.PostAsync(webhook.Url, content);
                        }
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
                if (allWebhooks.Count > 0)
                {
                    mainLink.AddToText(">:> All active webhooks executed.");
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
                Embeds = discordEmbeds.SuccessFailure
            });
            var jsonContentSuccess = JsonConvert.SerializeObject(new DiscordApiJsonContent()
            {
                Content = contentText,
                Embeds = discordEmbeds.Success
            });
            var jsonContentFailure = JsonConvert.SerializeObject(new DiscordApiJsonContent()
            {
                Content = contentText,
                Embeds = discordEmbeds.Failure
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
