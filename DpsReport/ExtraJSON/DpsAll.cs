using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class DpsAll
    {
        [JsonProperty("dps")]
        public int Dps { get; set; }

        [JsonProperty("damage")]
        public int Damage { get; set; }
    }
}
