using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Data
{
    #region Skill

    [Serializable]
    public class Skill
    {
        public int id;
        public string name;
        public float cooldown;
        public int damage;
        public SkillType skillType;
        public ProjectileInfo projectile;
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
        public List<Skill> skills = new();

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