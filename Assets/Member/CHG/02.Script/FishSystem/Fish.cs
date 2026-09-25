using System;
using CHG._02.Script.Agents;
using CHG._02.Script.CombatSystem;
using CHG._02.Script.CombatSystem.BT.Channel;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using CHG._02.Script.CoreSystem;
using DevLib.ObjectPool.Runtime;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using IPoolable = DevLib.ObjectPool.Runtime.IPoolable;

namespace CHG._02.Script.FishSystem
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Fish : Agent, IParryable, ISkillEntrySource, IPoolable, IDamageMultiplier
    {
        public FishStateEnum State { get; private set; } = FishStateEnum.Jump;
        public BehaviorGraphAgent BTAgent { get; private set; }
        public bool IsInSea { get; private set; } = false;
        [field:SerializeField] public PoolItemSO PoolItem { get; set; }
        public GameObject GameObject => this != null ? this.gameObject : null;
        public float DamageMultiplier => Data.DamageMultiplier;
        
        public bool HasStartedFalling { get; private set; }
        
        public override float MaxHealth => Data.Health;
        public SkillEntry[] SkillEntries => Data.Skills;
        
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
        private PoolManagerSO _poolManager;
        private bool _released;
        private StateChannel _stateChannel;

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
        
        public void OnSpawn(Vector3 pullForce, GameObject target, PoolManagerSO poolManager)
        {
            _poolManager = poolManager;
            _rb.mass = Data.Weight;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            CurrentHealth = MaxHealth;
            
            BTAgent.SetVariableValue("Fish", this);
            BTAgent.SetVariableValue("Target", target);
            BindStateChannel();
            BTAgent.Restart();
            
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

            if (_hasRisen) HasStartedFalling = true;
        }

        public bool TryParry(DamageData data) => _lunge != null && _lunge.TryParry(data);

        public override void Dead()
        {
            base.Dead();   
            SendState(FishStateEnum.Dead);
        }

        private void OnDestroy()
        {
            if (isKnockBack)
                OnDamaged -= OnKnockBack;
            if (_stateChannel != null)
                _stateChannel.Event -= HandleStateChanged;
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
        
        public void ResetItem()
        {
            State = FishStateEnum.Jump;
            IsInSea = false;
            _hasRisen = false;
            _released = false;
            HasStartedFalling = false;

            _rb.isKinematic = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _lunge.ResetLunge();
        }

        public void ReleaseToPool()
        {
            if (_released) return;
            _released = true;

            EnemySkillModule skillModule = GetModule<EnemySkillModule>();
            if (skillModule != null && skillModule.CurrentSkill != null)
                skillModule.CurrentSkill.StopSkill();

            if (_poolManager == null)
            {
                Destroy(gameObject);
                return;
            }
            
            _poolManager.Push(this);
        }

        public void ConsumeSeaTouch() => IsInSea = false;

        private void BindStateChannel()
        {
            if (_stateChannel == null)
            {
                if (!BTAgent.GetVariable("StateChannel", out BlackboardVariable<StateChannel> channel) ||
                    channel.Value == null)
                {
                    Debug.LogWarning("Channel not found");
                    return;
                }

                _stateChannel = channel.Value;
            }

            _stateChannel.Event -= HandleStateChanged;
            _stateChannel.Event += HandleStateChanged;
        }

        private void HandleStateChanged(FishStateEnum newState) => State = newState;

        public void SendState(FishStateEnum newState)
        {
            if (_stateChannel != null) _stateChannel.SendEventMessage(newState);
        }
        
        
        
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
            
            if (Keyboard.current.hKey.wasPressedThisFrame)
                TakeDamage(new DamageData(this, transform.position, Vector3.up, Vector3.up, 4f, 0f));
        }
#endif
    }
}
