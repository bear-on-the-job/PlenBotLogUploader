using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class TotalDamageDist
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("hits")]
        public int Hits { get; set; }

        [JsonProperty("connectedHits")]
        public int ConnectedHits { get; set; }

        [JsonProperty("flank")]
        public int Flank { get; set; }
    }
}
