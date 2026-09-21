using UnityEngine;

namespace Member.JJK._02._Scripts.Weapon
{
    [CreateAssetMenu(fileName = "WeaponSO", menuName = "JJK/WeaponData", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        [field: SerializeField] public float Damage { get; private set; } = 10f;

        [field: SerializeField] public float FireRate { get; private set; } = 0.1f;
        
        [field: SerializeField] public int MagazineSize { get; private set; } = 30;
        [field: SerializeField] public float ReloadTime { get; private set; } = 2f;
        [field: SerializeField] public Vector2 Recoil { get; private set; } = new Vector2(1.5f, 0.5f);
        [field: SerializeField] public float RecoilRecovery { get; private set; } = 5f;
        [field: SerializeField] public GameObject MuzzleFlashPrefab { get; private set; }
        [field: SerializeField] public GameObject ImpactVfxPrefab { get; private set; }
    }
}