using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class DpsTarget
    {
        [JsonProperty("dps")]
        public int Dps { get; set; }

        [JsonProperty("damage")]
        public int Damage { get; set; }
    }
}
