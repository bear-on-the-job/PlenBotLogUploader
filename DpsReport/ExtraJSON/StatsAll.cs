using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    internal sealed class StatsAll
    {
        [JsonProperty("killed")]
        internal int Killed { get; set; }

        [JsonProperty("downed")]
        internal int Downed { get; set; }

        [JsonProperty("flankingRate")]
        internal float FlankingRate { get; set; }

        [JsonProperty("criticalRate")]
        internal float CriticalRate { get; set; }

        [JsonProperty("distToCom")]
        internal float DistToCom { get; set; }

        [JsonProperty("swapCount")]
        internal float SwapCount { get; set; }

        [JsonProperty("interrupts")]
        internal float Interrupts { get; set; }

        [JsonProperty("connectedDirectDamageCount")]
        internal float ConnectedDirectDamageCount { get; set; }

        [JsonProperty("glanceRate")]
        internal float GlanceRate { get; set; }
    }
}
