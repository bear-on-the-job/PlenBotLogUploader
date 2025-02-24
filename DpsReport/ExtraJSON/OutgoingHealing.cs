using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson;

public class OutgoingHealing
{
    [JsonProperty("healing")]
    public long Healing { get; set; }

    [JsonProperty("hps")]
    public long HealingPerSecond { get; set; }
}
