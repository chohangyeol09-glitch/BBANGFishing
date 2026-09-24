using System;
using CHG._02.Script.Agents;
using CHG._02.Script.CombatSystem;
using CHG._02.Script.CombatSystem.BT.Channel;
using CHG._02.Script.CoreSystem;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CHG._02.Script.FishSystem
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Fish : Agent, IParryable
    {
        public FishStateEnum State { get; private set; } = FishStateEnum.Jump;
        public BehaviorGraphAgent BTAgent { get; private set; }
        public bool IsInSea { get; private set; } = false;
        public override float MaxHealth => Data.Health;
        [field:SerializeField] public FishDataSO Data { get; private set; }

        [SerializeField] private bool isKnockBack = true;
        
        [Header("Catch")]
        [SerializeField, Range(0f,1f)] private float weightInfluence = 0.5f; //무게 반영 비율
        [SerializeField] private float minJumpHeight = 0.8f;
        
        public bool IsParryable => _lunge != null && _lunge.IsParryable;

        private LungeModule _lunge;
        private FishFacingModule _facingModule;
        private Rigidbody _rb;
        private bool _hasRisen; //처음에 올라갔는가


        protected override void InitializeModules()
        {
            base.InitializeModules();
            _rb = GetComponent<Rigidbody>();
            BTAgent = GetComponent<BehaviorGraphAgent>();
            _lunge = GetModule<LungeModule>();
            _facingModule = GetModule<FishFacingModule>();

            if (isKnockBack)
                OnDamaged += OnKnockBack;

        }
        
        public void OnSpawn(Vector3 pullForce, GameObject target)
        {
            _rb.mass = Data.Weight;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            CurrentHealth = MaxHealth;
            BTAgent.SetVariableValue("Fish", this);
            BTAgent.SetVariableValue("Target", target);
            
            _lunge.Target = target;

            Vector3 dir = pullForce.normalized;
            float floor = dir.y > 0.1f ?  PhysicsUtil.SpeedForHeight(minJumpHeight) / dir.y : 0f;
            float deltaV = PhysicsUtil.ResolveDeltaV(pullForce.magnitude, _rb.mass, weightInfluence, floor);
            float minSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * minJumpHeight);

            if (dir.y > 0.1f)
                deltaV = Mathf.Max(deltaV, minSpeed / dir.y);
            
            _rb.AddForce(dir * deltaV, ForceMode.VelocityChange);
            if (_facingModule != null) _facingModule.SnapTo(pullForce);
        }

        private void FixedUpdate()
        {
            if (State != FishStateEnum.Jump) return;

            if (_rb.linearVelocity.y > 0.01f)
            {
                _hasRisen = true;
                return;
            }
            
            if (_hasRisen) ChangeState(FishStateEnum.Combat);
        }

        public bool TryParry(DamageData data) => _lunge != null && _lunge.TryParry(data);

        public override void Dead()
        {
            base.Dead();   
            ChangeState(FishStateEnum.Dead);
        }

        private void OnDestroy()
        {
            if (isKnockBack)
                OnDamaged -= OnKnockBack;
        }

        private void OnKnockBack(DamageData data)
        {
            if (_rb == null || State != FishStateEnum.Combat) return;

            float impulse = PhysicsUtil.ResolveImpulse(data.KnockbackPower, _rb.mass, weightInfluence);

            _rb.AddForceAtPosition(data.HitDirection.normalized * impulse,
                data.HitPoint, ForceMode.Impulse);
        }

        
        private void OnTriggerEnter(Collider collision)
        {
                Debug.Log("Collision");
            if (collision.CompareTag("Sea"))
            {
                IsInSea = true;
            }
        }

        public void ChangeState(FishStateEnum newState)
        {
            if (State == FishStateEnum.Dead || State == newState) return;
            
            State = newState;
            BTAgent.SetVariableValue("State", State);
            if (BTAgent.GetVariable("StateChannel", out BlackboardVariable<StateChannel> channel))
                channel.Value.SendEventMessage(newState);
            else
                Debug.LogWarning("State Channel not found");
        }

        public void ConsumeSeaTouch() => IsInSea = false;
#if UNITY_EDITOR
        [Header("Test")]
        [SerializeField] private float testUpSpeed = 8f;
        [SerializeField] private float testParryDamage = 10f; 

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                DamageData data = new DamageData(this, transform.position, -transform.forward, 
                    transform.forward, testParryDamage, 0f);
                bool parried = TryParry(data);
                Debug.Log($"parry success? : {parried}");
            }
        }
#endif

    }
}
