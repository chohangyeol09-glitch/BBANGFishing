using DevLib.ModuleSystem;
using Member.JJK._02._Scripts.Weapon;
using NKT.Fishing.Rob;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace NKT.Player.Modules
{
    public class WeaponModeModule : MonoBehaviour,IModule, IAfterInitModule
    {
        [SerializeField] private PlayerInputSO input;
        [SerializeField] private Rig fishingRig;
        [SerializeField] private Rig gunRig;

        public bool IsGunMode => _gunMode;

        private FishingModule _fishing;
        private WeaponEquipModule _equip;
        private RobEquipModule _robEquip;
        private FishingRob _storedRob;      //사격 중 낚시대를 잃어버리지 않게 들고 있는다
        private LookModule _look;

        private WeaponController _weapon;   //교체되므로 직렬화하지 않는다
        private AimModule _aim;

        private bool _gunMode;
        private bool _locked;
        private bool _attackHeld;

        public void Initialize(ModuleOwner owner)
        {
            _fishing = owner.GetModule<FishingModule>();
            _robEquip = owner.GetModule<RobEquipModule>();
            _look = owner.GetModule<LookModule>();
            _equip = owner.GetModule<WeaponEquipModule>();
        }

        public void AfterInit()
        {
            input.OnAttackPressed += HandleAttackPressed;
            input.OnAttackReleased += HandleAttackReleased;
            input.OnAimChanged += HandleAim;
            input.OnInputLocked += HandleLocked;
            input.OnInputUnlocked += HandleUnlocked;

            _equip.OnWeaponChanged += BindWeapon;

            fishingRig.weight = 1f;
            gunRig.weight = 0f;

            BindWeapon();       //처음 꽂혀있는 총도 잡아준다
        }

        private void OnDestroy()
        {
            if (_equip != null)
                _equip.OnWeaponChanged -= BindWeapon;

            if (input == null) return;

            input.OnAttackPressed -= HandleAttackPressed;
            input.OnAttackReleased -= HandleAttackReleased;
            input.OnAimChanged -= HandleAim;
            input.OnInputLocked -= HandleLocked;
            input.OnInputUnlocked -= HandleUnlocked;
        }

        private void BindWeapon()
        {
            _weapon = _equip.Current;
            _aim = _weapon != null ? _weapon.GetComponentInChildren<AimModule>(true) : null;

            _aim?.SetRecoilReceiver(_look);

            if (_weapon != null)
                _weapon.gameObject.SetActive(_gunMode);

            ApplyWeaponEnabled();
        }

        [ContextMenu("Test")]
        private void Test()
        {
            SetGunMode(true);
        }
        
        public void SetGunMode(bool on)
        {
            if (_gunMode == on) return;

            _gunMode = on;

            if (on)
            {
                _storedRob = _robEquip.Current;
                _robEquip.Unequip();            //안에서 CancelFishing까지 한다
            }
            else if (_storedRob != null)
            {
                _robEquip.Equip(_storedRob);
            }

            if (_weapon != null)
                _weapon.gameObject.SetActive(on);

            fishingRig.weight = on ? 0f : 1f;
            gunRig.weight = on ? 1f : 0f;

            ApplyWeaponEnabled();
        }

        private void Update()
        {
            if (!CanShoot() || !_attackHeld) return;
            if (!_weapon.WeaponData.IsAuto) return;

            _weapon.Fire();     //연사. FireRate로 알아서 걸러진다
        }

        private void HandleAttackPressed()
        {
            _attackHeld = true;

            if (!CanShoot()) return;
            if (_weapon.WeaponData.IsAuto) return;

            _weapon.Fire();     //단발
        }

        private void HandleAttackReleased() => _attackHeld = false;

        private void HandleAim(bool aiming)
        {
            if (!CanShoot()) return;

            _weapon.SetAiming(aiming);
        }

        private bool CanShoot() => _gunMode && !_locked && _weapon != null;

        private void HandleLocked()
        {
            _locked = true;
            _attackHeld = false;
            ApplyWeaponEnabled();
        }

        private void HandleUnlocked()
        {
            _locked = false;
            ApplyWeaponEnabled();
        }

        private void ApplyWeaponEnabled()
        {
            if (_weapon != null)
                _weapon.enabled = _gunMode && !_locked;
        }
    }
}
