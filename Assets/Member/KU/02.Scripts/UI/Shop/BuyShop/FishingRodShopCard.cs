using NKT.Fishing.Rob;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingRodShopCard : MonoBehaviour
{
    [Header("이미지")]
    [SerializeField]
    private Image rodImage;


    [Header("텍스트")]
    [SerializeField]
    private TMP_Text rodNameText;

    [SerializeField]
    private TMP_Text rarityText;

    [SerializeField]
    private TMP_Text priceText;


    [Header("구매 버튼")]
    [SerializeField]
    private Button buyButton;

    [SerializeField]
    private TMP_Text buyButtonText;


    [Header("잠금 오버레이")]
    [SerializeField]
    private GameObject lockOverlay;


    private FishingRobSO rodData;
    private BuyPageManager buyPageManager;
    private int rodIndex;

    private bool isUnlocked;
    private bool isPurchased;


    public int RodIndex => rodIndex;


    public void Setup(
        FishingRobSO rod,
        int index,
        BuyPageManager manager)
    {
        if (rod == null)
            return;

        rodData = rod;
        rodIndex = index;
        buyPageManager = manager;

        if (rodImage != null)
        {
            rodImage.sprite = rod.rodSprite;
        }

        if (rodNameText != null)
        {
            rodNameText.text = rod.rodName;
        }

        if (rarityText != null)
        {
            rarityText.text = GetRarityText(rod.rarity);
        }

        if (priceText != null)
        {
            int price = Mathf.RoundToInt(rod.price);
            priceText.text = $"{price:N0}원";
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnClickBuy);
        }
    }


    public void RefreshState(bool unlocked, bool purchased)
    {
        isUnlocked = unlocked;
        isPurchased = purchased;

        if (lockOverlay != null)
        {
            lockOverlay.SetActive(!isUnlocked);
        }

        if (buyButton != null)
        {
            buyButton.interactable =
                isUnlocked && !isPurchased;
        }

        if (buyButtonText != null)
        {
            if (!isUnlocked)
            {
                buyButtonText.text = "잠금";
            }
            else if (isPurchased)
            {
                buyButtonText.text = "구매 완료";
            }
            else
            {
                buyButtonText.text = "구매";
            }
        }
    }


    private void OnClickBuy()
    {
        if (buyPageManager == null)
            return;

        buyPageManager.TryBuyRod(rodIndex);
    }


    public FishingRobSO GetRodData()
    {
        return rodData;
    }


    private string GetRarityText(
        ShopItemRarity rarity)
    {
        switch (rarity)
        {
            case ShopItemRarity.Common:
                return "Common";

            case ShopItemRarity.Rare:
                return "Rare";

            case ShopItemRarity.Epic:
                return "Epic";

            case ShopItemRarity.Legendary:
                return "Legendary";
        }

        return "";
    }
}