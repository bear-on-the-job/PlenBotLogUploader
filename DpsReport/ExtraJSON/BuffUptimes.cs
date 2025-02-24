using Newtonsoft.Json;
using System.Collections.Generic;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class BuffUptimes
    {
        [JsonProperty("id")]
        public int Id{ get; set; }

        [JsonProperty("buffData")]
        public BuffData[] BuffData { get; set; }

        [JsonProperty("statesPerSource")]
        public Dictionary<string, List<int[]>> StatesPerSource { get; set; }
    }
}
