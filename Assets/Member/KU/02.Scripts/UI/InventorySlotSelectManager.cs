using System;
using NKT.Player.Modules;
using UnityEngine;
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


    [Header("핫바")]
    [SerializeField]
    private HotbarModule hotbar;


    [Header("설정")]
    [SerializeField]
    private bool selectFirstSlotOnStart = true;


    private int selectedIndex = -1;


    public int SelectedIndex => selectedIndex;


    // 선택과 슬롯 내용은 HotbarModule이 정한다.
    // 여기서 키를 직접 읽으면 선택 상태가 둘로 갈라지고, UI 잠금도 통과해 버린다.
    private void OnEnable()
    {
        if (hotbar == null)
            return;


        hotbar.OnSelectionChanged += SelectSlot;
        hotbar.OnSlotChanged += SetSlotSprite;
    }


    private void OnDisable()
    {
        if (hotbar == null)
            return;


        hotbar.OnSelectionChanged -= SelectSlot;
        hotbar.OnSlotChanged -= SetSlotSprite;
    }


    private void Start()
    {
        RefreshSlotImages();


        if (selectFrame == null)
            return;


        // 핫바가 있으면 그쪽 선택을 따르고, 없으면 기존처럼 첫 칸을 고른다
        if (hotbar != null)
        {
            // 구독이 늦어 초기 이벤트를 놓쳤을 수 있으니 한 번 다시 받는다
            hotbar.RefreshView();
        }
        else if (selectFirstSlotOnStart &&
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