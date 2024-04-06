using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    internal sealed class BuffUptimes
    {
        [JsonProperty("id")]
        internal int Id{ get; set; }

        [JsonProperty("buffData")]
        internal BuffData[] BuffData { get; set; }
    }
}
