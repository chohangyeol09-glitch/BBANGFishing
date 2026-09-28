using System;
using CHG._02.Script.CoreSystem;
using DevLib.ModuleSystem;
using NKT.Fishing.Rob;
using UnityEngine;

namespace NKT.Player.Modules
{
    // 낚시대 장착, 비장착만 관리
    public class RobEquipModule : MonoBehaviour, IModule
    {
        [SerializeField]
        private Transform handSocket;

        [SerializeField]
        private Transform rightGrip;

        [SerializeField]
        private FishingRob _current;

        [SerializeField]
        private Bobber _currentBobber;

        // 찌는 장착할 때 생성되므로 인스펙터에서 직접 연결할 수 없다.
        // 장착/해제 때마다 이 FollowTarget의 대상을 현재 찌로 바꿔 준다
        [SerializeField]
        private FollowTarget bobberFollower;


        public FishingRob Current =>
            _current;

        // Hide()는 참조를 남겨두므로,
        // 실제로 손에 들려 있는지는 활성 상태로 판단
        public bool IsEquip =>
            _current != null &&
            _current.gameObject.activeInHierarchy;

        public Bobber CurrentBobber =>
            _currentBobber;


        public event Action OnRobChanged;


        private FishingModule _fishingModule;
        private GameObject _currentRoot;



        public void Initialize(ModuleOwner owner)
        {
            _fishingModule =
                owner.GetModule<FishingModule>();


            if (_fishingModule == null)
            {
                Debug.LogWarning(
                    "RobEquipModule : FishingModule을 찾지 못했습니다.",
                    this
                );
            }


            // 인스펙터에 미리 들어 있는 찌가 있으면 처음부터 따라가게 한다
            SyncBobberFollower();
        }



        // =========================================
        // 상점에서 낚시대 구매 / 교체
        // =========================================

        public void Equip(FishingRobSO data)
        {
            TryEquip(data);
        }

        public bool TryEquip(FishingRobSO data)
        {
            if (data == null || data.prefab.RobGameobject == null || handSocket == null)
            {
                Debug.LogError("RobEquipModule : SO, 낚싯대 프리팹, Hand Socket 연결을 확인하세요.", this);
                return false;
            }

            if (_current != null && _current.Data == data)
            {
                _current.gameObject.SetActive(true);
                return true;
            }

            // 기존 장비를 제거하기 전에 새 프리팹의 필수 연결을 검증한다.
            FishingRob template = data.prefab.RobGameobject.GetComponentInChildren<FishingRob>(true);
            string rodLabel = string.IsNullOrWhiteSpace(data.rodName) ? data.name : data.rodName;
            if (template == null)
            {
                Debug.LogError($"{rodLabel} : RobGameobject '{data.prefab.RobGameobject.name}'에 FishingRob 컴포넌트가 없습니다.", data);
                return false;
            }
            if (template.RobEdgeTransform == null)
            {
                Debug.LogError($"{rodLabel} : FishingRob의 Rob Edge Transform이 연결되어 있지 않습니다.", data);
                return false;
            }
            if (data.prefab.BobberGameobject != null && template.BobberTransform == null)
            {
                Debug.LogError($"{rodLabel} : FishingRob의 Bobber Transform이 연결되어 있지 않습니다.", data);
                return false;
            }
            if (data.prefab.BobberGameobject != null &&
                data.prefab.BobberGameobject.GetComponentInChildren<Bobber>(true) == null)
            {
                Debug.LogError($"{rodLabel} : BobberGameobject '{data.prefab.BobberGameobject.name}'에 Bobber 컴포넌트가 없습니다. 동작용 Bobber 프리팹을 연결하세요.", data);
                return false;
            }

            GameObject robObj = Instantiate(data.prefab.RobGameobject, handSocket, false);
            robObj.transform.localPosition = Vector3.zero;
            robObj.transform.localRotation = Quaternion.identity;
            FishingRob next = robObj.GetComponentInChildren<FishingRob>(true);
            next.ApplyData(data);

            Bobber nextBobber = null;
            if (data.prefab.BobberGameobject != null)
            {
                GameObject bobberObj = Instantiate(data.prefab.BobberGameobject, next.BobberTransform, false);
                nextBobber = bobberObj.GetComponentInChildren<Bobber>(true);
            }

            if (_fishingModule != null)
                _fishingModule.CancelFishing();

            DestroyCurrent();
            _current = next;
            _currentRoot = robObj;
            _currentBobber = nextBobber;
            robObj.SetActive(true);
            _current.gameObject.SetActive(true);
            SyncGrip();
            SyncBobberFollower();
            OnRobChanged?.Invoke();
            Debug.Log($"{data.rodName} 장착 완료");
            return true;
        }

        // =========================================
        // 핫바에서 다른 슬롯으로 갔을 때
        // =========================================

        public void Hide()
        {
            if (_current == null)
                return;


            if (_fishingModule != null)
            {
                _fishingModule.CancelFishing();
            }


            _current.gameObject.SetActive(false);
        }



        // =========================================
        // 기존 FishingRob 직접 장착
        // =========================================

        public void Equip(FishingRob fishing)
        {
            if (fishing == null)
                return;


            Unequip();


            fishing.gameObject.SetActive(true);

            fishing.transform.SetParent(
                handSocket
            );

            fishing.transform.localPosition =
                Vector3.zero;

            fishing.transform.localRotation =
                Quaternion.identity;


            _current =
                fishing;
            _currentRoot = fishing.gameObject;
        }



        // =========================================
        // 낚시대 집어넣기
        // =========================================

        public void Unequip()
        {
            if (_current == null)
                return;


            if (_fishingModule != null)
            {
                _fishingModule.CancelFishing();
            }


            _current.gameObject.SetActive(false);

            _current =
                null;
        }



        private void SyncGrip()
        {
            if (rightGrip == null ||
                _current == null ||
                _current.GripPoint == null)
            {
                return;
            }


            rightGrip.localPosition =
                _current.GripPoint.localPosition;

            rightGrip.localRotation =
                _current.GripPoint.localRotation;
        }



        private void SyncBobberFollower()
        {
            if (bobberFollower == null)
                return;


            bobberFollower.SetTarget(
                _currentBobber != null ? _currentBobber.transform : null
            );
        }



        private void DestroyCurrent()
        {
            if (_currentBobber != null)
            {
                Destroy(
                    _currentBobber.gameObject
                );
            }


            if (_current != null)
            {
                _current.gameObject.SetActive(false);
                Destroy(
                    _currentRoot != null ? _currentRoot : _current.gameObject
                );
            }


            _currentRoot = null;
            _currentBobber =
                null;

            _current =
                null;

            SyncBobberFollower();
        }
    }
}
