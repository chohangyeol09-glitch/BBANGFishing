using System;
using DevLib.ModuleSystem;
using NKT.Fishing.Rob;
using UnityEngine;

namespace NKT.Player.Modules
{
    [RequireComponent(typeof(LineRenderer))]
    public class FishingLineModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private int pointCount = 12;
        [SerializeField] private float sagRatio = 0.08f;
        [SerializeField] private float castingSagRatio = 0.01f;

        private ModuleOwner _owner;
        private FishingModule _fishingModule;
        private RobEquipModule _robEquip;   //낚시대가 교체되므로 직렬화하지 않는다
        private LineRenderer _line;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;

            _line = GetComponent<LineRenderer>();
        }

        public void AfterInit()
        {
            _fishingModule = _owner.GetModule<FishingModule>();
            _robEquip = _owner.GetModule<RobEquipModule>();
        }
        
        private void Start()
        {
            _fishingModule.OnStateChanged += OnStateChanged;
            
            _line.enabled = false;
        }

        private void OnDestroy()
        {
            _fishingModule.OnStateChanged -= OnStateChanged;
        }

        private void LateUpdate()
        {
            if (_line.enabled == false) return;

            if(_fishingModule == null)
                _fishingModule = _owner.GetModule<FishingModule>();
            
            Bobber bobber = _fishingModule.Bobber;

            if (bobber == null) return;

            //낚시대는 교체될 때마다 새로 생기니 매번 현재 것을 가져온다
            FishingRob rob = _robEquip != null ? _robEquip.Current : null;

            if (rob == null || rob.RobEdgeTransform == null) return;

            Vector3 from = rob.RobEdgeTransform.position;
            Vector3 to = bobber.transform.position;
            
            bool isTaut = _fishingModule.State == FishingState.Casting
                        || _fishingModule.State == FishingState.Retrieving;
            
            float ratio = isTaut ? castingSagRatio : sagRatio;;
            
            float sag = Vector3.Distance(from, to) * ratio;
            
            _line.positionCount = pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(pointCount - 1);
                
                Vector3 pos = Vector3.Lerp(from, to, t);
                pos.y -= sag * 4f * t * (1f - t);
                
                _line.SetPosition(i, pos);
            }
        }

        private void OnStateChanged(FishingState state)
        {
            _line.enabled = state != FishingState.Idle && state != FishingState.Charging;
        }
    }
}