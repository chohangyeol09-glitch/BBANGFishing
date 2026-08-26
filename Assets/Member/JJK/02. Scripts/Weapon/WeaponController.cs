using System;
using DevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.JJK._02._Scripts.Weapon
{
    public class WeaponController : ModuleOwner
    {
        [field: SerializeField] public WeaponSO WeaponData { get; private set; }
        
        private ShootModule _shootModule;
        private AimModule _aimModule;
        
        protected override void InitializeModules()
        {
            base.InitializeModules();
            _shootModule = GetModule<ShootModule>();
            _aimModule = GetModule<AimModule>();
        }

        private void Update()
        {
            _aimModule.SetAiming(Mouse.current.rightButton.isPressed);
            
            if (Mouse.current.leftButton.isPressed)
                _shootModule.TryFire();
        }
    }
}