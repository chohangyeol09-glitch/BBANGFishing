using System;
using DevLib.AnimatorSystem;
using DevLib.ModuleSystem;
using NKT.Agent;
using UnityEngine;

namespace NKT.Player.Modules
{
    public class FishingAnimationModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private HashDataSO idleAnim;
        [SerializeField] private HashDataSO chargeAnim;
        [SerializeField] private HashDataSO castAnim;
        [SerializeField] private HashDataSO retrieveAnim;
        [SerializeField] private HashDataSO reelAnim;

        private ModuleOwner _owner;
        private FishingModule _fishingModule;
        private AgentRenderer _renderer;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
        }

        public void AfterInit()
        {
            _fishingModule = _owner.GetModule<FishingModule>();
            _renderer = _owner.GetModule<AgentRenderer>();
        }
        
        private void Start()
        {
            _fishingModule.OnStateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            _fishingModule.OnStateChanged -= OnStateChanged;
        }
        
        private void OnStateChanged(FishingState state)
        {
            Debug.Log("OnStateChanged : " + state);
            switch (state)
            {
                case FishingState.Idle:
                    _renderer.PlayClip(idleAnim.HashValue, 0,0.3f, 1);
                    break;
                case FishingState.Charging:
                    _renderer.PlayClip(chargeAnim.HashValue, 0,0, 1);
                    break;
                case FishingState.Casting:
                    _renderer.PlayClip(castAnim.HashValue, 0,0, 1);
                    break;
                case FishingState.Retrieving:
                    _renderer.PlayClip(retrieveAnim.HashValue, 0,0.5f, 1);
                    break;
                case FishingState.Waiting:
                    _renderer.PlayClip(idleAnim.HashValue, 0,0.5f, 1);
                    break;
                case FishingState.Reeling:
                    _renderer.PlayClip(reelAnim.HashValue, 0,0, 1);
                    break;
            }
        }
    }
}