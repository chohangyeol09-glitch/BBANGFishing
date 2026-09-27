using System;
using DevLib.AnimatorSystem;
using DevLib.ModuleSystem;
using Unity.Behavior;
using UnityEngine;

namespace CHG._02.Script.Agents
{
    [RequireComponent(typeof(Animator))]
    public class EnemyRenderer : MonoBehaviour, IModule
    {
        [SerializeField] private HashDataSO idleHash;
        [SerializeField] private float crossFadeDuration = 0.1f;
        
        public Animator Animator { get; private set; }

        private AnimationChannel _channel;

        public void Initialize(ModuleOwner owner)
        {
            Animator = GetComponent<Animator>();
        }

        public void BindChannel(BehaviorGraphAgent btAgent)
        {
            if (_channel == null)
            {
                if (!btAgent.GetVariable("AnimationChannel", out BlackboardVariable<AnimationChannel> channel) || channel.Value == null)
                {
                    Debug.LogWarning("Animation Channel not found");
                    return;
                }
                _channel = channel.Value;
            }

            _channel.Event -= PlayAnim;
            _channel.Event += PlayAnim;
        }

        public void SendAnim(HashDataSO anim)
        {
            if (anim != null && _channel != null) _channel.SendEventMessage(anim);
        }

        private void PlayAnim(HashDataSO anim)
        {
            if (anim == null) return;
            Animator.CrossFadeInFixedTime(anim.HashValue, crossFadeDuration);
        }
        
        public void PlayIdle() => SendAnim(idleHash);

        private void OnDestroy()
        {
            if (_channel != null) _channel.Event -= PlayAnim;
        }
    }
}