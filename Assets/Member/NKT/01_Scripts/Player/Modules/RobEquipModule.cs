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
        public bool IsEquip => _current != null;
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
            
            _fishingModule.CancelFishing();
            DestroyCurrent();
            
            GameObject robObj = Instantiate(data.prefab.RobGameobject, handSocket, false);
            robObj.transform.localScale = Vector3.one;
            robObj.transform.localRotation = Quaternion.identity;
            _current = robObj.GetComponent<FishingRob>();

            if (data.prefab.RobGameobject != null)
            {
                GameObject bobberObj = Instantiate(
                    data.prefab.RobGameobject, _current.BobberTransform, false);
                _currentBobber = bobberObj.GetComponent<Bobber>();
            }
            
            
        }

        private void DestroyCurrent()
        {
            
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
    }
}
