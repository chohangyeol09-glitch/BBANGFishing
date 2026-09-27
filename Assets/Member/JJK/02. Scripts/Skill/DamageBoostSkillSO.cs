using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    [CreateAssetMenu(fileName = "DamageBoostSkillSO", menuName = "JJK/Skill/DamageBoost", order = 1)]
    public class DamageBoostSkillSO : SkillSO
    {
        [SerializeField] private float damageMultiplier = 2f;

        public override void OnActivate(PlayerSkillContext context)
        {
            context.CombatState.SetDamageMultiplier(damageMultiplier);
        }

        public override void OnDeactivate(PlayerSkillContext context)
        {
            context.CombatState.SetDamageMultiplier(1f);
        }
    }
}
