using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public class PlayerCombatState : MonoBehaviour
    {
        public float DamageMultiplier { get; private set; } = 1f;
        public bool HasProjectileShield { get; private set; }

        public void SetDamageMultiplier(float multiplier) => DamageMultiplier = multiplier;

        public void GrantProjectileShield() => HasProjectileShield = true;

        public bool TryConsumeProjectileShield()
        {
            if (!HasProjectileShield) return false;

            HasProjectileShield = false;
            return true;
        }
    }
}
