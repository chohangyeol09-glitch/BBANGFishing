using System;
using CHG._02.Script.Agents;
using CHG._02.Script.CombatSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace NKT.Player.Modules
{
    //피해를 일정 시간 안 받으면 체력이 서서히 찬다
    public class HealthRegenModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private float regenDelay = 5f;             //마지막 피격 후 기다리는 시간
        [SerializeField] private float percentPerSecond = 0.08f;    //초당 최대체력의 몇 %
        [SerializeField] private float maxRegenRatio = 1f;          //여기까지만 회복. 1이면 만피

        public event Action<bool> OnRegenChanged;   //회복 시작/정지. UI용

        public bool IsRegen => _regen;

        //마지막 피격 후 남은 대기 시간. 0이면 회복 중
        public float RemainingDelay => Mathf.Max(0f, regenDelay - _timer);

        private CHG._02.Script.Agents.Agent _agent;
        private float _timer;
        private bool _regen;

        public void Initialize(ModuleOwner owner)
        {
            _agent = owner as CHG._02.Script.Agents.Agent;

            if (_agent == null)
                Debug.LogError("HealthRegenModule : Agent가 아닌 오너에 붙었습니다.", this);
        }

        public void AfterInit()
        {
            if (_agent == null) return;

            _agent.OnDamaged += HandleDamaged;
            _timer = regenDelay;    //시작하자마자 회복 가능한 상태
        }

        private void OnDestroy()
        {
            if (_agent == null) return;

            _agent.OnDamaged -= HandleDamaged;
        }

        private void HandleDamaged(DamageData data)
        {
            _timer = 0f;
            SetRegen(false);
        }

        private void Update()
        {
            if (_agent == null || _agent.IsDead)
            {
                SetRegen(false);
                return;
            }

            if (_timer < regenDelay)
            {
                _timer += Time.deltaTime;
                return;
            }

            float cap = _agent.MaxHealth * Mathf.Clamp01(maxRegenRatio);

            if (_agent.CurrentHealth >= cap)
            {
                SetRegen(false);
                return;
            }

            SetRegen(true);

            float amount = _agent.MaxHealth * percentPerSecond * Time.deltaTime;

            //상한을 넘어서 차지 않게 자른다
            _agent.Heal(Mathf.Min(amount, cap - _agent.CurrentHealth));
        }

        private void SetRegen(bool on)
        {
            if (_regen == on) return;

            _regen = on;
            OnRegenChanged?.Invoke(_regen);
        }
    }
}
