using CHG._02.Script.CombatSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace Member.JJK._02._Scripts.Weapon
{
    public class ShootModule : MonoBehaviour, IModule
    {
        [SerializeField] private Camera playerCam;
        [SerializeField] private Transform muzzleTrm;

        private WeaponController _weaponController;
        private WeaponSO _weaponData;
        private DurabilityRuntimeState _durabilityState;
        private AimModule _aimModule;
        private BulletTracerModule _tracerModule;
        private float _lastFireTime;

        public void Initialize(ModuleOwner owner)
        {
            _weaponController = owner.GetComponent<WeaponController>();
            _weaponData = _weaponController.WeaponData;
            _durabilityState = _weaponController.DurabilityState;
            _aimModule = owner.GetModule<AimModule>();
            _tracerModule = owner.GetModule<BulletTracerModule>();
        }

        public void TryFire()
        {
            if (Time.time - _lastFireTime < _weaponData.FireRate) return;

            _lastFireTime = Time.time;
            FireRayCast();

            if (_weaponData.IsUnbreakable) return;

            _durabilityState.Consume(1);
            if (_durabilityState.IsBroken)
                _weaponController.Break();
        }

        private void FireRayCast()
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 tracerEndPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                tracerEndPoint = hit.point;
                ApplyDamage(hit.collider);
                SpawnImpactVfx(hit.point, hit.normal);
            }
            else
            {
                tracerEndPoint = ray.origin + ray.direction * 100f;
            }

            _tracerModule.ShowTracer(muzzleTrm.position, tracerEndPoint);
            _aimModule.ApplyRecoil(_weaponData.Recoil);
            SpawnMuzzleFlash();
        }

        private void ApplyDamage(Collider hitCollider)
        {
            IDamageable damageable = hitCollider.GetComponentInParent<IDamageable>();
            //damageable?.TakeDamage(_weaponController.CurrentDamage); //DamageData
        }

        private void SpawnMuzzleFlash()
        {
            Instantiate(_weaponData.MuzzleFlashPrefab, muzzleTrm.position, muzzleTrm.rotation, muzzleTrm);
        }

        private void SpawnImpactVfx(Vector3 pos, Vector3 normal)
        {
            if (_weaponData.ImpactVfxPrefab == null) return;

            Instantiate(_weaponData.ImpactVfxPrefab, pos, Quaternion.LookRotation(normal));
        }
    }
}