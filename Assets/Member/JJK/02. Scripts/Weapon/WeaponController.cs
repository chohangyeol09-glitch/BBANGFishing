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

        public float CurrentDamage =>
            (WeaponData.Damage + (combatState != null ? combatState.DamageUpgradeBonus : 0f))
            * (combatState != null ? combatState.DamageMultiplier : 1f);

        // 발사 간격(초). 영구 강화만큼 줄어들고, 너무 짧아져 발사 로직이 깨지지 않도록 최소값으로 막는다.
        public float CurrentFireRate =>
            Mathf.Max(0.01f, WeaponData.FireRate - (combatState != null ? combatState.FireRateUpgradeBonus : 0f));

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

        // 상점 등 다른 쪽 코드가 무기를 영구 강화할 때 호출하는 진입점.
        // PlayerCombatState는 플레이어 쪽에 계속 남아있어서, 이 무기가 파괴되고 새 무기를 들어도 강화치는 유지된다.
        public void UpgradeDamage(float amount) => combatState?.UpgradeDamage(amount);

        public void UpgradeFireRate(float amount) => combatState?.UpgradeFireRate(amount);
    }
}