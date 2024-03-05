﻿using Newtonsoft.Json;
using PlenBotLogUploader.AppSettings;

namespace PlenBotLogUploader.DiscordApi
{
    /// <summary>
    /// Discord embedded rich content's footer
    /// </summary>
    internal sealed class DiscordApiJsonContentEmbedFooter
    {
        /// <summary>
        /// footer text
        /// </summary>
        [JsonProperty("text")]
        internal string Text { get; set; } = $"PlenBot Log Uploader r.{ApplicationSettings.Version}.Bear";

        /// <summary>
        /// url of the footer icon (only supports http(s) and attachments)
        /// </summary>
        [JsonProperty("icon_url")]
        internal string IconUrl { get; set; } = "https://plenbot.net/uploader/img/favicon.png";
    }
}
