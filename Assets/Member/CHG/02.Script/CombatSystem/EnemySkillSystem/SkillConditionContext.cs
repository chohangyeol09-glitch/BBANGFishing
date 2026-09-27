using CHG._02.Script.Agents;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public struct SkillConditionContext
    {
        public readonly Agent Owner;
        public readonly GameObject Target;
        
        public SkillConditionContext(Agent owner, GameObject target)
        {
            Owner = owner;
            Target = target;
        }
    }
}