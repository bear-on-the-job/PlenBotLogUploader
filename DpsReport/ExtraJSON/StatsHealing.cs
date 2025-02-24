using Newtonsoft.Json;
using System;

namespace PlenBotLogUploader.DpsReport.ExtraJson;

public class StatsHealing
{
    [JsonProperty("outgoingHealing")]
    public OutgoingHealing[] OutgoingHealing { get; set; }

    public long TotalHealing => (OutgoingHealing?.Length ?? 0) > 0 ? OutgoingHealing[0].Healing : 0;

    [JsonProperty("outgoingHealingAllies")]
    public OutgoingHealing[][] OutgoingHealingAllies { get; set; }

    public long TotalHealingOnSquad
    {
        get
        {
            long result = 0;
            foreach (var squadMember in OutgoingHealingAllies.AsSpan())
            {
                foreach (var squadMemberPhase in squadMember.AsSpan())
                {
                    result += squadMemberPhase.Healing;
                }
            }
            return result;
        }
    }
}

