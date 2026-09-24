using UnityEngine;
using UnityEngine.InputSystem;

public class FisihInventoryAction : InteractionAction
{
    private CameraLook cameraLook;

    [Header("Fish Inventory UI")]
    [SerializeField]
    private GameObject fishInventoryUI;


    [Header("Fish Inventory Manager")]
    [SerializeField]
    private FishInventoryManager fishInventoryManager;

    private bool isOpened = false;

    private void Awake()
    {
        Camera mainCamera = Camera.main;


        if (mainCamera != null)
        {
            cameraLook =
                mainCamera.GetComponent<CameraLook>();
        }


        if (fishInventoryUI != null)
        {
            fishInventoryUI.SetActive(false);
        }
    }


    private void Update()
    {
        if (!isOpened)
            return;


        if (Keyboard.current == null)
            return;


        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInventory();
        }
    }


    public override void Execute()
    {
        if (isOpened)
            return;


        OpenInventory();
    }


    private void OpenInventory()
    {
        isOpened = true;


        // 카메라 회전 정지
        if (cameraLook != null)
        {
            cameraLook.LockLook();
        }


        // 인벤토리 UI 켜기
        if (fishInventoryUI != null)
        {
            fishInventoryUI.SetActive(true);
        }


        // 혹시 이전 상태가 남아있을 수 있으니
        // 상세창 / 툴팁 초기화
        if (fishInventoryManager != null)
        {
            fishInventoryManager.ResetUIState();
        }


        // UI를 클릭할 수 있도록 마우스 활성화
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }


    private void CloseInventory()
    {
        if (!isOpened)
            return;


        isOpened = false;


        // 먼저 상세창 / 툴팁 초기화
        if (fishInventoryManager != null)
        {
            fishInventoryManager.ResetUIState();
        }


        // 전체 인벤토리 UI 끄기
        if (fishInventoryUI != null)
        {
            fishInventoryUI.SetActive(false);
        }


        // 카메라 회전 다시 활성화
        if (cameraLook != null)
        {
            cameraLook.UnlockLook();
        }


        // 다시 1인칭 마우스 상태로
        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }
}