namespace PlenBotLogUploader.DiscordApi.Awards
{
    internal class Award
    {
        /// <summary>
        /// Display values for the award
        /// </summary>
        public AwardDisplay Display { get; set; }
        /// <summary>
        /// Qualifier and rank processor for the award
        /// </summary>
        public AwardProcessor Processor { get; set; }
    }
}
