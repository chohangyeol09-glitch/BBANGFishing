using System;
using System.Collections;
using CHG._02.Script.Agents;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using Unity.Behavior;
using UnityEngine;

namespace CHG._02.Script.BossSystem
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Boss : Agent, ISkillEntrySource
    {
        public event Action<BossStateEnum> OnStateChanged;

        public BossStateEnum State =>
            BTAgent.GetVariable("State", out BlackboardVariable<BossStateEnum> state)
                ? state.Value : BossStateEnum.Appear;
        public BehaviorGraphAgent BTAgent { get; private set; }

        public override float MaxHealth => Data.Health;
        public SkillEntry[] SkillEntries => Data.Skills;
        
        [field: SerializeField] public BossDataSO Data { get; private set; }

        protected override void InitializeModules()
        {
            
            BTAgent = GetComponent<BehaviorGraphAgent>();
            base.InitializeModules();
        }

        public void OnSpawn(GameObject target)
        {
            CurrentHealth = MaxHealth;
            BTAgent.SetVariableValue("Self", this);
            BTAgent.SetVariableValue("Target", target);
            BTAgent.SetVariableValue("State", BossStateEnum.Appear);
            BTAgent.SetVariableValue("AppearDuration", Data.AppearDuration);
            BTAgent.SetVariableValue("GroggyDuration", Data.GroggyDuration);
        }
    }
}