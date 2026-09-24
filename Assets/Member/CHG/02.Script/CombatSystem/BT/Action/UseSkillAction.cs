using CHG._02.Script.Agents;
using System;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UseSkillAction", story: "[Agent] use skill to [Target]", category: "Action/Combat", id: "b65143f0dcccf30c7d031088886d0734")]
public partial class UseSkillAction : Action
{
    [SerializeReference] public BlackboardVariable<Agent> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private EnemySkillModule _skillModule;
    private bool _started;
    
    protected override Status OnStart()
    {
        _started = false;
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        if (Agent.Value is not ISkillEntrySource skillEntry)  return Status.Failure;
            
        SkillEntry[] entries = skillEntry.SkillEntries;
        if (entries == null || entries.Length == 0) return Status.Failure;

        _skillModule = Agent.Value.GetModule<EnemySkillModule>();
        if (_skillModule == null) return Status.Failure;
            
        SkillConditionContext context = new SkillConditionContext(Agent.Value, Target.Value);
        int? skillId = SelectSkillId(entries, context);
        if (!skillId.HasValue) return Status.Failure;

        _started = _skillModule.UseSkill(skillId.Value, Target.Value);
        return _started ? Status.Running : Status.Failure;
        
        
        return Status.Running;
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


