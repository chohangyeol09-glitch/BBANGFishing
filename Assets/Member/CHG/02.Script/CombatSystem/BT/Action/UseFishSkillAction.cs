using CHG._02.Script.FishSystem;
using DevLib.AnimatorSystem;
using System;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UseFishSkillAction", story: "[Fish] use [Skill] to [Target]", category: "Action/Combat", id: "f02484647cdc7ad0b8e43a059e1b37c8")]
public partial class UseFishSkillAction : Action
{
    [SerializeReference] public BlackboardVariable<Fish> Fish;
    [SerializeReference] public BlackboardVariable<HashDataSO> Skill;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private EnemySkillModule _skillModule;
    
    protected override Status OnStart()
    {
        if (Fish.Value == null || Skill.Value == null) return Status.Failure;

        _skillModule = Fish.Value.GetModule<EnemySkillModule>();
        if (_skillModule == null) return Status.Failure;
        
        return _skillModule.UseSkill(Skill.Value.HashValue, Target.Value) ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return _skillModule.CurrentSkill == null ? Status.Success : Status.Running;
    }
}

