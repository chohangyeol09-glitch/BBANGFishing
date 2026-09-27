using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    [CreateAssetMenu(fileName = "ProjectileShieldSkillSO", menuName = "JJK/Skill/ProjectileShield", order = 2)]
    public class ProjectileShieldSkillSO : SkillSO
    {
        public override void OnActivate(PlayerSkillContext context)
        {
            context.CombatState.GrantProjectileShield();
        }
    }
}
