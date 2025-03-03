﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlenBotLogUploader.DpsReport.ExtraJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlenBotLogUploader.DpsReport;

internal sealed class DpsReportJsonExtraJson
{
    [JsonProperty("eliteInsightsVersion")]
    internal string EliteInsightsVersion { get; set; }

    [JsonProperty("recordedBy")]
    internal string RecordedBy { get; set; }

    [JsonProperty("recordedAccountBy")]
    internal string RecordedByAccountName { get; set; }

    [JsonProperty("timeStartStd")]
    internal DateTime TimeStart { get; set; }

    [JsonProperty("timeEndStd")]
    internal DateTime TimeEnd { get; set; }

    [JsonProperty("duration")]
    internal string Duration { get; set; }

    [JsonProperty("durationMs")]
    internal ulong DurationMs { get; set; }

    [JsonProperty("success")]
    internal bool Succcess { get; set; }

    [JsonProperty("triggerID")]
    internal int TriggerId { get; set; }

    [JsonProperty("fightName")]
    internal string FightName { get; set; }

    [JsonProperty("gw2Build")]
    internal ulong GameBuild { get; set; }

    [JsonProperty("fightIcon")]
    internal string FightIcon { get; set; }

    [JsonProperty("isCM")]
    internal bool IsCm { get; set; }

    [JsonProperty("isLegendaryCM")]
    internal bool IsLegendaryCm { get; set; }

    [JsonProperty("targets")]
    internal Target[] Targets { get; set; }

    [JsonProperty("players")]
    internal Player[] Players { get; set; }

    [JsonProperty("logErrors")]
    internal string[] LogErrors { get; set; }

    internal Target PossiblyLastTarget
    {
        get
        {
            return TriggerId is (int)BossIds.Cerus or (int)BossIds.Decima or (int)BossIds.Ura ? TargetsByTotalHealth.FirstOrDefault() : TargetsByTotalHealth.FirstOrDefault(x => x.HealthPercentBurned <= 98.6);
        }
    }

    [JsonProperty("skillMap")]
    internal JObject SkillMap { get; set; }

    [JsonProperty("buffMap")]
    public JObject BuffMap { get; set; }

    public List<Skill> SkillList => SkillMap
        .Properties()
        .Select
        (
            p =>
            {
                var skill = p.Value.ToObject<Skill>();
                skill.Id = int.TryParse(Regex.Match(p.Name, @"\d+")?.Value ?? string.Empty, out int id) ? id : 0;
                return skill;
            }
        )
        .ToList();

    public List<Buff> BuffList => BuffMap
        .Properties()
        .Select
        (
            p =>
            {
                var buff = p.Value.ToObject<Buff>();
                buff.Id = int.TryParse(Regex.Match(p.Name, @"\d+")?.Value ?? string.Empty, out int id) ? id : 0;
                return buff;
            }
        )
        .ToList();    

    private IOrderedEnumerable<Target> TargetsByTotalHealth => Targets.OrderByDescending(x => x.TotalHealth);

    public Dictionary<Player, int> GetPlayerTargetDps()
    {
        var dict = new Dictionary<Player, int>();
        foreach (var player in Players.AsSpan())
        {
            var damage = player.DpsTargets
                .Select(x => x[0].Dps)
                .Sum();
            dict.Add(player, damage);
        }
        return dict;
    }
}
