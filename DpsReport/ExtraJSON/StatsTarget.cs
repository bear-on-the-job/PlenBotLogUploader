using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson;

public sealed class StatsTarget
{
    [JsonProperty("killed")]
    public int Killed { get; set; }

    [JsonProperty("downed")]
    public int Downed { get; set; }

    [JsonProperty("downContribution")]
    public int DownContribution { get; set; }

    [JsonProperty("againstDownedCount")]
    public int AgainstDownedCount { get; set; }

    [JsonProperty("againstDownedDamage")]
    public int AgainstDownedDamage { get; set; }
}

