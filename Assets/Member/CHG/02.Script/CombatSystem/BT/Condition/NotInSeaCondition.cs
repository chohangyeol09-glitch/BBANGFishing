using CHG._02.Script.FishSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NotInSeaCondition", story: "[Fish] is not in sea", category: "Conditions", id: "50ab2ab5a4e7d4c72abecd327df995e3")]
public partial class NotInSeaCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Fish> Fish;

    public override bool IsTrue()
    {
        return Fish.Value != null && !Fish.Value.IsInSea;
    }
}
