using System;
using System.Collections.Generic;
using CHG._02.Script.CoreSystem;
using CHG._02.Script.FishSystem;
using UnityEditor;
using UnityEngine;

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


    [Header("설정")]
    [SerializeField]
    private int slotsPerRow = 5;


    private readonly List<FishInventorySlot> slots =
        new List<FishInventorySlot>();


    private readonly List<FishInventoryItem> inventory =
        new List<FishInventoryItem>();


    public IReadOnlyList<FishInventoryItem> Inventory =>
        inventory;


    public int CurrentFishCount =>
        inventory.Count;


    public int MaxFishCount =>
        slots.Count;


    // 인벤토리 내용이 바뀌었을 때 알려주는 이벤트
    public event Action OnInventoryChanged;


    private void Awake()
    {
        FindSlots();
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
                    $"{foundSlotCount}개 있습니다. " +
                    $"현재 설정은 한 줄당 {slotsPerRow}칸입니다."
                );
            }
        }


        Debug.Log(
            $"물고기 인벤토리 슬롯 발견 : {slots.Count}개"
        );
    }


    // =========================================
    // 물고기 획득
    // =========================================

    // 일반적으로 물고기 잡았을 때 이걸 호출
    // MinWeight ~ MaxWeight 사이에서 랜덤 무게 생성
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


    // 무게를 직접 지정하고 싶을 때 사용
    public bool AddFish(
        FishDataSO fishData,
        float weight)
    {
        if (fishData == null)
            return false;


        if (slots.Count == 0)
        {
            Debug.LogWarning(
                "물고기 인벤토리 슬롯이 없습니다."
            );

            return false;
        }


        FishInventorySlot emptySlot =
            FindEmptySlot();


        if (emptySlot == null)
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


        emptySlot.SetItem(
            newItem
        );


        Debug.Log(
            $"{newItem.FishName} 획득 / " +
            $"{newItem.Weight:0.0}kg / " +
            $"{newItem.Price}원"
        );


        // 물고기가 추가됐다고 알림
        OnInventoryChanged?.Invoke();


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


        // 인벤토리가 바뀌었다고 알림
        OnInventoryChanged?.Invoke();


        return true;
    }


    // 여러 마리 한번에 삭제
    // 판매할 때 사용
    public void RemoveFishes(
        IReadOnlyList<FishInventoryItem> fishes)
    {
        if (fishes == null)
            return;


        bool removedAny = false;


        for (int i = 0;
             i < fishes.Count;
             i++)
        {
            if (fishes[i] == null)
                continue;


            if (inventory.Remove(fishes[i]))
            {
                removedAny = true;
            }
        }


        if (!removedAny)
            return;


        RefreshSlots();

        ResetUIState();


        // 판매 페이지 등에게 갱신 알림
        OnInventoryChanged?.Invoke();
    }


    // =========================================
    // 슬롯 갱신
    // =========================================

    private void RefreshSlots()
    {
        // 모든 슬롯 비우기
        for (int i = 0;
             i < slots.Count;
             i++)
        {
            if (slots[i] != null)
            {
                slots[i].Clear();
            }
        }


        // 현재 인벤토리 순서대로 다시 채우기
        for (int i = 0;
             i < inventory.Count &&
             i < slots.Count;
             i++)
        {
            if (slots[i] != null)
            {
                slots[i].SetItem(
                    inventory[i]
                );
            }
        }
    }


    private FishInventorySlot FindEmptySlot()
    {
        for (int i = 0;
             i < slots.Count;
             i++)
        {
            if (!slots[i].HasItem)
            {
                return slots[i];
            }
        }


        return null;
    }


    public bool IsFull()
    {
        return FindEmptySlot() == null;
    }


    // =========================================
    // 부가 UI 초기화
    // =========================================

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