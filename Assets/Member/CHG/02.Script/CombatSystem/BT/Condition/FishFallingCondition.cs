using CHG._02.Script.FishSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "FishFallingCondition", story: "[Fish] started falling", category: "Conditions", id: "bcf5a9ee5511cb98979b33dda96da57b")]
public partial class FishFallingCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Fish> Fish;

    public override bool IsTrue()
    {
        return Fish.Value != null && Fish.Value.HasStartedFalling;
    }
}
