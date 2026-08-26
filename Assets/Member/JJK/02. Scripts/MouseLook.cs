using Member.JJK._02._Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private MouseSensitivitySO sensitivity;
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
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * sensitivity.Value;
        float mouseY = mouseDelta.y * sensitivity.Value;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
}
