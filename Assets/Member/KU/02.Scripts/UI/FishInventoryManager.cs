using System.Collections.Generic;
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


        // 세로 부모 안에 있는 행들을
        // 위에서 아래 순서대로 확인
        for (int rowIndex = 0;
             rowIndex < rowsParent.childCount;
             rowIndex++)
        {
            Transform row =
                rowsParent.GetChild(rowIndex);


            int foundSlotCount = 0;


            // 한 행 안의 슬롯들을
            // 왼쪽에서 오른쪽 순서대로 확인
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


    // FishSO만 전달하면 무게를 랜덤 생성
    public bool AddFish(FishSO fish)
    {
        if (fish == null)
            return false;


        float randomWeight =
            fish.GetRandomWeight();


        return AddFish(
            fish,
            randomWeight
        );
    }


    // 무게까지 직접 지정해서 넣는 경우
    public bool AddFish(
        FishSO fish,
        float weight)
    {
        if (fish == null)
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
                fish,
                weight
            );


        inventory.Add(newItem);


        emptySlot.SetItem(
            newItem
        );


        Debug.Log(
            $"{fish.FishName} 추가 / " +
            $"{weight:0.0}kg / " +
            $"{newItem.Price}원"
        );


        return true;
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
}