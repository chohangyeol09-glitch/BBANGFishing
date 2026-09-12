using System;
using DevLib.ModuleSystem;
using NKT.Fishing;
using NKT.Fishing.Rob;
using UnityEngine;

namespace NKT.Player.Modules
{
    public class RobEquipModule : MonoBehaviour, IModule
    {
        [SerializeField] private CastCharger charger;
        [SerializeField] private RobParent _currentRob;
        
        private bool _isEquip => _currentRob != null;

        public void Initialize(ModuleOwner owner)
        {
            Debug.Assert(charger != null, "차저 어디감?");
            charger.OnCharged += OnPrimaryAction;
            charger.OnChargeStarted += ChargerAnimation;
        }

        private void OnDestroy()
        {
            charger.OnChargeStarted -= ChargerAnimation;
            charger.OnCharged -= OnPrimaryAction;
        }
        
        public void OnChargeStart()
        {
            if (!_isEquip) return;
            //애니메이션 실행, 뒤로 당기기
            charger.ProgressStart();
        }

        public void OnChargeEnd()
        {
            if (!_isEquip) return;
            //애니메이션 실행, 던지기
            charger.ProgressEnd();
        }

        //낚시대 들때 이거 실행
        public void Equip(RobParent parent)
        {//여기서 차저에 구독 + 애니메이션 구독 하고
            _currentRob = parent;
            _currentRob.OnEquip(parent.Data);
        }

        //낚시대 집어넣을때 이거 실행
        public void Unequip()
        {
            
            _currentRob = null;
        }

        private void OnPrimaryAction(float power)
        {//지금 낚시대 들고 있는 판단
            if (_currentRob == null) return;
            
            _currentRob?.OnPrimaryAction(power);
        }

        private void ChargerAnimation()
        {
            
        }
    }
}