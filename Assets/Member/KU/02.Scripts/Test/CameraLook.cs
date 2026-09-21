using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    private InputActionReference lookAction;


    [Header("설정")]
    [SerializeField]
    private float sensitivity = 0.1f;

    [SerializeField]
    private float maxVerticalAngle = 80f;


    private float yaw;
    private float pitch;

    private bool canLook = true;


    public bool CanLook => canLook;


    private void Start()
    {
        Vector3 rotation = transform.eulerAngles;

        yaw = rotation.y;
        pitch = rotation.x;

        if (pitch > 180f)
            pitch -= 360f;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void OnEnable()
    {
        lookAction?.action.Enable();
    }


    private void OnDisable()
    {
        lookAction?.action.Disable();
    }


    private void Update()
    {
        if (!canLook)
            return;

        Look();
    }


    private void Look()
    {
        if (lookAction == null)
            return;


        Vector2 input =
            lookAction.action.ReadValue<Vector2>();


        yaw += input.x * sensitivity;
        pitch -= input.y * sensitivity;


        pitch = Mathf.Clamp(
            pitch,
            -maxVerticalAngle,
            maxVerticalAngle
        );


        transform.rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );
    }


    // 외부에서 카메라 회전 가능 여부 설정
    public void SetLookEnabled(bool value)
    {
        canLook = value;
    }


    // 화면 회전 정지
    public void LockLook()
    {
        canLook = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }


    // 화면 회전 다시 활성화
    public void UnlockLook()
    {
        canLook = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}