using UnityEngine;

namespace Member.JJK._02._Scripts.Weapon
{
    public class DurabilityRuntimeState
    {
        public float MaxDurability { get; }
        public float CurrentDurability { get; private set; }
        public bool IsBroken => CurrentDurability <= 0f;

        public DurabilityRuntimeState(WeaponSO weaponData)
        {
            MaxDurability = weaponData.Durability;
            CurrentDurability = MaxDurability;
        }

        public void Consume(float amount)
        {
            CurrentDurability = Mathf.Max(0f, CurrentDurability - amount);
        }
    }
}
