using NKT.Fishing.Bait;
using NKT.Fishing.Rob;
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


    private bool isCreated = false;


    private void OnEnable()
    {
        if (!isCreated)
        {
            CreateShopItems();

            isCreated = true;
        }


        OpenBaitPage();
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


        foreach (BaitSO bait in baits)
        {
            if (bait == null)
                continue;


            BaitShopCard card =
                Instantiate(
                    baitCardPrefab,
                    baitContent
                );


            card.Setup(bait);
        }
    }


    private void CreateRodCards()
    {
        if (rodContent == null ||
            rodCardPrefab == null)
            return;


        foreach (FishingRobSO rod in rods)
        {
            if (rod == null)
                continue;


            FishingRodShopCard card =
                Instantiate(
                    rodCardPrefab,
                    rodContent
                );


            card.Setup(rod);
        }
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
    }
}