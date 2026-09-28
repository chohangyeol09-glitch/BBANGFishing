using UnityEngine;

namespace Member.JJK._02._Scripts.Weapon
{
    [CreateAssetMenu(fileName = "WeaponSO", menuName = "JJK/WeaponData", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        [field: SerializeField] public float Damage { get; private set; } = 10;

        [field: SerializeField] public float FireRate { get; private set; } = 0.1f;
        [field: SerializeField] public float KnockbackPower { get; private set; } = 0.1f;

        // 내구도 시스템은 꺼져서 더 이상 소모되지 않는다(ShootModule이 더 이상 건드리지 않음).
        // NKT WeaponEquipModule이 DurabilityRuntimeState/SetDurabilityState/OnBroken을 참조하고 있어서
        // 이 필드와 관련 타입은 호환성 때문에 남겨둔다.
        [field: SerializeField] public float Durability { get; private set; } = 100f;
        [field: SerializeField] public Vector2 Recoil { get; private set; } = new Vector2(1.5f, 0.5f);
        [field: SerializeField] public float RecoilRecovery { get; private set; } = 5f;
        [field: SerializeField] public bool IsAuto { get; private set; } = true;
        [field: SerializeField] public GameObject MuzzleFlashPrefab { get; private set; }
        [field: SerializeField] public GameObject ImpactVfxPrefab { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}