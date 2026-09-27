using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public abstract class SkillSO : ScriptableObject
    {
        [field: SerializeField] public string SkillName { get; private set; } = "New Skill";
        [field: SerializeField] public float Cooldown { get; private set; } = 5f;
        [field: SerializeField] public float Duration { get; private set; } = 3f;
        [field: SerializeField] public Sprite Icon { get; private set; }

        public virtual bool HasDuration => Duration > 0f;

        public abstract void OnActivate(PlayerSkillContext context);
        public virtual void OnUpdate(PlayerSkillContext context, float elapsedTime) { }
        public virtual void OnDeactivate(PlayerSkillContext context) { }
    }
}
