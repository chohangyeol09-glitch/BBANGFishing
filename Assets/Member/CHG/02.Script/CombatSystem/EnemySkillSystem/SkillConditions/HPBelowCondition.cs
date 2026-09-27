using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.SkillConditions
{
    [CreateAssetMenu(fileName = "HP below", menuName = "Fish/Skill.Condition/HP below", order = 0)]
    public class HPBelowCondition : AbstractSkillCondition
    {
        [SerializeField, Range(0f, 1f)] private float percent = 0.3f;
        
        public override bool IsCanUse(SkillConditionContext context)
            => context.Owner.CurrentHealth / context.Owner.MaxHealth <= percent;
    }
}