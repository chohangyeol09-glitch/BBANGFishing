using DevLib.ModuleSystem;
using UnityEngine;

namespace Member.JJK._02._Scripts.Weapon
{
    public class ShootModule : MonoBehaviour, IModule
    {
        [SerializeField] private Camera playerCam;
        [SerializeField] private Transform muzzleTrm;
        
        private WeaponSO _weaponData;
        private AmmoRuntimeState _ammoState;
        private AimModule _aimModule;
        private BulletTracerModule _tracerModule;
        private float _lastFireTime;
        
        public void Initialize(ModuleOwner owner)
        {
            _weaponData = owner.GetComponent<WeaponController>().WeaponData;
            _ammoState = new AmmoRuntimeState(_weaponData);
            _aimModule = owner.GetModule<AimModule>();
            _tracerModule = owner.GetModule<BulletTracerModule>();
        }

        public void TryFire()
        {
            if (_ammoState.isReloading) return;
            if (Time.time - _lastFireTime < _weaponData.FireRate) return;
            if (_ammoState.currentAmmo <= 0)
            {
                //재장전
                return;
            }
            
            _lastFireTime = Time.time;
            _ammoState.currentAmmo--;
            FireRayCast();
        }

        private void FireRayCast()
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 tracerEndPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                tracerEndPoint = hit.point;
            }
            else
            {
                tracerEndPoint = ray.origin + ray.direction * 100f;
            }
            
            _tracerModule.ShowTracer(muzzleTrm.position, tracerEndPoint);
            _aimModule.ApplyRecoil(_weaponData.Recoil);
        }

        private void SpawnMuzzleFlash()
        {
            
        }

        private void SpawnImpactVfx(Vector3 pos, Vector3 normal)
        {
            
        }
    }
}