using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    internal sealed class TotalDamageDist
    {
        [JsonProperty("id")]
        internal int Id { get; set; }

        [JsonProperty("hits")]
        internal int Hits { get; set; }

        [JsonProperty("connectedHits")]
        internal int ConnectedHits { get; set; }

        [JsonProperty("flank")]
        internal int Flank { get; set; }
    }
}
