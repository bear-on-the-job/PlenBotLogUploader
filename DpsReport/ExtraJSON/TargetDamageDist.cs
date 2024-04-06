using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    internal sealed class TargetDamageDist
    {
        [JsonProperty("id")]
        internal int Id { get; set; }

        [JsonProperty("hits")]
        internal int Hits { get; set; }

        [JsonProperty("connectedHits")]
        internal int ConnectedHits { get; set; }

        [JsonProperty("flank")]
        internal int Flank { get; set; }

        [JsonProperty("totalDamage")]
        internal int TotalDamage { get; set; }
    }
}
