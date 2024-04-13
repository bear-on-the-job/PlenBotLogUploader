using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class Buffs
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("buffData")]
        public BuffData[] BuffData { get; set; }
    }
}
