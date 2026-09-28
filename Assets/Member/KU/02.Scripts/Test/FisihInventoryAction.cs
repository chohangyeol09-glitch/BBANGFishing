using NKT.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class FisihInventoryAction : InteractionAction
{
    [Header("플레이어")]
    [SerializeField]
    private Player player;


    [Header("물고기 인벤토리 UI")]
    [SerializeField]
    private GameObject fishInventoryUI;


    [Header("물고기 인벤토리")]
    [SerializeField]
    private FishInventoryManager fishInventoryManager;


    private bool isOpened;



    private void Awake()
    {
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


        if (Keyboard.current.escapeKey
            .wasPressedThisFrame)
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


        if (fishInventoryUI != null)
        {
            fishInventoryUI.SetActive(true);
        }


        if (fishInventoryManager != null)
        {
            fishInventoryManager.ResetUIState();
        }


        // 기존 CameraLook.LockLook()
        if (player != null)
        {
            player.LockLook();
        }
    }



    private void CloseInventory()
    {
        isOpened = false;


        if (fishInventoryUI != null)
        {
            fishInventoryUI.SetActive(false);
        }


        if (fishInventoryManager != null)
        {
            fishInventoryManager.ResetUIState();
        }


        // 기존 CameraLook.UnlockLook()
        if (player != null)
        {
            player.UnlockLook();
        }
    }
}