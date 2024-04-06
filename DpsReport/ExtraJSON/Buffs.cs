using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    internal sealed class Buffs
    {
        [JsonProperty("id")]
        internal int Id { get; set; }

        [JsonProperty("buffData")]
        internal BuffData[] BuffData { get; set; }
    }
}
