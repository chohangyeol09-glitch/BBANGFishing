using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace NKT.Agent
{
    [RequireComponent(typeof(Animator))]
    public class AgentRenderer : MonoBehaviour, IRenderer, IModule
    {
        public Animator Animator { get; private set; }

        public void Initialize(ModuleOwner owner)
        {
            Animator = GetComponent<Animator>();
        }

        public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0)
        {
            Animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);
        }

    }
}