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

        public void Initialize(ModuleOwner owner) { }
        
        //낚시대 들때 이거 실행
       //여기서 차저에 구독 + 애니메이션 구독 하고
        public void Equip(FishingRob fishing)
        {
            Unequip();
            
            fishing.gameObject.SetActive(true);
            fishing.gameObject.transform.SetParent(handSocket);
            fishing.transform.localPosition = Vector3.zero;
            fishing.transform.localRotation = Quaternion.identity;
            
            _current = fishing;
            _current.OnEquip(fishing.Data);
        }

        //낚시대 집어넣을때 이거 실행
        public void Unequip()
        {
            if (_current == null) return;
            
            _current = null;
        }
    }
}