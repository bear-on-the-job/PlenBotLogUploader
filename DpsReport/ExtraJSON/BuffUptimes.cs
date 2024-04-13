using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class BuffUptimes
    {
        [JsonProperty("id")]
        public int Id{ get; set; }

        [JsonProperty("buffData")]
        public BuffData[] BuffData { get; set; }
    }
}
