using CHG._02.Script.FishSystem;
using DevLib.AnimatorSystem;
using System;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CanUseFishSkillCondition", story: "[Fish] can uss [Skill] to [Target]", category: "Conditions", id: "3e48ec6e578a27c5b5ea2042d14f5b32")]
public partial class CanUseFishSkillCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Fish> Fish;
    [SerializeReference] public BlackboardVariable<HashDataSO> Skill;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    public override bool IsTrue()
    {
        if (Fish.Value == null || Skill.Value == null) return false;

        EnemySkillModule skillModule = Fish.Value.GetModule<EnemySkillModule>();
        return skillModule != null && skillModule.CanUseSkill(Skill.Value.HashValue, Target.Value);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
