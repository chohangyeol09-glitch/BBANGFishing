using System;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using CHG._02.Script.FishSystem;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CHG._02.Script.CombatSystem.BT.Action
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "UseSkillFromDataAction", story: "[Fish] use skill from data to [Target]", category: "Action/Combat", id: "92f0b0a27c54ab6ab1b56125b79f9faa")]
    public partial class UseSkillFromDataAction : Unity.Behavior.Action
    {
        [SerializeReference] public BlackboardVariable<Fish> Fish;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private EnemySkillModule _skillModule;
        private bool _started;
        
        protected override Status OnStart()
        {
            _started = false;
            if (Fish.Value == null || Target.Value == null) return Status.Failure;
            
            SkillEntry[] entries = Fish.Value.Data.Skills;
            if (entries == null || entries.Length == 0) return Status.Failure;

            _skillModule = Fish.Value.GetModule<EnemySkillModule>();
            if (_skillModule == null) return Status.Failure;
            
            SkillConditionContext context = new SkillConditionContext(Fish.Value, Target.Value);
            int? skillId = SelectSkillId(entries, context);
            if (!skillId.HasValue) return Status.Failure;
            
            _skillModule = Fish.Value.GetModule<EnemySkillModule>();
            return _started ? Status.Running : Status.Success;
        }

        protected override Status OnUpdate()
        {
            return _skillModule.CurrentSkill == null ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            if (_started && _skillModule.CurrentSkill != null)
                _skillModule.CurrentSkill.StopSkill();
        }

        private int? SelectSkillId(SkillEntry[] entries, SkillConditionContext context)
        {
            int bestPriority = int.MinValue;
            int? chosen = null;
            int count = 0;

            foreach (SkillEntry entry in entries)
            {
                if (!IsUsable(entry, context)) continue;

                if (entry.Priority > bestPriority)
                {
                    bestPriority = entry.Priority;
                    chosen = entry.Skill.HashValue;
                    count = 1;
                }
                else if (entry.Priority == bestPriority)
                {
                    count++;
                    if (Random.Range(0, count) == 0)
                        chosen = entry.Skill.HashValue;
                }
            }
            return chosen;
        }

        private bool IsUsable(SkillEntry entry, SkillConditionContext context)
        {
            if (entry.Skill == null) return false;

            if (entry.Conditions != null)
            {
                foreach (AbstractSkillCondition condition in entry.Conditions)
                {
                    if (condition != null && !condition.IsCanUse(context)) 
                        return false;
                }
            }

            return _skillModule.CanUseSkill(entry.Skill.HashValue, context.Target);
        }
    }
}

