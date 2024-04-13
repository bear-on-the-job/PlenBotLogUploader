using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class StatsAll
    {
        [JsonProperty("killed")]
        public int Killed { get; set; }

        [JsonProperty("downed")]
        public int Downed { get; set; }

        [JsonProperty("flankingRate")]
        public float FlankingRate { get; set; }

        [JsonProperty("criticalRate")]
        public float CriticalRate { get; set; }

        [JsonProperty("distToCom")]
        public float DistToCom { get; set; }

        [JsonProperty("swapCount")]
        public float SwapCount { get; set; }

        [JsonProperty("interrupts")]
        public float Interrupts { get; set; }

        [JsonProperty("connectedDirectDamageCount")]
        public float ConnectedDirectDamageCount { get; set; }

        [JsonProperty("glanceRate")]
        public float GlanceRate { get; set; }

        [JsonProperty("missed")]
        public float Missed { get; set; }
    }
}
