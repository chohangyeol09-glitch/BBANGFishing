using DevLib.ModuleSystem;
using UnityEngine;

namespace Member.JJK._02._Scripts.Weapon
{
    public class ShootModule : MonoBehaviour, IModule
    {
        private WeaponSO _weaponData;
        private AmmoRuntimeState _ammoState;
        private AimModule _aimModule;
        private float _lastFireTime;
        
        public void Initialize(ModuleOwner owner)
        {
            _weaponData = owner.GetComponent<WeaponController>().WeaponData;
            _ammoState = new AmmoRuntimeState(_weaponData);
            _aimModule = owner.GetModule<AimModule>();
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
            //발사 로직
            _aimModule.ApplyRecoil(_weaponData.Recoil);
        }

        private void FireRayCast()
        {
            
        }

        private void SpawnMuzzleFlash()
        {
            
        }

        private void SpawnImpactVfx(Vector3 pos, Vector3 normal)
        {
            
        }
    }
}