using PlenBotLogUploader.DpsReport.ExtraJson;
using System.Collections.Generic;
using System.Linq;

namespace PlenBotLogUploader.DiscordApi.Awards
{
    internal class Award
    {
        /// <summary>
        /// Primary key, description of the award
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Name displayed in the discord report
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Icon to display
        /// </summary>
        public string Emoji { get; set; }
        /// <summary>
        /// Rarity of the award (determines color)
        /// One of:
        ///  "Common"
        ///  "Rare"
        ///  "Epic"
        ///  "Legendary"
        /// </summary>
        public string Rarity { get; set; }
        /// <summary>
        /// Whether the award should be displayed or not
        /// </summary>
        public string Active { get; set; }
        /// <summary>
        /// Whether the award should be displayed or not
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Whether the award should be displayed or not
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// Whether the award should be displayed or not
        /// </summary>
        public float Qualifier { get; set; }
        /// <summary>
        /// Whether the award should be displayed or not
        /// </summary>
        public string[] AbilityNames { get; set; }

        /// <summary>
        /// List of skills extracted from the JSON
        /// </summary>
        public IEnumerable<Skill> Skills { get; set; }
        /// <summary>
        /// List of buffs extracted from the JSON
        /// </summary>
        public IEnumerable<Buff> Buffs { get; set; }

        public bool Qualify(Player player, IEnumerable<Target> targets)
        {
            var result = Category switch
            {
                "Gameplay"      => QualifyGameplay(player, targets),
                "Offensive"     => QualifyOffensive(player, targets),
                "Defensive"     => QualifyDefensive(player, targets),
                "Support"       => QualifySupport(player, targets),
                "Skills"        => QualifySkills(player, targets),
                "Buffs"         => QualifyBuffs(player, targets),
                "DistanceToTag" => !player.IsCommander,
                _               => false
            }; 

            return result;
        }

        public float Rank(Player player, IEnumerable<Target> targets)
        {
            var result = Category switch
            {
                "Gameplay"      => RankGameplay(player, targets),
                "Offensive"     => RankOffensive(player, targets),
                "Defensive"     => RankDefensive(player, targets),
                "Support"       => RankSupport(player, targets),
                "Skills"        => RankSkills(player, targets),
                "Buffs"         => RankBuffs(player, targets),
                "DistanceToTag" => -player.StatsAll[0].DistToCom,
                _               => 0f
            };

            return result;
        }

        /// <summary>
        /// Qualifies a player for the minimum score required
        /// </summary>
        private bool QualifyGameplay(Player player, IEnumerable<Target> targets)
            => RankGameplay(player, targets) > Qualifier;
        /// <summary>
        /// Determines a player's score for the category 
        /// </summary>
        private float RankGameplay(Player player, IEnumerable<Target> targets)
        {
            // Get the gameplay stats for the player
            var gameplayStats = player.StatsAll?.FirstOrDefault();
            // Return the value based on property name
            return float.TryParse(gameplayStats.GetType()?.GetProperty(Property)?.GetValue(gameplayStats).ToString(), out var result) ? result : 0f;
        }

        /// <summary>
        /// Qualifies a player for the minimum score required
        /// </summary>
        private bool QualifyOffensive(Player player, IEnumerable<Target> targets) => RankOffensive(player, targets) > Qualifier;
        /// <summary>
        /// Determines a player's score for the category 
        /// </summary>
        private float RankOffensive(Player player, IEnumerable<Target> targets)
        {
            // Get the offensive stats for the player
            var offensiveStats = player.StatsTargets
                .Select(stats => stats?.FirstOrDefault())
                .Where(stats => stats is not null);
            // Get the property from each stat matching the name, and get the value for that property as a float
            var values = offensiveStats
                .Select(stats => float.TryParse(stats.GetType()?.GetProperty(Property)?.GetValue(stats).ToString(), out var result) ? result : 0f)
                .Where(value => value > 0f);

            // Sum up all the floats.
            return values.Sum();
        }

