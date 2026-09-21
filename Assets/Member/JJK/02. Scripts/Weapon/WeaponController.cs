using DevLib.ModuleSystem;
using Member.JJK._02._Scripts.Skill;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.JJK._02._Scripts.Weapon
{
    public class WeaponController : ModuleOwner
    {
        [field: SerializeField] public WeaponSO WeaponData { get; private set; }
        [SerializeField] private PlayerCombatState combatState;
        public AmmoRuntimeState AmmoState { get; private set; }

        public float CurrentDamage => WeaponData.Damage * (combatState != null ? combatState.DamageMultiplier : 1f);

        private ShootModule _shootModule;
        private AimModule _aimModule;

        protected override void InitializeModules()
        {
            AmmoState = new AmmoRuntimeState(WeaponData);
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