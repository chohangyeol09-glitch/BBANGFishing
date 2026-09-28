using System;
using DevLib.ModuleSystem;
using NKT.Fishing.Rob;
using UnityEngine;

namespace NKT.Player.Modules
{
    //낚시대 장착, 비장착만 관리
    public class RobEquipModule : MonoBehaviour, IModule
    {
        [SerializeField] private Transform handSocket;
        [SerializeField] private Transform rightGrip;
        [SerializeField] private FishingRob _current;
        [SerializeField] private Bobber _currentBobber;

        public FishingRob Current => _current;

        //Hide()는 참조를 남겨두므로, 실제로 손에 들려 있는지는 활성 상태로 판단한다
        public bool IsEquip => _current != null && _current.gameObject.activeInHierarchy;
        public Bobber CurrentBobber => _currentBobber;
        
        public event Action OnRobChanged;

        private FishingModule _fishingModule;

        public void Initialize(ModuleOwner owner)
        {
            _fishingModule = owner.GetModule<FishingModule>();
        }

        //상점에서 낚시대 갈아낄때 쓰기
        public void Equip(FishingRobSO data)
        {
            if (data == null || data.prefab.RobGameobject == null) return;

            //같은 낚시대면 다시 만들지 않고 켜기만 한다. 핫바는 자주 눌린다
            if (_current != null && _current.Data == data)
            {
                _current.gameObject.SetActive(true);
                return;
            }

            _fishingModule.CancelFishing();
            DestroyCurrent();

            GameObject robObj = Instantiate(data.prefab.RobGameobject, handSocket, false);
            robObj.transform.localPosition = Vector3.zero;
            robObj.transform.localRotation = Quaternion.identity;
            _current = robObj.GetComponent<FishingRob>();

            if (data.prefab.BobberGameobject != null)
            {
                GameObject bobberObj = Instantiate(
                    data.prefab.BobberGameobject, _current.BobberTransform, false);
                _currentBobber = bobberObj.GetComponent<Bobber>();
            }

            SyncGrip();
            OnRobChanged?.Invoke();
        }

        //핫바에서 다른 슬롯으로 갔을 때. 파괴하지 않고 숨기기만 한다
        public void Hide()
        {
            if (_current == null) return;

            _fishingModule.CancelFishing();
            _current.gameObject.SetActive(false);
        }

        //낚시대 들때 이거 실행
        public void Equip(FishingRob fishing)
        {
            Unequip();

            fishing.gameObject.SetActive(true);
            fishing.transform.SetParent(handSocket);
            fishing.transform.localPosition = Vector3.zero;
            fishing.transform.localRotation = Quaternion.identity;

            _current = fishing;
        }

        //낚시대 집어넣을때 이거 실행
        public void Unequip()
        {
            if (_current == null) return;

            _fishingModule.CancelFishing();     //차징이나 대기 중이었으면 정리한다

            _current.gameObject.SetActive(false);
            _current = null;
        }

        private void SyncGrip()
        {
            if(rightGrip == null || _current == null || _current.GripPoint == null) return;
            
            rightGrip.localPosition = _current.GripPoint.localPosition;
            rightGrip.localRotation = _current.GripPoint.localRotation;
        }

        private void DestroyCurrent()
        {
            if(_currentBobber != null) Destroy(_currentBobber.gameObject);
            if(_current != null) Destroy(_current.gameObject);
            
            _currentBobber = null;
            _current = null;
        }
    }
}
