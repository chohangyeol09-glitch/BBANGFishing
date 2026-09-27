using DevLib.AnimatorSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AnimationChannel", message: "Play [Anim]", category: "Events", id: "e4c98418109dd6a6a5ae413a1b53e791")]
public sealed partial class AnimationChannel : EventChannel<HashDataSO> { }

