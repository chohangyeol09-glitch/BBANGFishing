using System;
using DevLib.ModuleSystem;
using NKT.Fishing.Rob;
using UnityEngine;

namespace NKT.Player.Modules
{
    public enum FishingState
    {
        Idle,
        Charging,   //차징 중
        Casting,    //던지는 중
        Waiting,    //입질 기다리는중
        Biting,     //물고기가 묾
        Reeling     //휠 감는중
    }
    //낚시 상태 관리, 낚시대 관리
    public class FishingModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private CastCharger charger;
        
        public event Action<FishingState> OnStateChanged;
        public FishingState State => _state;
        
        private FishingState _state = FishingState.Idle;
        private RobEquipModule _robEquip;
        
        public void Initialize(ModuleOwner owner)
        {
            _robEquip = owner.GetModule<RobEquipModule>();
        }

        public void AfterInit()
        {
            charger.OnCharged += OnCharged;
        }

        public void OnAttackPressed()
        {
            switch (_state)
            {
                case FishingState.Idle:
                    if (!_robEquip.IsEquip) return;
                    charger.ProgressStart();
                    ChangeState(FishingState.Charging);
                    break;
                case FishingState.Waiting:
                    ChangeState(FishingState.Idle);
                    break;
                case FishingState.Biting:
                    ChangeState(FishingState.Reeling);
                    break;
            }
        }

        public void OnAttackReleased()
        {
            if (_state != FishingState.Charging) return;
            
            charger.ProgressEnd();
        }

        private void OnCharged(float power)
        {
            _robEquip.Current.Cast(power);
            ChangeState(FishingState.Charging);
        }

        private void ChangeState(FishingState state)
        {
            if (_state == state) return;
            
            _state = state;
            OnStateChanged?.Invoke(_state);
        }
    }
}