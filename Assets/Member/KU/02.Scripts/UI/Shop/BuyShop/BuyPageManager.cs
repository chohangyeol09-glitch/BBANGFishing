using System.Collections.Generic;
using NKT.Fishing.Bait;
using NKT.Fishing.Rob;
using NKT.Player.Modules;
using UnityEngine;

public class BuyPageManager : MonoBehaviour
{
    [Header("카테고리 페이지")]
    [SerializeField]
    private GameObject baitPage;

    [SerializeField]
    private GameObject rodPage;


    [Header("카드가 생성될 부모")]
    [SerializeField]
    private Transform baitContent;

    [SerializeField]
    private Transform rodContent;


    [Header("카드 프리팹")]
    [SerializeField]
    private BaitShopCard baitCardPrefab;

    [SerializeField]
    private FishingRodShopCard rodCardPrefab;


    [Header("판매할 미끼")]
    [SerializeField]
    private BaitSO[] baits;


    [Header("판매할 낚싯대")]
    [SerializeField]
    private FishingRobSO[] rods;


    [Header("플레이어 낚싯대 장착")]
    [SerializeField]
    private RobEquipModule robEquipModule;


    [Header("핫바")]
    [SerializeField]
    private HotbarModule hotbarModule;


    private bool isCreated = false;


    private readonly List<FishingRodShopCard> rodCards =
        new List<FishingRodShopCard>();


    private bool[] purchasedRodFlags;



    private void Awake()
    {
        if (rods != null)
        {
            purchasedRodFlags =
                new bool[rods.Length];
        }
    }



    private void OnEnable()
    {
        if (!isCreated)
        {
            CreateShopItems();

            isCreated = true;
        }


        OpenBaitPage();

        RefreshRodCards();
    }



    private void CreateShopItems()
    {
        CreateBaitCards();

        CreateRodCards();
    }



    private void CreateBaitCards()
    {
        if (baitContent == null ||
            baitCardPrefab == null)
            return;


        if (baits == null)
            return;


        foreach (BaitSO bait in baits)
        {
            if (bait == null)
                continue;


            BaitShopCard card =
                Instantiate(
                    baitCardPrefab,
                    baitContent
                );


            card.Setup(
                bait
            );
        }
    }



    private void CreateRodCards()
    {
        rodCards.Clear();


        if (rodContent == null ||
            rodCardPrefab == null)
            return;


        if (rods == null)
            return;


        for (int i = 0;
             i < rods.Length;
             i++)
        {
            FishingRobSO rod =
                rods[i];


            if (rod == null)
                continue;


            FishingRodShopCard card =
                Instantiate(
                    rodCardPrefab,
                    rodContent
                );


            card.Setup(
                rod,
                i,
                this
            );


            rodCards.Add(
                card
            );
        }


        RefreshRodCards();
    }



    public void TryBuyRod(
        int rodIndex)
    {
        if (!IsValidRodIndex(rodIndex))
            return;


        if (IsRodPurchased(rodIndex))
            return;


        if (!IsRodUnlocked(rodIndex))
        {
            Debug.Log(
                "이전 낚싯대를 먼저 구매해야 합니다."
            );

            return;
        }


        FishingRobSO rod =
            rods[rodIndex];


        if (rod == null)
            return;


        // 실제 장착 모듈 확인
        if (robEquipModule == null)
        {
            Debug.LogWarning(
                "BuyPageManager : RobEquipModule이 연결되어 있지 않습니다."
            );

            return;
        }


        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning(
                "MoneyManager가 없습니다."
            );

            return;
        }


        int price =
            Mathf.RoundToInt(
                rod.price
            );


        // 돈 차감
        bool success =
            MoneyManager.Instance
                .TrySpendMoney(
                    price
                );


        if (!success)
            return;


        // 핫바를 거쳐야 슬롯까지 갱신된다.
        // 직접 TryEquip 하면 총을 들었다 돌아올 때 슬롯의 옛 낚싯대로 되돌아간다.
        bool equipped = hotbarModule != null
            ? hotbarModule.EquipRod(rod)
            : robEquipModule.TryEquip(rod);


        // 장착에 실패하면 금액을 복구하고 구매/잠금 해제를 진행하지 않는다.
        if (!equipped)
        {
            MoneyManager.Instance.AddMoney(price);
            return;
        }

        purchasedRodFlags[rodIndex] = true;
        Debug.Log(
            $"{rod.rodName} 구매 완료 / 즉시 장착"
        );


        RefreshRodCards();
    }



    private void RefreshRodCards()
    {
        for (int i = 0;
             i < rodCards.Count;
             i++)
        {
            FishingRodShopCard card =
                rodCards[i];


            if (card == null)
                continue;


            int rodIndex =
                card.RodIndex;


            bool unlocked =
                IsRodUnlocked(
                    rodIndex
                );


            bool purchased =
                IsRodPurchased(
                    rodIndex
                );


            card.RefreshState(
                unlocked,
                purchased
            );
        }
    }



    private bool IsRodUnlocked(
        int rodIndex)
    {
        if (!IsValidRodIndex(rodIndex))
            return false;


        // 첫 번째 낚싯대는 항상 구매 가능
        if (rodIndex == 0)
            return true;


        // 바로 전 낚싯대를 구매했으면
        // 다음 낚싯대 잠금 해제
        return purchasedRodFlags[
            rodIndex - 1
        ];
    }



    private bool IsRodPurchased(
        int rodIndex)
    {
        if (!IsValidRodIndex(rodIndex))
            return false;


        return purchasedRodFlags[
            rodIndex
        ];
    }



    private bool IsValidRodIndex(
        int rodIndex)
    {
        if (purchasedRodFlags == null)
            return false;


        return rodIndex >= 0 &&
               rodIndex <
               purchasedRodFlags.Length;
    }



    public void OpenBaitPage()
    {
        if (baitPage != null)
        {
            baitPage.SetActive(true);
        }


        if (rodPage != null)
        {
            rodPage.SetActive(false);
        }
    }



    public void OpenRodPage()
    {
        if (baitPage != null)
        {
            baitPage.SetActive(false);
        }


        if (rodPage != null)
        {
            rodPage.SetActive(true);
        }


        RefreshRodCards();
    }
}