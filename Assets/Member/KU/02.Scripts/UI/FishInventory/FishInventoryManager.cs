using System;
using System.Collections.Generic;
using CHG._02.Script.CoreSystem;
using CHG._02.Script.FishSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishInventoryManager
    : MonoSingleton<FishInventoryManager>
{
    [Header("인벤토리 전체 부모")]
    [SerializeField]
    private Transform rowsParent;


    [Header("UI")]
    [SerializeField]
    private FishTooltipUI tooltipUI;

    [SerializeField]
    private FishDetailUI detailUI;


    [Header("페이지 UI")]
    [SerializeField]
    private Button previousPageButton;

    [SerializeField]
    private Button nextPageButton;

    [SerializeField]
    private TMP_Text pageText;


    [Header("페이지 설정")]
    [SerializeField]
    private int startPageCount = 1;

    [SerializeField]
    private int slotsPerRow = 5;


    private readonly List<FishInventorySlot> slots =
        new List<FishInventorySlot>();


    private readonly List<FishInventoryItem> inventory =
        new List<FishInventoryItem>();


    private int currentPage = 0;

    private int unlockedPageCount = 1;



    public IReadOnlyList<FishInventoryItem> Inventory =>
        inventory;


    public int CurrentFishCount =>
        inventory.Count;


    public int PageCapacity =>
        slots.Count;


    public int MaxFishCount =>
        PageCapacity *
        unlockedPageCount;


    public int CurrentPage =>
        currentPage + 1;


    public int UnlockedPageCount =>
        unlockedPageCount;


    public event Action OnInventoryChanged;



    protected override void Awake()
    {
        base.Awake();


        FindSlots();


        unlockedPageCount =
            Mathf.Max(
                1,
                startPageCount
            );


        currentPage = 0;


        if (previousPageButton != null)
        {
            previousPageButton.onClick.AddListener(
                PreviousPage
            );
        }


        if (nextPageButton != null)
        {
            nextPageButton.onClick.AddListener(
                NextPage
            );
        }


        RefreshSlots();

        RefreshPageUI();

        ResetUIState();
    }



    private void FindSlots()
    {
        slots.Clear();


        if (rowsParent == null)
        {
            Debug.LogWarning(
                "FishInventoryManager : Rows Parent가 없습니다."
            );

            return;
        }


        for (int rowIndex = 0;
             rowIndex < rowsParent.childCount;
             rowIndex++)
        {
            Transform row =
                rowsParent.GetChild(
                    rowIndex
                );


            int foundSlotCount = 0;


            for (int slotIndex = 0;
                 slotIndex < row.childCount;
                 slotIndex++)
            {
                Transform slotTransform =
                    row.GetChild(
                        slotIndex
                    );


                FishInventorySlot slot =
                    slotTransform
                        .GetComponent<FishInventorySlot>();


                if (slot == null)
                    continue;


                slot.Initialize(
                    tooltipUI,
                    detailUI
                );


                slots.Add(
                    slot
                );


                foundSlotCount++;
            }


            if (foundSlotCount !=
                slotsPerRow)
            {
                Debug.LogWarning(
                    $"{row.name}에 FishInventorySlot이 " +
                    $"{foundSlotCount}개 있습니다."
                );
            }
        }


        Debug.Log(
            $"한 페이지 슬롯 개수 : {slots.Count}"
        );
    }



    // =========================================
    // 물고기 추가
    // =========================================

    public bool AddFish(
        FishDataSO fishData)
    {
        if (fishData == null)
            return false;


        float randomWeight =
            fishData.GetRandomWeight();


        return AddFish(
            fishData,
            randomWeight
        );
    }



    public bool AddFish(
        FishDataSO fishData,
        float weight)
    {
        if (fishData == null)
            return false;


        if (IsFull())
        {
            Debug.Log(
                "물고기 인벤토리가 가득 찼습니다."
            );

            return false;
        }


        FishInventoryItem newItem =
            new FishInventoryItem(
                fishData,
                weight
            );


        inventory.Add(
            newItem
        );


        RefreshSlots();


        OnInventoryChanged?.Invoke();


        Debug.Log(
            $"{newItem.FishName} 획득 / " +
            $"{newItem.Weight:0.0}kg / " +
            $"{newItem.Price}원 / " +
            $"{inventory.Count}/{MaxFishCount}"
        );


        return true;
    }



    // =========================================
    // 특정 물고기 개수 확인
    // =========================================

    public int GetFishCount(
        FishDataSO fishData)
    {
        if (fishData == null)
            return 0;


        int count = 0;


        for (int i = 0;
             i < inventory.Count;
             i++)
        {
            FishInventoryItem item =
                inventory[i];


            if (item == null)
                continue;


            if (item.FishData ==
                fishData)
            {
                count++;
            }
        }


        return count;
    }



    // =========================================
    // 특정 물고기를 가지고 있는지 확인
    // =========================================

    public bool HasFish(
        FishDataSO fishData,
        int count = 1)
    {
        if (fishData == null)
            return false;


        if (count <= 0)
            return true;


        return GetFishCount(
                   fishData
               ) >= count;
    }



    // =========================================
    // 특정 물고기 소비
    // =========================================

    public bool ConsumeFish(
        FishDataSO fishData,
        int count = 1)
    {
        if (fishData == null)
            return false;


        if (count <= 0)
            return true;


        // 충분히 없으면 아무것도 삭제하지 않음
        if (!HasFish(
                fishData,
                count))
        {
            return false;
        }


        int removedCount = 0;


        // 뒤에서부터 삭제
        for (int i =
                 inventory.Count - 1;
             i >= 0;
             i--)
        {
            FishInventoryItem item =
                inventory[i];


            if (item == null)
                continue;


            if (item.FishData !=
                fishData)
            {
                continue;
            }


            inventory.RemoveAt(
                i
            );


            removedCount++;


            if (removedCount >=
                count)
            {
                break;
            }
        }


        RefreshSlots();

        ResetUIState();


        OnInventoryChanged?.Invoke();


        Debug.Log(
            $"{fishData.Name} " +
            $"{removedCount}마리 소비"
        );


        return true;
    }



    // =========================================
    // 물고기 1마리 삭제
    // =========================================

    public bool RemoveFish(
        FishInventoryItem fish)
    {
        if (fish == null)
            return false;


        bool removed =
            inventory.Remove(
                fish
            );


        if (!removed)
            return false;


        RefreshSlots();

        ResetUIState();


        OnInventoryChanged?.Invoke();


        return true;
    }



    // =========================================
    // 여러 마리 삭제
    // =========================================

    public void RemoveFishes(
        IReadOnlyList<FishInventoryItem> fishes)
    {
        if (fishes == null)
            return;


        bool removedAny =
            false;


        for (int i = 0;
             i < fishes.Count;
             i++)
        {
            if (fishes[i] == null)
                continue;


            if (inventory.Remove(
                    fishes[i]))
            {
                removedAny = true;
            }
        }


        if (!removedAny)
            return;


        RefreshSlots();

        ResetUIState();


        OnInventoryChanged?.Invoke();
    }



    // =========================================
    // 용량 업그레이드
    // =========================================

    public void UnlockNextPage()
    {
        unlockedPageCount++;


        Debug.Log(
            $"물고기 인벤토리 페이지 증가 : " +
            $"{unlockedPageCount}페이지 / " +
            $"최대 {MaxFishCount}마리"
        );


        RefreshPageUI();


        OnInventoryChanged?.Invoke();
    }



    // =========================================
    // 다음 페이지
    // =========================================

    public void NextPage()
    {
        if (currentPage >=
            unlockedPageCount - 1)
        {
            return;
        }


        currentPage++;


        ResetUIState();

        RefreshSlots();

        RefreshPageUI();
    }



    // =========================================
    // 이전 페이지
    // =========================================

    public void PreviousPage()
    {
        if (currentPage <= 0)
            return;


        currentPage--;


        ResetUIState();

        RefreshSlots();

        RefreshPageUI();
    }



    // =========================================
    // 현재 페이지 슬롯 갱신
    // =========================================

    private void RefreshSlots()
    {
        for (int i = 0;
             i < slots.Count;
             i++)
        {
            if (slots[i] != null)
            {
                slots[i].Clear();
            }
        }


        if (slots.Count == 0)
            return;


        int startIndex =
            currentPage *
            PageCapacity;


        for (int slotIndex = 0;
             slotIndex < PageCapacity;
             slotIndex++)
        {
            int inventoryIndex =
                startIndex +
                slotIndex;


            if (inventoryIndex >=
                inventory.Count)
            {
                break;
            }


            if (slots[slotIndex] == null)
                continue;


            slots[slotIndex].SetItem(
                inventory[
                    inventoryIndex
                ]
            );
        }
    }



    // =========================================
    // 페이지 UI
    // =========================================

    private void RefreshPageUI()
    {
        if (pageText != null)
        {
            pageText.text =
                $"{currentPage + 1} / " +
                $"{unlockedPageCount}";
        }


        if (previousPageButton != null)
        {
            previousPageButton.interactable =
                currentPage > 0;
        }


        if (nextPageButton != null)
        {
            nextPageButton.interactable =
                currentPage <
                unlockedPageCount - 1;
        }
    }



    public bool IsFull()
    {
        return inventory.Count >=
               MaxFishCount;
    }



    public void ResetUIState()
    {
        if (tooltipUI != null)
        {
            tooltipUI.Hide();
        }


        if (detailUI != null)
        {
            detailUI.Hide();
        }
    }
}