using Member.JJK._02._Scripts;
using Member.JJK._02._Scripts.Settings;
using Member.JJK._02._Scripts.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private MouseSensitivitySO sensitivity;
    [SerializeField] private AimModule aimModule;
    [SerializeField] private float verticalClamp = 80f;
    [SerializeField] private Transform playerBody;

    private float _xRotation = 0f;

    public void AddRecoil(Vector2 recoil)
    {
        _xRotation -= recoil.y;
        _xRotation = Mathf.Clamp(_xRotation, -verticalClamp, verticalClamp);

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * recoil.x);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (SettingsMenuUI.IsOpen) return;

        float currentSensitivity = aimModule != null && aimModule.IsAiming ? sensitivity.ZoomValue : sensitivity.Value;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * currentSensitivity;
        float mouseY = mouseDelta.y * currentSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
}
