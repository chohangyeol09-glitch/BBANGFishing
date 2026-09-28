using System;
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
        [SerializeField] private float breakDelay = 0.1f;
        public DurabilityRuntimeState DurabilityState { get; private set; }


        public event Action<WeaponController> OnBroken;
        public float CurrentDamage => WeaponData.Damage * (combatState != null ? combatState.DamageMultiplier : 1f);

        private ShootModule _shootModule;
        private AimModule _aimModule;
        private bool _isBroken;

        protected override void InitializeModules()
        {
            DurabilityState ??= new DurabilityRuntimeState(WeaponData);
            base.InitializeModules();
            _shootModule = GetModule<ShootModule>();
            _aimModule = GetModule<AimModule>();
        }

        public void SetDurabilityState(DurabilityRuntimeState state)
        {
            DurabilityState = state;
        }
        public void Fire()
        {
            if (_isBroken) return;
            
            _shootModule.TryFire();
        }

        public void SetAiming(bool aiming)
        {
            if (_isBroken) return;
            
            _aimModule.SetAiming(aiming);
        }

        public void Break()
        {
            if (_isBroken) return;

            _isBroken = true;
            OnBroken?.Invoke(this);
            // 마지막 발사의 트레이서/머즐플래시가 보일 수 있도록 약간 늦게 제거한다.
            Destroy(gameObject, breakDelay);
        }
    }
}