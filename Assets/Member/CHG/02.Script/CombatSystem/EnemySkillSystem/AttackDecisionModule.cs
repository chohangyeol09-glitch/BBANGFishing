using System;
using CHG._02.Script.Agents;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public class AttackDecisionModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private ScriptableObject initialStrategy;
        [SerializeField] private float startDelay;

        private float _startTime;

        public GameObject Target { get; set; }

        private Agent _owner;
        private EnemySkillModule _skillModule;
        private ISkillStrategy _strategy;


        public void Initialize(ModuleOwner owner)
        {

            _owner = owner as Agent;
            _skillModule = owner.GetModule<EnemySkillModule>();
            _strategy = initialStrategy as ISkillStrategy;
            if (_strategy == null) 
                Debug.LogError($"{name}: initialStrategy가 없거나 ISkillStrategy가 아닙니다.");
        }

        public void AfterInit()
        {
            _skillModule.OnSkillEnd += Evaluate;
            _startTime = Time.time;
        }

        private void Update()
        {
            if (_skillModule.CurrentSkill == null) Evaluate();
        }

        public void SetStrategy(ISkillStrategy strategy) => _strategy = strategy;

        private void Evaluate()
        {
            if (Time.time - _startTime < startDelay) return;
            if (_strategy == null || Target == null || _owner.IsDead) return;

            var context = new SkillStrategyContext(_skillModule, _owner, Target);
            int? skillId = _strategy.SelectNextSkillId(context);
            if (skillId.HasValue) _skillModule.UseSkill(skillId.Value, Target);
        }
    }
}