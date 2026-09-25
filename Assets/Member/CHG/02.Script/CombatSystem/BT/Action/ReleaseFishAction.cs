using CHG._02.Script.FishSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ReleaseFishAction", story: "[Fish] release to pool", category: "Action", id: "1ed5fbc4766bc8b67eb30a9915a346b9")]
public partial class ReleaseFishAction : Action
{
    [SerializeReference] public BlackboardVariable<Fish> Fish;

    protected override Status OnStart()
    {
        if (Fish.Value == null) return Status.Failure;
        
        Fish.Value.ReleaseToPool();
        return Status.Success;
    }
}

