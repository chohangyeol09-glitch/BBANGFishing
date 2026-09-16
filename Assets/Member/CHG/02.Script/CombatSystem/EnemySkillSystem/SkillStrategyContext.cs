using CHG._02.Script.Agents;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public struct SkillStrategyContext
    {
        public readonly EnemySkillModule SkillModule;
        public readonly Agent Owner;
        public readonly GameObject Target;

        public SkillStrategyContext(EnemySkillModule skillModule, Agent owner, GameObject target)
        {
            SkillModule = skillModule;
            Owner = owner;
            Target = target;
        }
    }
}