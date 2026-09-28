using System;
using CHG._02.Script.CoreSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    public class LungeModule : MonoBehaviour, IModule, IAfterInitModule
    {

        public event Action OnLungeStart;
        public event Action OnReturned;

        public GameObject Target { get; set; }

        public bool HasLunged => _used;
        public bool IsFlying => _phase != Phase.Idle;
        public bool IsApproaching => _phase == Phase.Approach;
        public bool IsReturning => _phase == Phase.Return;

        public Vector3 FlightVelocity => _flightVelocity + Physics.gravity * Mathf.Min(_elapsed, _flightDuration);

        private enum Phase
        {
            Idle,
            Approach,
            Return
        }

        private Fish _fish;
        private Transform _body;
        private Rigidbody _rb;
        private bool _used;
        private Vector3 _seaPoint; //Sea에 닿은 지점. 되돌아갈 곳
        private Vector3 _flightStart;
        private Vector3 _flightVelocity;
        private float _flightDuration;
        private float _elapsed;
        private Phase _phase;

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

            BeginFlight(Phase.Approach, arrival, _fish.Data.LungeFlightTime);
            OnLungeStart?.Invoke();
        }

        //접근 중이면 도착을 기다리지 않고 바로 바다 지점으로 되돌아간다 (ParryModule이 패링 성공 시 부른다)
        public void ReturnNow()
        {
            if (_phase != Phase.Approach) return;
            BeginFlight(Phase.Return, _seaPoint, _fish.Data.ReturnFlightTime);
        }

        private void BeginFlight(Phase phase, Vector3 destination, float duration)
        {
            _phase = phase;
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
            if (_phase == Phase.Idle) return;

            _elapsed += Time.deltaTime;
            _body.position = PhysicsUtil.BallisticPosition(_flightStart, _flightVelocity,
                Mathf.Min(_elapsed, _flightDuration));

            if (_elapsed < _flightDuration) return;

            if (_phase == Phase.Approach)
            {
                // 도착까지 패링이 없다면 되돌아간다
                BeginFlight(Phase.Return, _seaPoint, _fish.Data.ReturnFlightTime);
                return;
            }

            EndFlight(false);
            OnReturned?.Invoke();
        }

        private void HandleDeath()
        {
            if (_phase != Phase.Idle) EndFlight(true);
        }   


        private void EndFlight(bool keepVelocity)
        {
            Vector3 velocity = keepVelocity ? _flightVelocity + Physics.gravity * _elapsed : Vector3.zero;
            _phase = Phase.Idle;
            _rb.isKinematic = false;
            _rb.linearVelocity = velocity;
        }

        public void ResetLunge()
        {
            _phase = Phase.Idle;
            _used = false;
        }
    }
}
