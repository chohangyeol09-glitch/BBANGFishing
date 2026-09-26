using CHG._02.Script.BossSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/BossStateChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "BossStateChannel", message: "Change [State]", category: "Events", id: "86076a1dad5ecb9e3ad27c32db0b90cc")]
public sealed partial class BossStateChannel : EventChannel<BossStateEnum> { }

