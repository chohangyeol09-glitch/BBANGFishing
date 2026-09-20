using UnityEngine;
using UnityEngine.InputSystem;

public class FisihInventoryAction : InteractionAction
{
    private CameraLook cameraLook;

    [Header("Fish Inventory UI")]
    [SerializeField]
    private GameObject fishInventoryUI;


    private bool isOpened = false;


    private void Awake()
    {
        Camera mainCamera = Camera.main;
        cameraLook = mainCamera.GetComponent<CameraLook>();

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


        // 물고기 인벤토리 UI 켜기
        if (fishInventoryUI != null)
        {
            fishInventoryUI.SetActive(true);
        }
    }


    private void CloseInventory()
    {
        isOpened = false;


        // UI 끄기
        if (fishInventoryUI != null)
        {
            fishInventoryUI.SetActive(false);
        }


        // 카메라 회전 다시 활성화
        if (cameraLook != null)
        {
            cameraLook.UnlockLook();
        }
    }
}