using System;
using CHG._02.Script.CombatSystem;
using CHG._02.Script.CoreSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    public class LungeModule : MonoBehaviour, IModule, IAfterInitModule
    {
        private enum State { Idle, Approach, Return }

        public event Action OnLungeStart;
        public event Action OnParried;
        public event Action OnReturned;

        public GameObject Target { get; set; }

        public bool IsParryable => _state == State.Approach && Target != null &&
            Vector3.Distance(_body.position, Target.transform.position) <= _fish.Data.ParryRange; //일정 거리 안 일때 패링 가능

        private Fish _fish;
        private Transform _body; //움직일 대상. 모듈이 자식 오브젝트에 있어도 Rigidbody가 있는 Fish 루트를 움직인다
        private Rigidbody _rb;
        private State _state;
        private bool _used; 
        private Vector3 _seaPoint; //Sea에 닿은 지점. 되돌아갈 곳
        private Vector3 _flightStart;
        private Vector3 _flightVelocity;
        private float _flightDuration;
        private float _elapsed;

        public void Initialize(ModuleOwner owner)
        {
            _fish = owner as Fish;
            _body = _fish.transform;
            _rb = _fish.GetComponent<Rigidbody>();
        }

        public void AfterInit()
        {
            _fish.OnDeath += HandleDeath;
        }

        private void OnDestroy()
        {
            if (_fish != null) _fish.OnDeath -= HandleDeath;
        }

        public void StartLunge()
        {
            Debug.Log("Lunge");
            if (_used || Target == null || _fish.IsDead) return;
            
            Debug.Log("Lunge Success");
            _used = true;

            _seaPoint = _body.position;
            Transform target = Target.transform;
            Vector3 arrival = target.position + target.forward * _fish.Data.LungeFrontDistance
                                              + Vector3.up * _fish.Data.LungeHeightOffset;

            BeginFlight(State.Approach, arrival, _fish.Data.LungeFlightTime);
            OnLungeStart?.Invoke();
        }

        public bool TryParry(DamageData data)
        {
            if (!IsParryable) return false;

            _fish.TakeDamage(data);
            OnParried?.Invoke();

            // 죽었다면 HandleDeath에서 이미 비행이 끝났다
            if (!_fish.IsDead) BeginFlight(State.Return, _seaPoint, _fish.Data.ReturnFlightTime);
            return true;
        }

        private void BeginFlight(State state, Vector3 destination, float duration)
        {
            _state = state;
            _flightDuration = Mathf.Max(duration, 0.01f);
            _elapsed = 0f;
            _flightStart = _body.position;
            _flightVelocity = PhysicsUtil.SolveBallisticVelocity(_flightStart, destination, _flightDuration);

            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
        }

        private void Update()
        {
            if (_state == State.Idle) return;

            _elapsed += Time.deltaTime;
            _body.position = PhysicsUtil.BallisticPosition(_flightStart, _flightVelocity,
                Mathf.Min(_elapsed, _flightDuration));

            if (_elapsed < _flightDuration) return;

            if (_state == State.Approach)
            {
                // 도착까지 패링이 없었다 -> 되돌아간다
                BeginFlight(State.Return, _seaPoint, _fish.Data.ReturnFlightTime);
                return;
            }

            EndFlight(false);
            OnReturned?.Invoke();
        }

        private void HandleDeath()
        {
            if (_state != State.Idle) EndFlight(true);
        }

        //비행을 끝내고 물리로 되돌린다. 죽은 경우에는 비행 중이던 속도를 이어받아 자연스럽게 떨어진다
        private void EndFlight(bool keepVelocity)
        {
            Vector3 velocity = keepVelocity ? _flightVelocity + Physics.gravity * _elapsed : Vector3.zero;
            _state = State.Idle;
            _rb.isKinematic = false;
            _rb.linearVelocity = velocity;
        }
    }
}
