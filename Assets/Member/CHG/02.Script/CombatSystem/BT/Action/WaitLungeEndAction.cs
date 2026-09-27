using CHG._02.Script.FishSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitLungeEndAction", story: "[Fish] wait lunge end", category: "Action/Combat", id: "4f2669f3688b8f6deb31f355d7ac0d77")]
public partial class WaitLungeEndAction : Action
{
    [SerializeReference] public BlackboardVariable<Fish> Fish;

    private LungeModule _lunge;
    protected override Status OnStart()
    {
        if (Fish.Value == null) return Status.Failure;
        _lunge = Fish.Value.GetModule<LungeModule>();
        
        return _lunge != null && _lunge.IsFlying ? Status.Running : Status.Success;
    }

    protected override Status OnUpdate() => _lunge.IsFlying ? Status.Running : Status.Success;
}

