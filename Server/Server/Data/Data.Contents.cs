using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Data
{
    #region Stat

    [Serializable]
    public class StatData : ILoader<int, StatInfo>
    {
        public List<StatInfo> stats = new List<StatInfo>();

        public Dictionary<int, StatInfo> MakeDict()
        {
            var dict = new Dictionary<int, StatInfo>();
            foreach (var stat in stats)
            {
                stat.Hp = stat.MaxHp;
                dict.Add(stat.Level, stat);
            }

            return dict;
        }
    }

    #endregion

    #region Skill

    [Serializable]
    public class Skill
    {
        public float cooldown;
        public int damage;
        public int id;
        public string name;
        public ProjectileInfo projectile;
        public SkillType skillType;
    }

    public class ProjectileInfo
    {
        public string name;
        public string prefab;
        public int range;
        public float speed;
    }

    [Serializable]
    public class SkillData : ILoader<int, Skill>
    {
        public List<Skill> skills = new List<Skill>();

        public Dictionary<int, Skill> MakeDict()
        {
            var dict = new Dictionary<int, Skill>();
            foreach (var skill in skills)
                dict.Add(skill.id, skill);
            return dict;
        }
    }

    #endregion
}