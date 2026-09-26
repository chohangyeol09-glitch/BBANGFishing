using CHG._02.Script.BossSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ResetGroggyAction", story: "[Boss] reset groggy", category: "Action", id: "62ad3198337382f3fc3e72e4bde7df30")]
public partial class ResetGroggyAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;

    protected override Status OnStart()
    {
        GroggyModule groggy = Boss.Value != null ? Boss.Value.GetModule<GroggyModule>() : null;
        if (groggy == null) return Status.Failure;
        
        groggy.ResetGauge();
        return Status.Running;
    }
}

