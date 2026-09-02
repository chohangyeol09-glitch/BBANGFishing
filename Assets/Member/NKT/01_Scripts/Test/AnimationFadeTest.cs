using System;
using DevLib.AnimatorSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NKT.Test
{
    public class AnimationFadeTest : MonoBehaviour
    {
        [SerializeField] private HashDataSO idle;
        [SerializeField] private HashDataSO cast;
        [SerializeField] private Animator animator;
        
        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                animator.Play(idle.name);
            }
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                animator.Play(cast.name);
            }
        }
    }
}