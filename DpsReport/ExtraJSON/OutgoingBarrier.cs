using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public class OutgoingBarrier
    {
        [JsonProperty("barrier")]
        public long Barrier { get; set; }

        [JsonProperty("bps")]
        public long BarrierPerSecond { get; set; }
    }
}
