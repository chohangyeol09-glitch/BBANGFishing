using System;
using DevLib.AnimatorSystem;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    [Serializable]
    public class SkillEntry
    {
        public HashDataSO Skill;
        public int Priority;
        public AbstractSkillCondition[] Conditions;
    }
}