using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    [CreateAssetMenu(fileName = "SlowTimeSkillSO", menuName = "JJK/Skill/SlowTime", order = 0)]
    public class SlowTimeSkillSO : SkillSO
    {
        [SerializeField] private float timeScale = 0.3f;

        public override void OnActivate(PlayerSkillContext context)
        {
            EnemyTime.SetScale(timeScale);
            AfterimageEmitter.StartAll();
        }

        public override void OnDeactivate(PlayerSkillContext context)
        {
            EnemyTime.SetScale(1f);
            AfterimageEmitter.StopAll();
        }
    }
}
