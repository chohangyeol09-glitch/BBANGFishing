using System;
using CHG._02.Script.FishSystem;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.BT.Action
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "StartLungeAction", story: "[Fish] start lunge", category: "Action/Combat", id: "3ad43031266feae8d50ce2bced9c93ff")]
    public partial class StartLungeAction : Unity.Behavior.Action
    {
        [SerializeReference] public BlackboardVariable<Fish> Fish;

        protected override Status OnStart()
        {
            if (Fish.Value == null) return Status.Failure;
        
            LungeModule lunge = Fish.Value.GetModule<LungeModule>();
            if (lunge == null) return Status.Failure;
        
            Fish.Value.ConsumeSeaTouch();
            lunge.StartLunge();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return Fish.Value.State == FishStateEnum.Lunge || Fish.Value.State == FishStateEnum.Return
                ? Status.Running
                : Status.Success;
        }
    }
}

