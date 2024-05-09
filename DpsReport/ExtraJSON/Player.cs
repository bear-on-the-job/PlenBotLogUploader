using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class Player
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("group")]
        public int Group { get; set; }

        [JsonProperty("hasCommanderTag")]
        public bool IsCommander { get; set; }

        [JsonProperty("profession")]
        public string Profession { get; set; }

        public string ProfessionShort
        {
            get
            {
                if ("Soulbeast".Equals(Profession))
                {
                    return "Slb";
                }
                if ("Specter".Equals(Profession))
                {
                    return "Spec";
                }
                if ("Bladesworn".Equals(Profession))
                {
                    return "BS";
                }
                if ("Spellbreaker".Equals(Profession))
                {
                    return "Spb";
                }
                if ("Engineer".Equals(Profession))
                {
                    return "Engi";
                }
                if ("Dragonhunter".Equals(Profession))
                {
                    return "DH";
                }
                if ("Holosmith".Equals(Profession))
                {
                    return "Holo";
                }
                if ("Willbender".Equals(Profession))
                {
                    return "WB";
                }
                if ("Mechanist".Equals(Profession))
                {
                    return "Mech";
                }
                if ("Virtuoso".Equals(Profession))
                {
                    return "Virt";
                }
                if ("Firebrand".Equals(Profession))
                {
                    return "FB";
                }
                return (!string.IsNullOrWhiteSpace(Profession) && Profession.Length > 2) ? Profession[..3] : "";
            }
        }

        [JsonProperty("friendlyNPC")]
        public bool FriendlyNpc { get; set; }

        [JsonProperty("notInSquad")]
        public bool NotInSquad { get; set; }

        [JsonProperty("support")]
        public PlayerSupport[] Support { get; set; }

        /// <summary>
        /// Healing stats from extension
        /// </summary>
        //[JsonProperty("extHealingStats")]
        //public ExtHealingStats ExtHealingStats { get; set; }

        /// <summary>
        /// Barrier stats from extension
        /// </summary>
        //[JsonProperty("extBarrierStats")]
        //public ExtBarrierStats ExtBarrierStats { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dpsAll")]
        public DpsAll[] DpsAll { get; set; }

        [JsonProperty("dpsTargets")]
        public DpsTarget[][] DpsTargets { get; set; }

        [JsonProperty("statsTargets")]
        public StatsTarget[][] StatsTargets { get; set; }

        [JsonProperty("statsAll")]
        public StatsAll[] StatsAll { get; set; }

        [JsonProperty("defenses")]
        public Defenses[] Defenses { get; set; }

        [JsonProperty("extHealingStats")]
        public StatsHealing StatsHealing { get; set; }

        [JsonProperty("extBarrierStats")]
        public StatsBarrier StatsBarrier { get; set; }

        [JsonProperty("totalDamageDist")]
        public TotalDamageDist[][] TotalDamageDist { get; set; }

        [JsonProperty("buffUptimes")]
        public BuffUptimes[] BuffUptimes { get; set; }

        [JsonProperty("targetDamageDist")]
        public TargetDamageDist[][][] TargetDamageDist { get; set; }
    }
}
