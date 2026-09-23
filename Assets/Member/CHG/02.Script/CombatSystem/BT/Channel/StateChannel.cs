using System;
using CHG._02.Script.FishSystem;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.BT.Channel
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/StateChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "StateChannel", message: "Change [State]", category: "Events", id: "5ab69f5f604bd86905262b95d062a20a")]
    public sealed partial class StateChannel : EventChannel<FishStateEnum> { }
}

