using System.Collections;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    public class HealSkill : AbstractEnemySkill
    {
        [SerializeField, Range(0f, 1f)] private float healPercent = 0.3f; //최대 체력 대비 회복 비율

        public override bool CanUseSkill(GameObject target = null)
        {
            if (!base.CanUseSkill(target)) return false;
            return Owner.CurrentHealth < Owner.MaxHealth; 
        }

        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            Owner.Heal(Owner.MaxHealth * healPercent);
            yield break;
        }
    }
}