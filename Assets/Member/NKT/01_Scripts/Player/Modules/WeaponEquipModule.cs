using System;
using DevLib.ModuleSystem;
using Member.JJK._02._Scripts.Weapon;
using NKT.Fishing.Rob;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace NKT.Player.Modules
{
    public class WeaponEquipModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private Transform gunSocket;
        [SerializeField] private RigBuilder rigBuilder;
        [SerializeField] private TwoBoneIKConstraint rightHandIK;
        [SerializeField] private TwoBoneIKConstraint leftHandIK;
        [SerializeField] private WeaponController _current;

        public WeaponController Current => _current;

        //Hide()는 참조를 남겨두므로, 실제로 손에 들려 있는지는 활성 상태로 판단한다
        public bool IsEquip => _current != null && _current.gameObject.activeInHierarchy;

        public event Action OnWeaponChanged;
        public event Action<WeaponSO> OnWeaponBroken;

        public void Initialize(ModuleOwner owner) { }

        public void Equip(WeaponSO data, DurabilityRuntimeState durability = null)
        {
            if (data == null || data.Prefab == null) return;

            //같은 총이면 다시 만들지 않고 켜기만 한다. 재생성은 RigBuilder.Build까지 부른다
            if (_current != null && _current.WeaponData == data)
            {
                _current.gameObject.SetActive(true);
                return;
            }

            DestroyCurrent();

            GameObject obj = Instantiate(data.Prefab, gunSocket, false);
            _current = obj.GetComponent<WeaponController>();

            if (durability != null)
                _current.SetDurabilityState(durability);

            _current.OnBroken += HandleBroken;

            SyncGrip();
            OnWeaponChanged?.Invoke();
        }

        public void Unequip()
        {
            DestroyCurrent();
            OnWeaponChanged?.Invoke();
        }

        //핫바에서 다른 슬롯으로 갔을 때. 파괴하지 않고 숨기기만 한다
        public void Hide()
        {
            if (_current == null) return;

            _current.gameObject.SetActive(false);
        }

        private void HandleBroken(WeaponController weapon)
        {
            weapon.OnBroken -= HandleBroken;

            WeaponSO data = weapon.WeaponData;
            if (_current == weapon) _current = null;

            OnWeaponBroken?.Invoke(data);
            OnWeaponChanged?.Invoke();
        }

        private void DestroyCurrent()
        {
            if (_current == null) return;

            _current.OnBroken -= HandleBroken;
            Destroy(_current.gameObject);
            _current = null;
        }

        private void SyncGrip()
        {
            WeaponGrip grip = _current.GetComponent<WeaponGrip>();
            if (grip == null)
            {
                Debug.LogWarning($"{_current.name} 루트에 WeaponGrip이 없어서 손 IK를 못 맞춤");
                return;
            }

            rightHandIK.data.target = grip.RightGrip;
            leftHandIK.data.target = grip.LeftGrip;

            rigBuilder.Build();
        }

        public void AfterInit()
        {
            if (_current != null)
                _current.OnBroken += HandleBroken;
        }
    }
}