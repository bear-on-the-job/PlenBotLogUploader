using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson;

public sealed class PlayerSupport
{
    [JsonProperty("condiCleanse")]
    internal int CondiCleanse { get; set; }

    [JsonProperty("condiCleanseSelf")]
    public int CondiCleanseSelf { get; set; }

    public int CondiCleanseTotal => CondiCleanse + CondiCleanseSelf;

    [JsonProperty("boonStrips")]
    public int BoonStrips { get; set; }

    [JsonProperty("resurrectTime")]
    public float ResurrectTime { get; set; }
}
