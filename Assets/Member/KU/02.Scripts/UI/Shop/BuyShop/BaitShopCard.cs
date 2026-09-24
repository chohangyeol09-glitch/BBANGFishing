using NKT.Fishing.Bait;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaitShopCard : MonoBehaviour
{
    [Header("이미지")]
    [SerializeField]
    private Image baitImage;

    [SerializeField]
    private Image requiredFishImage;


    [Header("텍스트")]
    [SerializeField]
    private TMP_Text baitNameText;

    [SerializeField]
    private TMP_Text rarityText;

    [SerializeField]
    private TMP_Text priceText;


    [Header("구매 버튼")]
    [SerializeField]
    private Button buyButton;


    private BaitSO baitData;


    public void Setup(BaitSO bait)
    {
        if (bait == null)
            return;


        baitData = bait;


        if (baitImage != null)
        {
            baitImage.sprite =
                bait.baitSprite;
        }


        if (baitNameText != null)
        {
            baitNameText.text =
                bait.baitName;
        }


        if (rarityText != null)
        {
            rarityText.text =
                GetRarityText(bait.rarity);
        }


        if (priceText != null)
        {
            priceText.text =
                bait.price.ToString() + "$ 구매";
        }


        if (requiredFishImage != null)
        {
            if (bait.fishesToBuy != null)
            {
                requiredFishImage.sprite =
                    bait.fishesToBuy.Sprite;

                requiredFishImage.gameObject.SetActive(true);
            }
            else
            {
                requiredFishImage.gameObject.SetActive(false);
            }
        }


        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();

            buyButton.onClick.AddListener(
                Buy
            );
        }
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


    private void Buy()
    {
        if (baitData == null)
            return;


        Debug.Log(
            $"{baitData.baitName} 구매 버튼 클릭"
        );
    }
}