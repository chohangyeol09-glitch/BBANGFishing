using System;
using System.Collections;
using DevLib.ModuleSystem;
using NKT.Fishing;
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
        Retrieving, //물고기가 안 물었는데 다시 가져옴
        Biting,     //물고기가 묾
        Reeling,    //휠 감는중
    }

    //낚시 상태 관리, 낚시대 관리
    public class FishingModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private CastCharger charger;
        [SerializeField] private Bobber bobberObject;
        
        [SerializeField] private float orbitHeight = 3f;
        [SerializeField] private float flightTime = 1.2f;
        [SerializeField] private float returnTime = 0.8f;
        
        [SerializeField] private float waterTransformY;

        [Header("입질")]
        [SerializeField] private float biteWindow = 1f;

        public event Action<FishingState> OnStateChanged;
        public event Action<CastAim> OnAimUpdated;   //차징 중 궤도 미리보기용
        public FishingState State => _state;
        public Bobber Bobber => bobberObject;

        private FishingState _state = FishingState.Idle;
        private RobEquipModule _robEquip;
        private LookModule _lookModule;
        
        private Coroutine _stateRoutine;

        public void Initialize(ModuleOwner owner)
        {
            _robEquip = owner.GetModule<RobEquipModule>();
            _lookModule = owner.GetModule<LookModule>();
        }

        public void AfterInit()
        {
            charger.OnCharged += OnCharged;
            charger.OnValueChanged += OnChargeValueChanged;
            charger.OnChargeCanceled += OnChargeCanceled;
            
            bobberObject.OnLanded += ReportCastLanded;
            bobberObject.OnBite += ReportBite;
        }

        private void OnDestroy()
        {
            if (charger == null) return;

            charger.OnCharged -= OnCharged;
            charger.OnValueChanged -= OnChargeValueChanged;
            charger.OnChargeCanceled -= OnChargeCanceled;
            
            bobberObject.OnLanded -= ReportCastLanded;
            bobberObject.OnBite -= ReportBite;
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
                    ChangeState(FishingState.Retrieving);     //회수
                    break;
                case FishingState.Biting:
                    ChangeState(FishingState.Reeling);  //후킹
                    break;
            }
        }

        public void OnAttackReleased()
        {
            if (_state != FishingState.Charging) return;

            charger.ProgressEnd();  //동기적으로 OnCharged 가 불리고 거기서 상태가 바뀐다
        }

        //찌가 물에 닿았을때
        public void ReportCastLanded()
        {
            if (_state != FishingState.Casting) return;

            ChangeState(FishingState.Waiting);
        }

        //물고기가 안 물고 회수할때
        public void ReportRetrieveFinished()
        {
            if (_state != FishingState.Retrieving) return;

            ChangeState(FishingState.Idle);
        }
        
        //물고기가 물었을때
        public void ReportBite()
        {
            if (_state != FishingState.Waiting) return;

            ChangeState(FishingState.Biting);
        }

        //낚시대를 집어넣는 등 중간에 끊을때
        public void CancelFishing()
        {
            ChangeState(FishingState.Idle);
        }

        //=== 내부 ===

        private void OnCharged(float power)
        {
            CastAim aim = BuildAim(power);

            bobberObject.Launch(aim, flightTime);

            ChangeState(FishingState.Casting);
        }

        private void OnChargeValueChanged(float power)
        {
            if (_state != FishingState.Charging) return;

            OnAimUpdated?.Invoke(BuildAim(power));
        }

        private void OnChargeCanceled()
        {
            if (_state != FishingState.Charging) return;

            ChangeState(FishingState.Idle);
        }

        private CastAim BuildAim(float power)
        {
            Vector3 aim = _lookModule.CameraTransform.forward;
            aim.y = 0f;
            aim.Normalize();

            FishingRobSO data = _robEquip.Current.Data;
            float distance = Mathf.Lerp(data.minDistance, data.maxDistance, power);

            Vector3 origin = _robEquip.Current.BobberTransform.position;
            Vector3 landPoint = origin + aim * distance;
            landPoint.y = waterTransformY;

            return new CastAim
            {
                origin = origin,
                landPoint = landPoint,
                arcHeight = orbitHeight,
            };
        }

        private void ChangeState(FishingState state)
        {
            if (_state == state) return;

            FishingState prev = _state;
            _state = state;

            ExitState(prev);
            EnterState(state);

            OnStateChanged?.Invoke(_state);
        }

        private void EnterState(FishingState state)
        {
            switch (state)
            {
                case FishingState.Idle:
                    ClearBobber();
                    break;
                case FishingState.Retrieving:
                    ReturnBobber();
                    break;
                case FishingState.Biting:
                    _stateRoutine = StartCoroutine(BiteWindowRoutine());
                    break;
            }
        }

        private void ExitState(FishingState state)
        {
            if (_stateRoutine != null)
            {
                StopCoroutine(_stateRoutine);
                _stateRoutine = null;
            }

            if (state == FishingState.Charging)
                charger.ProgressCancel();
        }


        private void ReturnBobber()
        {
            if (bobberObject == null) return;
            
            bobberObject.Return(returnTime, orbitHeight);
        }
        private void ClearBobber()
        {
            if (bobberObject == null) return;
            
            bobberObject.PositionInit();

        }

        private IEnumerator BiteWindowRoutine()
        {
            yield return new WaitForSeconds(biteWindow);

            ChangeState(FishingState.Waiting);  //놓침
        }
    }
}
