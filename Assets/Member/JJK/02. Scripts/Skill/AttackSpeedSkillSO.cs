using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    [CreateAssetMenu(fileName = "AttackSpeedSkillSO", menuName = "JJK/Skill/AttackSpeed", order = 3)]
    public class AttackSpeedSkillSO : SkillSO
    {
        [SerializeField] private float attackSpeedMultiplier = 2f;

        public override void OnActivate(PlayerSkillContext context)
        {
            context.CombatState.SetAttackSpeedMultiplier(attackSpeedMultiplier);
        }

        public override void OnDeactivate(PlayerSkillContext context)
        {
            context.CombatState.SetAttackSpeedMultiplier(1f);
        }
    }
}
