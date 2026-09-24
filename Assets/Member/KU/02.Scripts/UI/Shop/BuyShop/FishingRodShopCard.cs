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


    private FishingRobSO rodData;


    public void Setup(FishingRobSO rod)
    {
        if (rod == null)
            return;


        rodData = rod;


        if (rodImage != null)
        {
            rodImage.sprite =
                rod.rodSprite;
        }


        if (rodNameText != null)
        {
            rodNameText.text =
                rod.rodName;
        }


        if (rarityText != null)
        {
            rarityText.text =
                GetRarityText(rod.rarity);
        }


        if (priceText != null)
        {
            priceText.text =
                rod.price.ToString();
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
                return "Common 낚싯대";

            case ShopItemRarity.Rare:
                return "Rare 낚싯대";

            case ShopItemRarity.Epic:
                return "Epic 낚싯대";

            case ShopItemRarity.Legendary:
                return "Legendary 낚싯대";
        }


        return "";
    }


    private void Buy()
    {
        if (rodData == null)
            return;


        Debug.Log(
            $"{rodData.rodName} 구매 버튼 클릭"
        );
    }
}