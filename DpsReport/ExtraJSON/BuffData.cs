using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    internal sealed class BuffData
    {
        [JsonProperty("uptime")]
        internal float Uptime { get; set; }
        
        [JsonProperty("generated")]
        // Needs to be a JObject, because the members are dynamically named, based on player names...
        internal JObject Generated { get; set; }
    }
}
