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
        [SerializeField] private WeaponController weapon;
        [SerializeField] private AimModule aimModule;
        [SerializeField] private GameObject gunObject;
        [SerializeField] private Rig fishingRig;
        [SerializeField] private Rig gunRig;

        public bool IsGunMode => _gunMode;

        private FishingModule _fishing;
        private RobEquipModule _robEquip;
        private FishingRob _storedRob;      //사격 중 낚시대를 잃어버리지 않게 들고 있는다
        private LookModule _look;

        private bool _gunMode;
        private bool _locked;
        private bool _attackHeld;

        public void Initialize(ModuleOwner owner)
        {
            _fishing = owner.GetModule<FishingModule>();
            _robEquip = owner.GetModule<RobEquipModule>();
            _look = owner.GetModule<LookModule>();
        }

        public void AfterInit()
        {
            input.OnAttackPressed += HandleAttackPressed;
            input.OnAttackReleased += HandleAttackReleased;
            input.OnAimChanged += HandleAim;
            input.OnInputLocked += HandleLocked;
            input.OnInputUnlocked += HandleUnlocked;

            gunObject.SetActive(false);
            fishingRig.weight = 1f;
            gunRig.weight = 0f;
            ApplyWeaponEnabled();
            
            aimModule.SetRecoilReceiver(_look);
        }

        private void OnDestroy()
        {
            if (input == null) return;

            input.OnAttackPressed -= HandleAttackPressed;
            input.OnAttackReleased -= HandleAttackReleased;
            input.OnAimChanged -= HandleAim;
            input.OnInputLocked -= HandleLocked;
            input.OnInputUnlocked -= HandleUnlocked;
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

            gunObject.SetActive(on);
            fishingRig.weight = on ? 0f : 1f;
            gunRig.weight = on ? 1f : 0f;

            ApplyWeaponEnabled();
        }

        private void Update()
        {
            if (!_gunMode || _locked || !_attackHeld) return;
            if (!weapon.WeaponData.IsAuto) return;

            weapon.Fire();      //연사. FireRate로 알아서 걸러진다
        }

        private void HandleAttackPressed()
        {
            _attackHeld = true;

            if (!_gunMode || _locked) return;
            if (weapon.WeaponData.IsAuto) return;

            weapon.Fire();      //단발
        }

        private void HandleAttackReleased() => _attackHeld = false;

        private void HandleAim(bool aiming)
        {
            if (!_gunMode || _locked) return;
            weapon.SetAiming(aiming);
        }

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
            if (weapon != null)
                weapon.enabled = _gunMode && !_locked;
        }
    }
}