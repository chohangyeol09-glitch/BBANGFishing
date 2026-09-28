using System;
using System.Collections.Generic;
using CHG._02.Script.CoreSystem;
using CHG._02.Script.FishSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishInventoryManager : MonoSingleton<FishInventoryManager>
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


    // 0부터 시작
    private int currentPage = 0;


    // 현재 해금된 페이지 개수
    private int unlockedPageCount = 1;



    public IReadOnlyList<FishInventoryItem> Inventory =>
        inventory;


    public int CurrentFishCount =>
        inventory.Count;


    // 한 페이지에 들어가는 슬롯 개수
    public int PageCapacity =>
        slots.Count;


    // 전체 최대 물고기 수
    public int MaxFishCount =>
        PageCapacity * unlockedPageCount;


    // 외부에서 현재 페이지 확인용
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
                rowsParent.GetChild(rowIndex);


            int foundSlotCount = 0;


            for (int slotIndex = 0;
                 slotIndex < row.childCount;
                 slotIndex++)
            {
                Transform slotTransform =
                    row.GetChild(slotIndex);


                FishInventorySlot slot =
                    slotTransform
                        .GetComponent<FishInventorySlot>();


                if (slot == null)
                    continue;


                slot.Initialize(
                    tooltipUI,
                    detailUI
                );


                slots.Add(slot);

                foundSlotCount++;
            }


            if (foundSlotCount != slotsPerRow)
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


        // 전체 페이지가 꽉 찼는지 확인
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
    // 물고기 삭제
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
    // 페이지 이동
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
        // 화면에 보이는 슬롯 전부 비움
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


        // 현재 페이지가 시작하는 인벤토리 인덱스
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
                inventory[inventoryIndex]
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
                $"{currentPage + 1} / {unlockedPageCount}";
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



    // =========================================
    // 기타
    // =========================================

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