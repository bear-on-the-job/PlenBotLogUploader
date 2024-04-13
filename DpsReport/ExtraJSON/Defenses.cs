using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    public sealed class Defenses
    {
        [JsonProperty("damageTaken")]
        public int DamageTaken { get; set; }

        [JsonProperty("downCount")]
        public int DownCount { get; set; }

        [JsonProperty("deadCount")]
        public int DeadCount { get; set; }

        [JsonProperty("damageBarrier")]
        public int DamageBarrier { get; set; }

        [JsonProperty("downDuration")]
        public float DownDuration { get; set; }

        [JsonProperty("blockedCount")]
        public float BlockedCount { get; set; }

        [JsonProperty("evadedCount")]
        public float EvadedCount { get; set; }

        [JsonProperty("missedCount")]
        public float MissedCount { get; set; }

        [JsonProperty("dodgeCount")]
        public float DodgeCount { get; set; }

        [JsonProperty("interruptedCount")]
        public float InterruptedCount { get; set; }

        [JsonProperty("boonStrips")]
        public float BoonStrips { get; set; }
    }
}
