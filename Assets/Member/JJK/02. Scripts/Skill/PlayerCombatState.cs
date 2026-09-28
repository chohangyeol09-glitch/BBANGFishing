using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public class PlayerCombatState : MonoBehaviour
    {
        public float DamageMultiplier { get; private set; } = 1f;
        public bool HasProjectileShield { get; private set; }

        // 상점 등에서 구매하는 영구 강화치. 스킬용 DamageMultiplier(임시 버프, 끝나면 1로 리셋)와는
        // 별개로 누적되므로, 스킬이 꺼져도 이 강화는 사라지지 않는다.
        public float DamageUpgradeBonus { get; private set; }
        public float FireRateUpgradeBonus { get; private set; }

        public void SetDamageMultiplier(float multiplier) => DamageMultiplier = multiplier;

        // 총SO의 Damage에 더해지는 영구 데미지 강화. 다른 스크립트(상점 등)에서 호출해서 사용한다.
        public void UpgradeDamage(float amount) => DamageUpgradeBonus += amount;

        // 총SO의 FireRate(발사 간격, 초)에서 빼지는 영구 연사속도 강화 — 값이 클수록 더 빨리 쏜다.
        public void UpgradeFireRate(float amount) => FireRateUpgradeBonus += amount;

        public void GrantProjectileShield() => HasProjectileShield = true;

        public bool TryConsumeProjectileShield()
        {
            if (!HasProjectileShield) return false;

            HasProjectileShield = false;
            return true;
        }
    }
}
