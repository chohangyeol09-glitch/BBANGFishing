using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlotSelectManager : MonoBehaviour
{
    [Serializable]
    public class QuickSlot
    {
        [Header("슬롯 위치")]
        public RectTransform slotTransform;

        [Header("아이템 이미지")]
        public Image itemImage;

        [Header("슬롯에 표시할 스프라이트")]
        public Sprite itemSprite;
    }


    [Header("퀵 슬롯")]
    [SerializeField]
    private QuickSlot[] slots;


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
        RefreshSlotImages();


        if (selectFrame == null)
            return;


        if (selectFirstSlotOnStart &&
            slots != null &&
            slots.Length > 0)
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


        if (slots[index].slotTransform == null)
            return;


        selectedIndex = index;


        if (selectFrame != null)
        {
            selectFrame.gameObject.SetActive(true);

            selectFrame.position =
                slots[index].slotTransform.position;
        }
    }


    private void RefreshSlotImages()
    {
        if (slots == null)
            return;


        for (int i = 0; i < slots.Length; i++)
        {
            QuickSlot slot = slots[i];


            if (slot.itemImage == null)
                continue;


            if (slot.itemSprite != null)
            {
                slot.itemImage.sprite =
                    slot.itemSprite;

                slot.itemImage.enabled = true;
            }
            else
            {
                slot.itemImage.sprite = null;
                slot.itemImage.enabled = false;
            }
        }
    }


    public void SetSlotSprite(
        int index,
        Sprite sprite)
    {
        if (slots == null)
            return;


        if (index < 0 || index >= slots.Length)
            return;


        QuickSlot slot = slots[index];

        slot.itemSprite = sprite;


        if (slot.itemImage == null)
            return;


        slot.itemImage.sprite = sprite;
        slot.itemImage.enabled = sprite != null;
    }


    public void ClearSlot(int index)
    {
        SetSlotSprite(
            index,
            null
        );
    }


    public Sprite GetSlotSprite(int index)
    {
        if (slots == null)
            return null;


        if (index < 0 || index >= slots.Length)
            return null;


        return slots[index].itemSprite;
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