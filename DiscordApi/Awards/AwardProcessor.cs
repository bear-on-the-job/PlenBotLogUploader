using PlenBotLogUploader.DpsReport.ExtraJson;
using System;
using System.Collections.Generic;

namespace PlenBotLogUploader.DiscordApi.Awards
{
    internal class AwardProcessor
    {
        /// <summary>
        /// Primary key, description of the award
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Determines if a player is qualified to win the award
        /// </summary>
        public Func<Player, IEnumerable<Target>, bool> Qualifier { get; set; }
        /// <summary>
        /// After qualifying, determines the ranking of the player compared to other qualifiers.
        /// Higher number result means higher ranking.
        /// </summary>
        public Func<Player, IEnumerable<Target>, float> Ranking { get; set; }
    }
}
