using System;
using DevLib.ModuleSystem;
using Member.JJK._02._Scripts.Weapon;
using NKT.Fishing.Rob;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace NKT.Player.Modules
{
    public class WeaponEquipModule : MonoBehaviour
    {
        [SerializeField] private Transform gunSocket;
        [SerializeField] private RigBuilder rigBuilder;
        [SerializeField] private TwoBoneIKConstraint rightHandIK;
        [SerializeField] private TwoBoneIKConstraint leftHandIK;
        [SerializeField] private WeaponController _current;

        public WeaponController Current => _current;
        public bool IsEquip => _current != null;

        public event Action OnWeaponChanged;
        public event Action<WeaponSO> OnWeaponBroken;

        public void Initialize(ModuleOwner owner) { }

        public void Equip(WeaponSO data, DurabilityRuntimeState durability = null)
        {
            if (data == null || data.Prefab == null) return;

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
            if (grip == null) return;

            rightHandIK.data.target = grip.RightGrip;
            leftHandIK.data.target = grip.LeftGrip;

            rigBuilder.Build();
        }
    }
}