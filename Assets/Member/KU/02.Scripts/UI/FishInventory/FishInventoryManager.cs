using System;
using System.Collections.Generic;
using CHG._02.Script.FishSystem;
using UnityEngine;

public class FishInventoryManager : MonoBehaviour
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
                    $"{foundSlotCount}개 있습니다."
                );
            }
        }
    }


    public bool AddFish(
        FishDataSO fishData)
    {
        if (fishData == null)
            return false;


        return AddFish(
            fishData,
            fishData.Weight
        );
    }


    public bool AddFish(
        FishDataSO fishData,
        float weight)
    {
        if (fishData == null)
            return false;


        if (inventory.Count >= slots.Count)
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
            $"{newItem.FishName} 추가 / " +
            $"{newItem.Weight:0.0}kg / " +
            $"{newItem.Price}원"
        );


        return true;
    }


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


        bool removedAny = false;


        for (int i = 0;
             i < fishes.Count;
             i++)
        {
            if (inventory.Remove(fishes[i]))
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


    private void RefreshSlots()
    {
        for (int i = 0;
             i < slots.Count;
             i++)
        {
            slots[i].Clear();
        }


        for (int i = 0;
             i < inventory.Count &&
             i < slots.Count;
             i++)
        {
            slots[i].SetItem(
                inventory[i]
            );
        }
    }


    public bool IsFull()
    {
        return inventory.Count >=
               slots.Count;
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