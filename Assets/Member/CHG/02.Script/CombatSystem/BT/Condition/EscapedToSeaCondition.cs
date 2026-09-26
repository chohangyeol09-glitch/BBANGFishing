using CHG._02.Script.FishSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EscapedToSeaCondition", story: "[Fish] escaped to sea", category: "Conditions", id: "fd399a8126c688daeb21ad0386127a80")]
public partial class EscapedToSeaCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Fish> Fish;

    public override bool IsTrue()
    {
        if (Fish.Value == null || !Fish.Value.IsInSea) return false;
        
        //런지를 못하거나 이미 런지를 했을 때만
        LungeModule lunge = Fish.Value.GetModule<LungeModule>();
        return !Fish.Value.Data.CanLunge || lunge == null || lunge.HasLunged;
    }
}
