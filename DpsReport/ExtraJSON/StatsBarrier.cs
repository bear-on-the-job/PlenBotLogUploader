using Newtonsoft.Json;
using System;

namespace PlenBotLogUploader.DpsReport.ExtraJson;

public class StatsBarrier
{
    [JsonProperty("outgoingBarrier")]
    public OutgoingBarrier[] OutgoingBarrier { get; set; }

    public long TotalBarrier => (OutgoingBarrier?.Length ?? 0) > 0 ? OutgoingBarrier[0].Barrier : 0;

    [JsonProperty("outgoingBarrierAllies")]
    public OutgoingBarrier[][] OutgoingBarrierAllies { get; set; }

    public long TotalBarrierOnSquad
    {
        get
        {
            long result = 0;
            foreach (var squadMember in OutgoingBarrierAllies.AsSpan())
            {
                foreach (var squadMemberPhase in squadMember.AsSpan())
                {
                    result += squadMemberPhase.Barrier;
                }
            }
            return result;
        }
    }
}

