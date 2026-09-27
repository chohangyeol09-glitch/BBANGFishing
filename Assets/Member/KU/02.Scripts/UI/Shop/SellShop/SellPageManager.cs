using System.Collections.Generic;
using TMPro;
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


    [Header("판매 정보")]
    [SerializeField]
    private TMP_Text totalPriceText;

    [SerializeField]
    private TMP_Text selectedCountText;


    [Header("버튼")]
    [SerializeField]
    private Button resetButton;

    [SerializeField]
    private Button sellButton;


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


    private void Awake()
    {
        if (resetButton != null)
        {
            resetButton.onClick
                .AddListener(
                    ClearSelection
                );
        }


        if (sellButton != null)
        {
            sellButton.onClick
                .AddListener(
                    SellSelectedFishes
                );
        }
    }


    private void OnEnable()
    {
        if (fishInventoryManager != null)
        {
            fishInventoryManager.OnInventoryChanged +=
                RefreshFishList;
        }


        ClearSelection();

        RefreshFishList();
    }


    private void OnDisable()
    {
        if (fishInventoryManager != null)
        {
            fishInventoryManager.OnInventoryChanged -=
                RefreshFishList;
        }
    }


    // =========================
    // 왼쪽 물고기 목록
    // =========================

    public void RefreshFishList()
    {
        ClearFishCards();


        if (fishInventoryManager == null)
            return;


        if (fishCardPrefab == null)
            return;


        if (fishContent == null)
            return;


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
    // 선택
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


        selectedFishes.Add(
            fish
        );


        SetCardSelected(
            fish,
            true
        );


        CreateSelectedFishUI(
            fish
        );


        RefreshSellInfo();
    }


    public void DeselectFish(
        FishInventoryItem fish)
    {
        if (fish == null)
            return;


        if (!selectedFishes.Remove(fish))
            return;


        SetCardSelected(
            fish,
            false
        );


        RemoveSelectedFishUI(
            fish
        );


        RefreshSellInfo();
    }


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


        SellSelectedFishItem item =
            Instantiate(
                selectedFishPrefab,
                selectedFishContent
            );


        item.Setup(
            fish,
            this
        );


        selectedFishUIs.Add(
            item
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


            if (ui.FishItem != fish)
                continue;


            selectedFishUIs.RemoveAt(i);


            Destroy(
                ui.gameObject
            );


            break;
        }


        RefreshRightScroll();
    }


    // =========================
    // 선택 초기화
    // =========================

    public void ClearSelection()
    {
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

        RefreshSellInfo();
    }


    // =========================
    // 실제 판매
    // =========================

    public void SellSelectedFishes()
    {
        if (selectedFishes.Count == 0)
        {
            Debug.Log(
                "판매할 물고기가 없습니다."
            );

            return;
        }


        if (fishInventoryManager == null)
            return;


        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning(
                "MoneyManager가 없습니다."
            );

            return;
        }


        List<FishInventoryItem> fishesToSell =
            new List<FishInventoryItem>(
                selectedFishes
            );


        int totalPrice = 0;


        for (int i = 0;
             i < fishesToSell.Count;
             i++)
        {
            totalPrice +=
                fishesToSell[i].Price;
        }


        // 선택 UI 먼저 정리
        ClearSelection();


        // 실제 물고기 삭제
        fishInventoryManager.RemoveFishes(
            fishesToSell
        );


        // 돈 지급
        MoneyManager.Instance.AddMoney(
            totalPrice
        );


        Debug.Log(
            $"{fishesToSell.Count}마리 판매 완료 / " +
            $"{totalPrice}원 획득"
        );
    }


    // =========================
    // 판매 정보
    // =========================

    private void RefreshSellInfo()
    {
        int totalPrice = 0;


        for (int i = 0;
             i < selectedFishes.Count;
             i++)
        {
            totalPrice +=
                selectedFishes[i].Price;
        }


        if (totalPriceText != null)
        {
            totalPriceText.text =
                $"{totalPrice:N0}원";
        }


        if (selectedCountText != null)
        {
            selectedCountText.text =
                $"{selectedFishes.Count}마리";
        }


        if (sellButton != null)
        {
            sellButton.interactable =
                selectedFishes.Count > 0;
        }
    }


    // =========================
    // 스크롤
    // =========================

    private void RefreshLeftScroll()
    {
        Canvas.ForceUpdateCanvases();


        if (fishContent
            is RectTransform content)
        {
            LayoutRebuilder
                .ForceRebuildLayoutImmediate(
                    content
                );
        }


        Canvas.ForceUpdateCanvases();
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