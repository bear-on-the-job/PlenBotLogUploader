using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson
{
    internal sealed class Defenses
    {
        [JsonProperty("damageTaken")]
        internal int DamageTaken { get; set; }

        [JsonProperty("downCount")]
        internal int DownCount { get; set; }

        [JsonProperty("deadCount")]
        internal int DeadCount { get; set; }

        [JsonProperty("damageBarrier")]
        internal int DamageBarrier { get; set; }

        [JsonProperty("downDuration")]
        internal float DownDuration { get; set; }

        [JsonProperty("blockedCount")]
        internal float BlockedCount { get; set; }

        [JsonProperty("evadedCount")]
        internal float EvadedCount { get; set; }

        [JsonProperty("missedCount")]
        internal float MissedCount { get; set; }

        [JsonProperty("dodgeCount")]
        internal float DodgeCount { get; set; }

        [JsonProperty("interruptedCount")]
        internal float InterruptedCount { get; set; }

        [JsonProperty("boonStrips")]
        internal float BoonStrips { get; set; }
    }
}