        /// <summary>
        /// Qualifies a player for the minimum score required
        /// </summary>
        private bool QualifyDefensive(Player player, IEnumerable<Target> targets) => RankDefensive(player, targets) > Qualifier;
        /// <summary>
        /// Determines a player's score for the category 
        /// </summary>
        private float RankDefensive(Player player, IEnumerable<Target> targets)
        {
            // Get the gameplay stats for the player
            var defensiveStats = player.Defenses?.FirstOrDefault();
            // Return the value based on property name
            return float.TryParse(defensiveStats.GetType()?.GetProperty(Property)?.GetValue(defensiveStats).ToString(), out var result) ? result : 0f;
        }

        /// <summary>
        /// Qualifies a player for the minimum score required
        /// </summary>
        private bool QualifySupport(Player player, IEnumerable<Target> targets) => RankSupport(player, targets) > Qualifier;
        /// <summary>
        /// Determines a player's score for the category 
        /// </summary>
        private float RankSupport(Player player, IEnumerable<Target> targets)
        {
            // Get the gameplay stats for the player
            var supportStats = player.Support?.FirstOrDefault();
            // Return the value based on property name
            return float.TryParse(supportStats.GetType()?.GetProperty(Property)?.GetValue(supportStats).ToString(), out var result) ? result : 0f;
        }

        /// <summary>
        /// Qualifies a player for the minimum score required
        /// </summary>
        private bool QualifySkills(Player player, IEnumerable<Target> targets) => RankSkills(player, targets) > Qualifier;
        /// <summary>
        /// Determines a player's score for the category 
        /// </summary>
        private float RankSkills(Player player, IEnumerable<Target> targets)
        {
            // Create a list of skills, based on the skill names specified for this award.
            var skills = AbilityNames?.Where(name => !string.IsNullOrEmpty(name))?.Any() is not true ? null : Skills
                .Where(skill => AbilityNames.Any(skillName => skill.Name == skillName))
                .ToList();
            // Look through all target damage for the player, and get a list of skills that match the ones provided
            var playerSkills = player.TargetDamageDist
                .Where(damage => skills?.Any(skill => skill.Id == damage?.FirstOrDefault()?.FirstOrDefault()?.Id) ?? true)
                .Select(damage => damage?.FirstOrDefault()?.FirstOrDefault())
                .Where(damage => damage is not null)
                .ToList();
            // Get the property from each skill matching the name, and get the value for that property as a float
            var values = playerSkills
                .Select(skill => float.TryParse(skill.GetType()?.GetProperty(Property)?.GetValue(skill).ToString(), out var result) ? result : 0f)
                .Where(value => value > 0f)
                .ToList();

            // Sum up all the floats.
            return values.Sum();
        }

        /// <summary>
        /// Qualifies a player for the minimum score required
        /// </summary>
        private bool QualifyBuffs(Player player, IEnumerable<Target> targets) => RankBuffs(player, targets) > Qualifier;
        /// <summary>
        /// Determines a player's score for the category 
        /// </summary>
        private float RankBuffs(Player player, IEnumerable<Target> targets)
        {
            // Create a list of skills, based on the skill names specified for this award.
            var buffs = AbilityNames?.Where(name => !string.IsNullOrEmpty(name))?.Any() is not true ? null : Buffs
                .Where(buff => AbilityNames.Any(buffName => buff.Name == buffName))
                .ToList();
            // Look through all target damage for the player, and get a list of skills that match the ones provided
            var playerBuffs = player.BuffUptimes
                .Where(uptime => buffs?.Any(buff => buff.Id == uptime?.Id) ?? true)
                .Select(uptime => uptime.BuffData?.FirstOrDefault())
                .Where(buffData => buffData is not null)
                .ToList();
            // Get the property from each skill matching the name, and get the value for that property as a float
            var values = playerBuffs
                .Select(buffData => float.TryParse(buffData.GetType()?.GetProperty(Property)?.GetValue(buffData).ToString(), out var result) ? result : 0f)
                .Where(value => value > 0f)
                .ToList();

            // Sum up all the floats.
            return values.Sum();
        }
    }
}
