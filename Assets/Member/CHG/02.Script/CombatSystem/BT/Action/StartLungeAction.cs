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

        private LungeModule _lunge;
        protected override Status OnStart()
        {
            if (Fish.Value == null) return Status.Failure;
            _lunge = Fish.Value.GetModule<LungeModule>();
        
            if (_lunge == null) return Status.Failure;
        
            Fish.Value.ConsumeSeaTouch();
            _lunge.StartLunge();
            return _lunge.IsApproaching ? Status.Running : Status.Failure;
        }

        protected override Status OnUpdate() => _lunge.IsApproaching ? Status.Running : Status.Success;
    }
}

