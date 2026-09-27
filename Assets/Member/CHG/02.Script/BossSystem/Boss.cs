using System;
using System.Collections;
using CHG._02.Script.Agents;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using Unity.Behavior;
using UnityEngine;

namespace CHG._02.Script.BossSystem
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Boss : Agent, ISkillEntrySource, IDamageMultiplier
    {
        public event Action<BossStateEnum> OnStateChanged;

        public BossStateEnum State { get; private set; } = BossStateEnum.Appear;
        public BehaviorGraphAgent BTAgent { get; private set; }
        public float DamageMultiplier => Data.DamageMultiplier;
        

        public override float MaxHealth => Data.Health;
        public SkillEntry[] SkillEntries => Data.Skills;
        
        [field: SerializeField] public BossDataSO Data { get; private set; }

        private BossStateChannel _stateChannel;
        
        protected override void InitializeModules()
        {
            
            BTAgent = GetComponent<BehaviorGraphAgent>();
            base.InitializeModules();
        }

        public void OnSpawn(GameObject target)
        {
            CurrentHealth = MaxHealth;
            State = BossStateEnum.Appear;
            
            BTAgent.SetVariableValue("Boss", this);
            BTAgent.SetVariableValue("Target", target);
            BTAgent.SetVariableValue("AppearDuration", Data.AppearDuration);
            BTAgent.SetVariableValue("GroggyDuration", Data.GroggyDuration);

            BindStateChannel();
            EnemyRenderer enemyRenderer = GetModule<EnemyRenderer>();
            if (enemyRenderer != null) enemyRenderer.BindChannel(BTAgent);
            BTAgent.Restart();
        }

        public override void Dead()
        {
            base.Dead();
            SendState(BossStateEnum.Dead);
        }

        public void SendState(BossStateEnum newState)
        {
            if (_stateChannel != null) _stateChannel.SendEventMessage(newState);
        }
        
        private void BindStateChannel()
        {
            if (_stateChannel == null)
            {
                if (!BTAgent.GetVariable("StateChannel", out BlackboardVariable<BossStateChannel> channel) ||
                    channel.Value == null)
                {
                    Debug.LogWarning("boss StateChannel is not found");
                    return;
                }
                
                _stateChannel = channel.Value;
            }

            _stateChannel.Event -= HandleStateChanged;
            _stateChannel.Event += HandleStateChanged;
        }

        private void HandleStateChanged(BossStateEnum newState)
        {
            State = newState;
            OnStateChanged?.Invoke(newState);
        }

        private void OnDestroy()
        {
            if (_stateChannel != null)
                _stateChannel.Event -= HandleStateChanged;
        }

    }
}