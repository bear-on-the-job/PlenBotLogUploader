using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class TargetDamageDist
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("hits")]
        public int Hits { get; set; }

        [JsonProperty("connectedHits")]
        public int ConnectedHits { get; set; }

        [JsonProperty("flank")]
        public int Flank { get; set; }

        [JsonProperty("crit")]
        public int Crit { get; set; }

        [JsonProperty("glance")]
        public int Glance { get; set; }

        [JsonProperty("totalDamage")]
        public int TotalDamage { get; set; }

        [JsonProperty("missed")]
        public int Missed { get; set; }

        [JsonProperty("interrupted")]
        public int Interrupted { get; set; }

        [JsonProperty("evaded")]
        public int Evaded { get; set; }

        [JsonProperty("blocked")]
        public int Blocked { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }
}
