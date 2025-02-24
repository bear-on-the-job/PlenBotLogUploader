using Hardstuck.Http;
using Newtonsoft.Json;
using PlenBotLogUploader.Teams;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PlenBotLogUploader.DiscordApi.DiscordWebhookData;

namespace PlenBotLogUploader.DiscordApi;

[JsonObject(MemberSerialization.OptIn)]
public sealed class DiscordWebhookData
{
    private Team team;
    /// <summary>
    ///     Indicates whether the webhook is currently active
    /// </summary>
    [JsonProperty("isActive")]
    internal bool Active { get; set; }

    /// <summary>
    ///     Name of the webhook
    /// </summary>
    [JsonProperty("name")]
    internal string Name { get; set; }

    /// <summary>
    ///     URL of the webhook
    /// </summary>
    [JsonProperty("url")]
    internal string Url { get; set; }

    /// <summary>
    ///     Indicates whether the webhook is executed only if the encounter is a success
    /// </summary>
    [JsonProperty("successFailToggle")]
    internal DiscordWebhookDataSuccessToggle SuccessFailToggle { get; set; } = DiscordWebhookDataSuccessToggle.OnSuccessAndFailure;

    /// <summary>
    ///     Indicates what type of summary is shown on the webhook
    /// </summary>
    [JsonProperty("summaryType")]
    internal DiscordWebhookDataLogSummaryType SummaryType { get; set; } = DiscordWebhookDataLogSummaryType.SquadAndPlayers;

    /// <summary>
    ///     A list containing boss ids which are omitted to be posted via webhook
    /// </summary>
    [JsonProperty("disabledBosses")]
    internal int[] BossesDisable { get; set; } = [];

    [JsonProperty("allowUnknownBossIds")]
    internal bool AllowUnknownBossIds { get; set; }

    [JsonProperty("teamId")]
    internal int TeamId { get; set; }

    //BEAR 
    [JsonProperty("classEmojis")]
    internal List<(string className, string emojiCode)> ClassEmojis { get; set; } = new List<(string className, string emojiCode)> { };
    //BEAR 
    [JsonProperty("includePreventionSummary")]
    internal bool IncludePreventionSummary { get; set; } = true;
    //BEAR 
    [JsonProperty("includeDamageSummary")]
    internal bool IncludeDamageSummary { get; set; } = true;
    //BEAR 
    [JsonProperty("includeDownsContributionSummary")]
    internal bool IncludeDownsContributionSummary { get; set; } = true;
    //BEAR 
    [JsonProperty("includeHealingSummary")]
    internal bool IncludeHealingSummary { get; set; } = false;
    //BEAR 
    [JsonProperty("includeBarrierSummary")]
    internal bool IncludeBarrierSummary { get; set; } = false;
    //BEAR 
    [JsonProperty("includeCleansingSummary")]
    internal bool IncludeCleansingSummary { get; set; } = false;
    //BEAR 
    [JsonProperty("includeStripSummary")]
    internal bool IncludeStripSummary { get; set; } = false;
    //BEAR 
    [JsonProperty("includeCCSummary")]
    internal bool IncludeCCSummary { get; set; } = false;
    //BEAR 
    [JsonProperty("includeStabSummary")]
    internal bool IncludeStabilitySummary { get; set; } = false;
    //BEAR 
    [JsonProperty("adjustBarrier")]
    internal bool AdjustBarrier { get; set; } = false;
    //BEAR 
    [JsonProperty("combineBarrierHealing")]
    internal bool CombineHealingBarrier { get; set; } = false;
    //BEAR 
    [JsonProperty("maxPlayers")]
    internal int MaxPlayers { get; set; } = 10;
    //BEAR 
    [JsonProperty("showDpsColumn")]
    internal bool ShowDpsColumn { get; set; } = true;
    //BEAR 
    [JsonProperty("showOpponentIcons")]
    internal bool ShowClassIcons { get; set; } = true;
    //BEAR 
    [JsonProperty("showFightAwards")]
    internal bool ShowFightAwards { get; set; } = true;

    [JsonProperty("includeNormalLogs")]
    internal bool IncludeNormalLogs { get; set; } = true;

    [JsonProperty("includeChallengeModeLogs")]
    internal bool IncludeChallengeModeLogs { get; set; } = true;

    [JsonProperty("includeLegendaryChallengeModeLogs")]
    internal bool IncludeLegendaryChallengeModeLogs { get; set; } = true;

    /// <summary>
    ///     A selected webhook team, with which the webhook should evaluate itself
    /// </summary>
    internal Team Team
    {
        get
        {
            if (this.team is null && Teams.Teams.All.TryGetValue(TeamId, out var team))
            {
                this.team = team;
            }
            return this.team;
        }
        set
        {
            team = value;
            TeamId = value.Id;
        }
    }

