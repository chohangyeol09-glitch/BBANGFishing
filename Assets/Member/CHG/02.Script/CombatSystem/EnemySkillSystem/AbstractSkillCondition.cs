using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public abstract class AbstractSkillCondition : ScriptableObject
    {
        public abstract bool IsCanUse(SkillConditionContext context);
    }
}