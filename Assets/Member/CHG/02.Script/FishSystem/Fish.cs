using System;
using CHG._02.Script.Agents;
using CHG._02.Script.CombatSystem;
using CHG._02.Script.CoreSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    public class Fish : Agent
    {
        public bool IsJumping = false;
        public override float MaxHealth => Data.Health;
        [field:SerializeField] public FishDataSO Data { get; private set; }

        [SerializeField] private bool isKnockBack = true;
        
        [Header("Catch")]
        [SerializeField, Range(0f,1f)] private float weightInfluence = 0.5f; //무게 반영 비율
        
        [SerializeField] private float minJumpHeight = 0.8f;
        
        private Rigidbody _rb;


        protected override void InitializeModules()
        {
            base.InitializeModules();
            _rb = GetComponent<Rigidbody>();
            
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
            float floor = dir.y > 0.1f ?  ForceUtil.SpeedForHeight(minJumpHeight) / dir.y : 0f;
            float deltaV = ForceUtil.ResolveDeltaV(pullForce.magnitude, _rb.mass, weightInfluence, floor);
            float minSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * minJumpHeight);

            if (dir.y > 0.1f)
                deltaV = Mathf.Max(deltaV, minSpeed / dir.y);
            
            _rb.AddForce(dir * deltaV, ForceMode.VelocityChange);
        }

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

            float impulse = ForceUtil.ResolveImpulse(data.KnockbackPower, _rb.mass, weightInfluence);

            _rb.AddForceAtPosition(data.HitDirection.normalized * impulse,
                data.HitPoint, ForceMode.Impulse);
        }

        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Sea"))
            {
                if (IsJumping)
                {
                    IsJumping = false;
                }
            }
        }

    }
}
