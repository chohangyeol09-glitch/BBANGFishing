using CHG._02.Script.FishSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CanLungeCondition", story: "[Fish] Can lunge", category: "Conditions", id: "87b596c6143fd64bb4ab5b6f98d84e8b")]
public partial class CanLungeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Fish> Fish;

    public override bool IsTrue()
    {
        if (Fish.Value == null) return false;
        LungeModule lunge = Fish.Value.GetModule<LungeModule>();
        return Fish.Value.IsInSea && Fish.Value.Data.CanLunge && lunge != null && !lunge.HasLunged;
    }
}
