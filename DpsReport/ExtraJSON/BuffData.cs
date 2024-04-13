using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class BuffData
    {
        [JsonProperty("uptime")]
        public float Uptime { get; set; }

        [JsonProperty("presence")]
        public float Presence { get; set; }

        [JsonProperty("generated")]
        // Needs to be a JObject, because the members are dynamically named, based on player names...
        public JObject Generated { get; set; }
    }
}
