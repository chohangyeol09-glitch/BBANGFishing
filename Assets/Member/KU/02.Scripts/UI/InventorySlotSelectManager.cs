using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySlotSelectManager : MonoBehaviour
{
    [Header("인벤토리 슬롯")]
    [SerializeField]
    private RectTransform[] slots;

        
    [Header("선택 프레임")]
    [SerializeField]
    private RectTransform selectFrame;


    [Header("설정")]
    [SerializeField]
    private bool selectFirstSlotOnStart = true;


    private int selectedIndex = -1;


    public int SelectedIndex => selectedIndex;


    private void Start()
    {
        if (selectFrame == null)
            return;


        if (selectFirstSlotOnStart && slots.Length > 0)
        {
            SelectSlot(0);
        }
        else
        {
            selectFrame.gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;


        if (Keyboard.current.digit1Key.wasPressedThisFrame ||
            Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            SelectSlot(0);
        }


        if (Keyboard.current.digit2Key.wasPressedThisFrame ||
            Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            SelectSlot(1);
        }


        if (Keyboard.current.digit3Key.wasPressedThisFrame ||
            Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            SelectSlot(2);
        }
    }


    public void SelectSlot(int index)
    {
        if (slots == null)
            return;


        if (index < 0 || index >= slots.Length)
            return;


        if (slots[index] == null)
            return;


        selectedIndex = index;


        // 선택 프레임 활성화
        if (!selectFrame.gameObject.activeSelf)
        {
            selectFrame.gameObject.SetActive(true);
        }


        // 선택한 슬롯 위치로 이동
        selectFrame.position =
            slots[index].position;
    }


    public void HideSelectFrame()
    {
        selectedIndex = -1;


        if (selectFrame != null)
        {
            selectFrame.gameObject.SetActive(false);
        }
    }
}