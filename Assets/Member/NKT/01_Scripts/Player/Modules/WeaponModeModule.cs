using DevLib.ModuleSystem;
using Member.JJK._02._Scripts.Weapon;
using NKT.Fishing.Rob;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace NKT.Player.Modules
{
    public class WeaponModeModule :
        MonoBehaviour,
        IModule,
        IAfterInitModule
    {
        [SerializeField]
        private PlayerInputSO input;

        [SerializeField]
        private Rig fishingRig;

        [SerializeField]
        private Rig gunRig;


        public bool IsGunMode =>
            _gunMode;


        private FishingModule _fishing;

        private WeaponEquipModule _equip;

        private RobEquipModule _robEquip;

        private LookModule _look;


        private WeaponController _weapon;

        private AimModule _aim;


        private bool _gunMode;

        private bool _locked;

        private bool _attackHeld;



        // =========================================
        // UI 열기 전 장비 기억
        // =========================================

        private enum EquipmentType
        {
            None,
            Rod,
            Gun
        }


        private EquipmentType _equipmentBeforeUI =
            EquipmentType.None;


        private bool _isEquipmentHiddenForUI;



        public void Initialize(
            ModuleOwner owner)
        {
            _fishing =
                owner.GetModule<FishingModule>();


            _robEquip =
                owner.GetModule<RobEquipModule>();


            _look =
                owner.GetModule<LookModule>();


            _equip =
                owner.GetModule<WeaponEquipModule>();
        }



        public void AfterInit()
        {
            input.OnAttackPressed +=
                HandleAttackPressed;

            input.OnAttackReleased +=
                HandleAttackReleased;

            input.OnAimChanged +=
                HandleAim;

            input.OnInputLocked +=
                HandleLocked;

            input.OnInputUnlocked +=
                HandleUnlocked;


            _equip.OnWeaponChanged +=
                BindWeapon;


            fishingRig.weight = 1f;

            gunRig.weight = 0f;


            BindWeapon();
        }



        private void OnDestroy()
        {
            if (_equip != null)
            {
                _equip.OnWeaponChanged -=
                    BindWeapon;
            }


            if (input == null)
                return;


            input.OnAttackPressed -=
                HandleAttackPressed;

            input.OnAttackReleased -=
                HandleAttackReleased;

            input.OnAimChanged -=
                HandleAim;

            input.OnInputLocked -=
                HandleLocked;

            input.OnInputUnlocked -=
                HandleUnlocked;
        }



        private void BindWeapon()
        {
            _weapon =
                _equip.Current;


            _aim =
                _weapon != null
                    ? _weapon
                        .GetComponentInChildren<AimModule>(true)
                    : null;


            _aim?.SetRecoilReceiver(
                _look
            );


            if (_weapon != null)
            {
                _weapon.gameObject
                    .SetActive(_gunMode);
            }


            ApplyWeaponEnabled();
        }



        // =========================================
        // 낚싯대 장착
        // =========================================

        public void EquipRod(
            FishingRobSO data)
        {
            _equip.Hide();


            _robEquip.Equip(
                data
            );


            ApplyMode(false);
        }



        // =========================================
        // 총 장착
        // =========================================

        public void EquipGun(
            WeaponSO data,
            DurabilityRuntimeState durability)
        {
            if (_fishing != null)
            {
                _fishing.CancelFishing();
            }


            _robEquip.Hide();


            _equip.Equip(
                data,
                durability
            );


            ApplyMode(true);
        }



        // =========================================
        // 완전히 아무것도 들지 않기
        //
        // 핫바 등에서 실제로 빈손을 선택할 때 사용
        // =========================================

        public void EquipNothing()
        {
            if (_fishing != null)
            {
                _fishing.CancelFishing();
            }


            _robEquip.Hide();

            _equip.Hide();


            _gunMode = false;


            HideAllRigs();

            ApplyWeaponEnabled();


            // 진짜 빈손으로 바꾼 것이므로
            // UI 복구 정보도 제거
            _equipmentBeforeUI =
                EquipmentType.None;

            _isEquipmentHiddenForUI =
                false;
        }



        // =========================================
        // UI 열기 전
        //
        // 현재 장비를 기억하고 잠시 숨김
        // =========================================

        public void HideEquipmentForUI()
        {
            // 이미 UI 때문에 숨겨진 상태면
            // 또 저장하지 않음
            if (_isEquipmentHiddenForUI)
                return;


            _isEquipmentHiddenForUI =
                true;


            // 현재 총을 들고 있는지 확인
            if (_gunMode &&
                _equip != null &&
                _equip.Current != null &&
                _equip.Current.gameObject.activeInHierarchy)
            {
                _equipmentBeforeUI =
                    EquipmentType.Gun;
            }

            // 현재 낚싯대를 들고 있는지 확인
            else if (_robEquip != null &&
                     _robEquip.IsEquip)
            {
                _equipmentBeforeUI =
                    EquipmentType.Rod;
            }

            else
            {
                _equipmentBeforeUI =
                    EquipmentType.None;
            }


            // 낚시 중이었다면 정리
            if (_fishing != null)
            {
                _fishing.CancelFishing();
            }


            // 실제 오브젝트는 파괴하지 않고 숨김
            if (_robEquip != null)
            {
                _robEquip.Hide();
            }


            if (_equip != null)
            {
                _equip.Hide();
            }


            _gunMode = false;


            // UI 보는 동안 손 Rig도 전부 제거
            HideAllRigs();


            ApplyWeaponEnabled();
        }



        // =========================================
        // UI 닫을 때
        //
        // UI 열기 전에 들던 장비 복구
        // =========================================

        public void RestoreEquipmentAfterUI()
        {
            if (!_isEquipmentHiddenForUI)
                return;


            _isEquipmentHiddenForUI =
                false;


            switch (_equipmentBeforeUI)
            {
                // =================================
                // 총 복구
                // =================================

                case EquipmentType.Gun:

                    if (_equip != null &&
                        _equip.Current != null)
                    {
                        _equip.Current.gameObject
                            .SetActive(true);


                        ApplyMode(true);
                    }

                    break;



                // =================================
                // 낚싯대 복구
                // =================================

                case EquipmentType.Rod:

                    if (_robEquip != null &&
                        _robEquip.Current != null)
                    {
                        _robEquip.Current.gameObject
                            .SetActive(true);


                        ApplyMode(false);
                    }

                    break;



                // =================================
                // 원래부터 빈손
                // =================================

                case EquipmentType.None:

                    _gunMode = false;

                    HideAllRigs();

                    ApplyWeaponEnabled();

                    break;
            }


            _equipmentBeforeUI =
                EquipmentType.None;
        }



        // =========================================
        // 총 / 낚싯대 모드 적용
        // =========================================

        private void ApplyMode(
            bool gun)
        {
            _gunMode =
                gun;


            // 현재 총 활성/비활성
            if (_weapon != null)
            {
                _weapon.gameObject
                    .SetActive(gun);
            }


            // 총이면 총 Rig
            // 낚싯대면 낚시 Rig
            if (fishingRig != null)
            {
                fishingRig.weight =
                    gun ? 0f : 1f;
            }


            if (gunRig != null)
            {
                gunRig.weight =
                    gun ? 1f : 0f;
            }


            ApplyWeaponEnabled();
        }



        private void HideAllRigs()
        {
            if (fishingRig != null)
            {
                fishingRig.weight = 0f;
            }


            if (gunRig != null)
            {
                gunRig.weight = 0f;
            }
        }



        private void Update()
        {
            if (!CanShoot() ||
                !_attackHeld)
                return;


            if (!_weapon.WeaponData.IsAuto)
                return;


            _weapon.Fire();
        }



        private void HandleAttackPressed()
        {
            _attackHeld = true;


            if (!CanShoot())
                return;


            if (_weapon.WeaponData.IsAuto)
                return;


            _weapon.Fire();
        }



        private void HandleAttackReleased()
        {
            _attackHeld = false;
        }



        private void HandleAim(
            bool aiming)
        {
            if (!CanShoot())
                return;


            _weapon.SetAiming(
                aiming
            );
        }



        private bool CanShoot()
        {
            return
                _gunMode &&
                !_locked &&
                _weapon != null;
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
            if (_weapon != null)
            {
                _weapon.enabled =
                    _gunMode &&
                    !_locked;
            }
        }
    }
}