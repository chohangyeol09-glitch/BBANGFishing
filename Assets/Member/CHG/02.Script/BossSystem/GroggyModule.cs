using System;
using System.Collections;
using CHG._02.Script.CombatSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.BossSystem
{
    public class GroggyModule : Module, IAfterInitModule
    {
        public event Action<float> OnGaugeChanged;
        
        public float Gauge { get; private set; }
        public bool IsFull => Gauge >= _maxGauge;
        public float NormalizedGauge => _maxGauge <= 0f ? 0f : Gauge / _maxGauge;

        [SerializeField] private float damageToGroggy = 1f; //받은 데미지 1당 그로기 전환 비율
        private Boss _boss;
        private float _maxGauge => _boss.Data.GroggyMaxGauge;
        
        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            _boss = owner as Boss;
        }
        
        public void AfterInit()
        {
            if (_boss != null)
                _boss.OnDamaged += HandleDamaged;
        }

        private void OnDestroy()
        {
            if (_boss != null)
                _boss.OnDamaged -= HandleDamaged;
        }

        private void HandleDamaged(DamageData data)
        {
            AddGroggy(data.Damage * damageToGroggy);
        }


        public void AddGroggy(float amount)
        {
            if (amount <= 0f || IsFull || _boss.State != BossStateEnum.Combat) return;
            
            Gauge = Mathf.Min(Gauge + amount, _maxGauge);
            OnGaugeChanged?.Invoke(NormalizedGauge);

            if (IsFull) _boss.SendState(BossStateEnum.Groggy);
        }

        public void ResetGauge()
        {
            Gauge = 0f;
            OnGaugeChanged?.Invoke(0f);
        }
    }
}