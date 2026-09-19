using DevLib.ModuleSystem;
using NKT.Fishing.Rob;
using UnityEngine;

namespace NKT.Player.Modules
{
    //낚시대 장착, 비장착만 관리
    public class RobEquipModule : MonoBehaviour, IModule
    {
        [SerializeField] private Transform handSocket;
        [SerializeField] private FishingRob _current;

        public FishingRob Current => _current;
        public bool IsEquip => _current != null;

        private FishingModule _fishingModule;

        public void Initialize(ModuleOwner owner)
        {
            _fishingModule = owner.GetModule<FishingModule>();
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
