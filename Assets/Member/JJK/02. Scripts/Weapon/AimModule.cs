using DevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.JJK._02._Scripts.Weapon
{
    public class AimModule : MonoBehaviour, IModule
    {
        [SerializeField] private MouseSensitivitySO sensitivity;
        [SerializeField] private float tiltAmount = 8f;
        [SerializeField] private float maxTilt = 15f;
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float aimFov = 40f;
        [SerializeField] private float defaultFov = 60f;
        [SerializeField] private float fovLerpSpeed = 10f;
        [SerializeField] private Vector3 hipPosition;
        [SerializeField] private Vector3 adsPosition;
        [SerializeField] private float adsPositionSpeed = 10f;
        [SerializeField] private MouseLook mouseLook;

        [Header("Weapon Visual Recoil")]
        [SerializeField] private float weaponKickBack = 0.05f;
        [SerializeField] private float weaponKickUp = 0.02f;
        [SerializeField] private float weaponRotRecoil = 5f;
        [SerializeField] private float weaponRecoilSnapSpeed = 15f;
        [SerializeField] private float weaponRecoilRecovery = 8f;

        private WeaponSO _weaponData;
        private Vector3 _baseLocalRotation;
        private float _currentTilt;
        private float _targetTilt;
        private float _tiltVelocity;
        private bool _isAiming;
        private ModuleOwner _owner;

        private Vector3 _weaponPosRecoilCurrent;
        private Vector3 _weaponPosRecoilTarget;
        private float _weaponRotRecoilCurrent;
        private float _weaponRotRecoilTarget;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _weaponData = owner.GetComponent<WeaponController>().WeaponData;
            hipPosition = _owner.transform.localPosition;
        }

        public void ApplyRecoil(Vector2 recoil)
        {
            mouseLook.AddRecoil(recoil);
            _weaponPosRecoilTarget += new Vector3(0f, weaponKickUp, -weaponKickBack);
            _weaponRotRecoilTarget += weaponRotRecoil;
        }

        public void SetAiming(bool isAiming) => _isAiming = isAiming;

        private void WeaponTilt()
        {
            float mouseX = Mouse.current.delta.ReadValue().x * sensitivity.Value;

            _targetTilt = Mathf.Clamp(-mouseX * tiltAmount, -maxTilt, maxTilt);
            _currentTilt = Mathf.SmoothDamp(_currentTilt, _targetTilt, ref _tiltVelocity, smoothTime);

            _owner.transform.localRotation = Quaternion.Euler(
                _baseLocalRotation.x - _weaponRotRecoilCurrent,
                _baseLocalRotation.y,
                _baseLocalRotation.z + _currentTilt
            );
        }

        private void Update()
        {
            _weaponPosRecoilCurrent = Vector3.Lerp(_weaponPosRecoilCurrent, _weaponPosRecoilTarget, weaponRecoilSnapSpeed * Time.deltaTime);
            _weaponPosRecoilTarget = Vector3.Lerp(_weaponPosRecoilTarget, Vector3.zero, weaponRecoilRecovery * Time.deltaTime);

            _weaponRotRecoilCurrent = Mathf.Lerp(_weaponRotRecoilCurrent, _weaponRotRecoilTarget, weaponRecoilSnapSpeed * Time.deltaTime);
            _weaponRotRecoilTarget = Mathf.Lerp(_weaponRotRecoilTarget, 0f, weaponRecoilRecovery * Time.deltaTime);

            float targetFov = _isAiming ? aimFov : defaultFov;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);

            Vector3 basePos = _isAiming ? adsPosition : hipPosition;
            _owner.transform.localPosition = Vector3.Lerp(_owner.transform.localPosition, basePos + _weaponPosRecoilCurrent, adsPositionSpeed * Time.deltaTime);

            WeaponTilt();
        }
    }
}
