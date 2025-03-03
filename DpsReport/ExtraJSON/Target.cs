﻿using Newtonsoft.Json;

namespace PlenBotLogUploader.DpsReport.ExtraJson;

internal sealed class Target
{
    [JsonProperty("id")]
    internal int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("isFake")]
    public bool IsFake { get; set; }

    [JsonProperty("dpsAll")]
    public DpsAll[] DpsAll { get; set; }

    [JsonProperty("statsAll")]
    public StatsAll[] StatsAll { get; set; }

    [JsonProperty("defenses")]
    public Defenses[] Defenses { get; set; }

    [JsonProperty("totalHealth")]
    public long TotalHealth { get; set; }

    [JsonProperty("healthPercentBurned")]
    public double HealthPercentBurned { get; set; }

    [JsonProperty("enemyPlayer")]
    public bool EnemyPlayer { get; set; }

    [JsonProperty("teamId")]
    public int TeamId { get; set; }

    [JsonProperty("buffs")]
    public Buffs[] Buffs { get; set; }
}

