﻿using Newtonsoft.Json;

namespace PlenBotLogUploader.Aleeva;

internal sealed class AleevaServer
{
    [JsonProperty("id")]
    internal string Id { get; set; }

    [JsonProperty("name")]
    internal string Name { get; set; }

    [JsonProperty("avatar")]
    internal string Avatar { get; set; }

    public override string ToString() => $"{Name} ({Id})";
}