    /// <summary>
    ///     Tests whether webhook is valid
    /// </summary>
    /// <param name="httpController">HttpClientController class used for using http connection</param>
    /// <returns>True if webhook is valid, false otherwise</returns>
    internal async Task<bool> TestWebhookAsync(HttpClientController httpController)
    {
        try
        {
            var response = await httpController.DownloadFileToStringAsync(Url);
            var pingTest = JsonConvert.DeserializeObject<DiscordApiJsonWebhookResponse>(response);
            return pingTest?.Success ?? false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     True if boss is enabled for webhook broadcast, false otherwise; default: true
    /// </summary>
    /// <param name="bossId">Queried boss ID</param>
    internal bool IsBossEnabled(int bossId) => !BossesDisable.Contains(bossId);

    internal static Dictionary<int, DiscordWebhookData> FromJsonString(string jsonString)
    {
        var webhookId = 1;

        var parsedData = JsonConvert.DeserializeObject<IEnumerable<DiscordWebhookData>>(jsonString)
                         ?? throw new JsonException("Could not parse json to WebhookData");

        return parsedData.Select(x => (Key: webhookId++, DiscordWebhookData: x))
            .ToDictionary(x => x.Key, x => x.DiscordWebhookData);
    }

    /// <summary>
    /// Resets the class emoji list to a default set
    /// </summary>
    internal void ResetEmojis()
    {
        // All the emojis here are from Bear on the job's server, but should be accessible
        // accross discord because of the numeric unique IDs.
        ClassEmojis =
        [
            ("ELEMENTALIST","<:ele:1225280046055428177>"),
                ("TEMPEST","<:tem:1225297472709070878>"),
                ("WEAVER","<:wea:1225297478367187019>"),
                ("CATALYST","<:cat:1225280016099835915>"),
                ("ENGINEER","<:eng:1225280046923645052>"),
                ("SCRAPPER","<:scr:1225297432112271369>"),
                ("HOLOSMITH","<:hol:1225292403833835541>"),
                ("MECHANIST","<:mec:1225292404869955616>"),
                ("GUARDIAN","<:gua:1225280048844771439>"),
                ("DRAGONHUNTER","<:dra:1225280020277235833>"),
                ("FIREBRAND","<:fir:1225280047838007358>"),
                ("WILLBENDER","<:wil:1225292398087897250>"),
                ("MESMER","<:mes:1225292405759021087>"),
                ("CHRONOMANCER","<:chr:1225280017085501451>"),
                ("MIRAGE","<:mir:1225297354018783262>"),
                ("VIRTUOSO","<:vir:1225297476492198020>"),
                ("NECROMANCER","<:nec:1225292411228651621>"),
                ("REAPER","<:rea:1225292387610398820>"),
                ("SCOURGE","<:sco:1225297431378268190>"),
                ("HARBINGER","<:har:1225280049775775744>"),
                ("RANGER","<:ran:1225297306866286652>"),
                ("DRUID","<:dru:1225280045329678506>"),
                ("SOULBEAST","<:sou:1225297433110646815>"),
                ("UNTAMED","<:unt:1225297474693107802>"),
                ("REVENANT","<:rev:1225297430644527114>"),
                ("HERALD","<:her:1225292402424545360>"),
                ("RENEGADE","<:ren:1225292392056492093>"),
                ("VINDICATOR","<:vin:1225297475171254324>"),
                ("THIEF","<:thi:1225297473480949760>"),
                ("DAREDEVIL","<:dar:1225280017957912718>"),
                ("DEADEYE","<:dea:1225280019115544586>"),
                ("SPECTER","<:spec:1225297434876313630>"),
                ("WARRIOR","<:war:1225297477478121504>"),
                ("BERSERKER","<:ber:1225280014237564929>"),
                ("SPELLBREAKER","<:spe:1225297434213744750>"),
                ("BLADESWORN","<:bla:1225280015181156516>")
        ];
    }

    /// <summary>
    /// URL of the google sheet for custom awards
    /// </summary>
    [JsonProperty("googleSheetsUrl")]
    internal string GoogleSheetsUrl { get; set; }

    /// <summary>
    /// Data model for mapping team colors to IDs
    /// </summary>
    public class TeamColorId
    {
        /// <summary>
        /// The color of the team (ex: Red, Blue, Green)
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }
        /// <summary>
        /// The IDs associated with that team
        /// </summary>
        [JsonProperty("ids")]
        public List<string> Ids { get; set; }
    }

    /// <summary>
    /// List of team color mappings
    /// </summary>
    [JsonProperty("teamColorIds")]
    public List<TeamColorId> TeamColorIds { get; set; } =
    [
        new ()
        {
            Color = "Red",
            Ids = ["705","706"]
        },
        new ()
        {
            Color = "Blue",
            Ids = ["432"]
        },
        new ()
        {
            Color = "Green",
            Ids = ["2752","2763"]
        }
    ];
}

public static class TeamColorIdExtensions
{
    public static string ToText(this TeamColorId teamColorId) => string.Join(",", teamColorId.Ids.Where(i => !string.IsNullOrEmpty(i)));
    public static List<string> ToList(this string text) => text.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
}
