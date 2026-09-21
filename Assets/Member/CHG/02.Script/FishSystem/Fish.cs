using System;
using CHG._02.Script.Agents;
using CHG._02.Script.CombatSystem;
using CHG._02.Script.CoreSystem;
using DevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CHG._02.Script.FishSystem
{
    public class Fish : Agent, IParryable
    {
        public bool IsJumping = false;
        public override float MaxHealth => Data.Health;
        [field:SerializeField] public FishDataSO Data { get; private set; }

        [SerializeField] private bool isKnockBack = true;
        
        [Header("Catch")]
        [SerializeField, Range(0f,1f)] private float weightInfluence = 0.5f; //무게 반영 비율
        
        [SerializeField] private float minJumpHeight = 0.8f;
        
        public bool IsParryable => _lunge != null && _lunge.IsParryable;

        private Rigidbody _rb;
        private LungeModule _lunge;


        protected override void InitializeModules()
        {
            base.InitializeModules();
            _rb = GetComponent<Rigidbody>();
            _lunge = GetModule<LungeModule>();

            if (isKnockBack)
                OnDamaged += OnKnockBack;

        }
        
        public void OnSpawn(Vector3 pullForce)
        {
            _rb.mass = Data.Weight;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            CurrentHealth = MaxHealth;
            IsJumping = true;

            Vector3 dir = pullForce.normalized;
            float floor = dir.y > 0.1f ?  PhysicsUtil.SpeedForHeight(minJumpHeight) / dir.y : 0f;
            float deltaV = PhysicsUtil.ResolveDeltaV(pullForce.magnitude, _rb.mass, weightInfluence, floor);
            float minSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * minJumpHeight);

            if (dir.y > 0.1f)
                deltaV = Mathf.Max(deltaV, minSpeed / dir.y);
            
            _rb.AddForce(dir * deltaV, ForceMode.VelocityChange);
        }

        public bool TryParry(DamageData data) => _lunge != null && _lunge.TryParry(data);

        public override void Dead()
        {
            
            base.Dead();   
        }

        private void OnDestroy()
        {
            if (isKnockBack)
                OnDamaged -= OnKnockBack;
        }

        private void OnKnockBack(DamageData data)
        {
            if (_rb == null) return;

            float impulse = PhysicsUtil.ResolveImpulse(data.KnockbackPower, _rb.mass, weightInfluence);

            _rb.AddForceAtPosition(data.HitDirection.normalized * impulse,
                data.HitPoint, ForceMode.Impulse);
        }

        
        private void OnTriggerEnter(Collider collision)
        {
                Debug.Log("Collision");
            if (collision.CompareTag("Sea"))
            {
                Debug.Log(collision.name);
                if (IsJumping)
                {
                    IsJumping = false;
                    if (Data.CanLunge && _lunge != null)
                    {
                        _lunge.StartLunge();
                    }
                    else Debug.LogError("Lunge can't be lunge");
                }
            }
        }
        
#if UNITY_EDITOR
        [Header("Test")]
        [SerializeField] private float testUpSpeed = 8f;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                _rb.AddForce(Vector3.up * testUpSpeed, ForceMode.VelocityChange);
        }
#endif

    }
}
