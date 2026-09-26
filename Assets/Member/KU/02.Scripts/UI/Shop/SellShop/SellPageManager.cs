using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellPageManager : MonoBehaviour
{
    [Header("물고기 인벤토리")]
    [SerializeField]
    private FishInventoryManager fishInventoryManager;


    [Header("왼쪽 - 보유 물고기")]
    [SerializeField]
    private SellFishCard fishCardPrefab;

    [SerializeField]
    private Transform fishContent;

    [SerializeField]
    private ScrollRect fishScrollRect;


    [Header("오른쪽 - 판매 물품")]
    [SerializeField]
    private SellSelectedFishItem selectedFishPrefab;

    [SerializeField]
    private Transform selectedFishContent;

    [SerializeField]
    private ScrollRect selectedFishScrollRect;


    private readonly List<SellFishCard>
        createdCards =
        new List<SellFishCard>();


    private readonly List<FishInventoryItem>
        selectedFishes =
        new List<FishInventoryItem>();


    private readonly List<SellSelectedFishItem>
        selectedFishUIs =
        new List<SellSelectedFishItem>();


    public IReadOnlyList<FishInventoryItem>
        SelectedFishes =>
        selectedFishes;


    private void OnEnable()
    {
        RefreshFishList();
    }


    // =========================
    // 왼쪽 물고기 목록 생성
    // =========================

    public void RefreshFishList()
    {
        ClearFishCards();


        if (fishInventoryManager == null)
        {
            Debug.LogWarning(
                "FishInventoryManager가 없습니다."
            );

            return;
        }


        if (fishCardPrefab == null)
        {
            Debug.LogWarning(
                "SellFishCard Prefab이 없습니다."
            );

            return;
        }


        if (fishContent == null)
        {
            Debug.LogWarning(
                "Fish Content가 없습니다."
            );

            return;
        }


        IReadOnlyList<FishInventoryItem> fishes =
            fishInventoryManager.Inventory;


        for (int i = 0;
             i < fishes.Count;
             i++)
        {
            FishInventoryItem fish =
                fishes[i];


            SellFishCard card =
                Instantiate(
                    fishCardPrefab,
                    fishContent
                );


            card.Setup(
                fish,
                this
            );


            // 이미 판매 선택된 물고기라면
            // 새로 생성된 카드도 선택 표시
            card.SetSelected(
                selectedFishes.Contains(fish)
            );


            createdCards.Add(
                card
            );
        }


        RefreshLeftScroll();
    }


    private void ClearFishCards()
    {
        for (int i = 0;
             i < createdCards.Count;
             i++)
        {
            if (createdCards[i] != null)
            {
                Destroy(
                    createdCards[i].gameObject
                );
            }
        }


        createdCards.Clear();
    }


    // =========================
    // 물고기 선택 / 취소
    // =========================

    public void ToggleFishSelection(
        FishInventoryItem fish)
    {
        if (fish == null)
            return;


        if (selectedFishes.Contains(fish))
        {
            DeselectFish(fish);
        }
        else
        {
            SelectFish(fish);
        }
    }


    private void SelectFish(
        FishInventoryItem fish)
    {
        if (fish == null)
            return;


        if (selectedFishes.Contains(fish))
            return;


        selectedFishes.Add(fish);


        // 왼쪽 카드 선택 프레임 ON
        SetCardSelected(
            fish,
            true
        );


        // 오른쪽 판매 물품 생성
        CreateSelectedFishUI(
            fish
        );
    }


    public void DeselectFish(
        FishInventoryItem fish)
    {
        if (fish == null)
            return;


        if (!selectedFishes.Contains(fish))
            return;


        selectedFishes.Remove(fish);


        // 왼쪽 카드 선택 프레임 OFF
        SetCardSelected(
            fish,
            false
        );


        // 오른쪽 판매 물품 삭제
        RemoveSelectedFishUI(
            fish
        );
    }


    // =========================
    // 왼쪽 선택 프레임
    // =========================

    private void SetCardSelected(
        FishInventoryItem fish,
        bool selected)
    {
        for (int i = 0;
             i < createdCards.Count;
             i++)
        {
            SellFishCard card =
                createdCards[i];


            if (card == null)
                continue;


            if (card.FishItem == fish)
            {
                card.SetSelected(
                    selected
                );

                return;
            }
        }
    }


    // =========================
    // 오른쪽 판매 물품
    // =========================

    private void CreateSelectedFishUI(
        FishInventoryItem fish)
    {
        if (selectedFishPrefab == null)
            return;


        if (selectedFishContent == null)
            return;


        SellSelectedFishItem newItem =
            Instantiate(
                selectedFishPrefab,
                selectedFishContent
            );


        newItem.Setup(
            fish,
            this
        );


        selectedFishUIs.Add(
            newItem
        );


        RefreshRightScroll();
    }


    private void RemoveSelectedFishUI(
        FishInventoryItem fish)
    {
        for (int i =
                 selectedFishUIs.Count - 1;
             i >= 0;
             i--)
        {
            SellSelectedFishItem ui =
                selectedFishUIs[i];


            if (ui == null)
            {
                selectedFishUIs.RemoveAt(i);
                continue;
            }


            if (ui.FishItem == fish)
            {
                selectedFishUIs.RemoveAt(i);

                Destroy(
                    ui.gameObject
                );

                break;
            }
        }


        RefreshRightScroll();
    }


    // =========================
    // 선택 초기화
    // =========================

    public void ClearSelection()
    {
        // 왼쪽 선택 프레임 모두 OFF
        for (int i = 0;
             i < createdCards.Count;
             i++)
        {
            if (createdCards[i] != null)
            {
                createdCards[i]
                    .SetSelected(false);
            }
        }


        selectedFishes.Clear();


        // 오른쪽 판매 물품 모두 제거
        for (int i = 0;
             i < selectedFishUIs.Count;
             i++)
        {
            if (selectedFishUIs[i] != null)
            {
                Destroy(
                    selectedFishUIs[i]
                        .gameObject
                );
            }
        }


        selectedFishUIs.Clear();


        RefreshRightScroll();
    }


    // =========================
    // Scroll / Layout 갱신
    // =========================

    private void RefreshLeftScroll()
    {
        Canvas.ForceUpdateCanvases();


        if (fishContent is RectTransform content)
        {
            LayoutRebuilder
                .ForceRebuildLayoutImmediate(
                    content
                );
        }


        Canvas.ForceUpdateCanvases();


        if (fishScrollRect != null)
        {
            fishScrollRect
                .verticalNormalizedPosition = 1f;
        }
    }


    private void RefreshRightScroll()
    {
        Canvas.ForceUpdateCanvases();


        if (selectedFishContent
            is RectTransform content)
        {
            LayoutRebuilder
                .ForceRebuildLayoutImmediate(
                    content
                );
        }


        Canvas.ForceUpdateCanvases();
    }
}