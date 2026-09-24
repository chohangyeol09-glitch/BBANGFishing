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
        public float NormalizedGauge => _maxGauge <= 0f ? 0f : Gauge / _maxGauge;

        [SerializeField] private float damageToGroggy = 1f; //받은 데미지 1당 그로기 전환 비율
        private Boss _boss;
        private Coroutine _groggyRoutine;
        private float _maxGauge => _boss.Data.GroggyMaxGauge;
        
        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            _boss = owner as Boss;
        }
        
        public void AfterInit()
        {
            if (_boss == null) return;
            _boss.OnDamaged += HandleDamaged;
            _boss.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            if (_boss == null) return;
            _boss.OnDamaged += HandleDamaged;
            _boss.OnStateChanged += HandleStateChanged;
        }

        private void HandleDamaged(DamageData data)
        {
            AddGroggy(data.Damage * damageToGroggy);
        }


        public void AddGroggy(float amount)
        {
            if (amount <= 0f || _boss.State != BossStateEnum.Combat) return;
            
            Gauge = Mathf.Min(Gauge + amount, _maxGauge);
            OnGaugeChanged?.Invoke(NormalizedGauge);

            if (Gauge >= _maxGauge)
            {
                _boss.ChangeState(BossStateEnum.Groggy);
                _groggyRoutine = StartCoroutine(GroggyRoutine());
            }
        }

        private IEnumerator GroggyRoutine()
        {
            yield return new WaitForSeconds(_boss.Data.GroggyDuration);
            _groggyRoutine = null;
            Gauge = 0f;
            OnGaugeChanged?.Invoke(0f);
            _boss.ChangeState(BossStateEnum.Combat);
        }

        private void HandleStateChanged(BossStateEnum state)
        {
            if (state == BossStateEnum.Dead && _groggyRoutine != null)
            {
                StopCoroutine(_groggyRoutine);
                _groggyRoutine = null;
            }
        }
    }
}