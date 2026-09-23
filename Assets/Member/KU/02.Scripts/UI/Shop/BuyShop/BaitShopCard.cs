using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaitShopCard : MonoBehaviour
{
    //[Header("이미지")]
    //[SerializeField]
    //private Image baitImage;

    //[SerializeField]
    //private Image requiredFishImage;


    //[Header("텍스트")]
    //[SerializeField]
    //private TMP_Text baitNameText;

    //[SerializeField]
    //private TMP_Text rarityText;

    //[SerializeField]
    //private TMP_Text priceText;


    //[Header("구매 버튼")]
    //[SerializeField]
    //private Button buyButton;


    //private BaitSO baitData;


    //public void Setup(BaitSO bait)
    //{
    //    if (bait == null)
    //        return;


    //    baitData = bait;


    //    if (baitImage != null)
    //    {
    //        baitImage.sprite =
    //            bait.BaitSprite;
    //    }


    //    if (baitNameText != null)
    //    {
    //        baitNameText.text =
    //            bait.BaitName;
    //    }


    //    if (rarityText != null)
    //    {
    //        rarityText.text =
    //            GetRarityText(bait.Rarity);
    //    }


    //    if (priceText != null)
    //    {
    //        priceText.text =
    //            bait.Price.ToString();
    //    }


    //    if (requiredFishImage != null)
    //    {
    //        if (bait.RequiredFish != null)
    //        {
    //            requiredFishImage.sprite =
    //                bait.RequiredFish.FishSprite;

    //            requiredFishImage.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            requiredFishImage.gameObject.SetActive(false);
    //        }
    //    }


    //    if (buyButton != null)
    //    {
    //        buyButton.onClick.RemoveAllListeners();

    //        buyButton.onClick.AddListener(
    //            Buy
    //        );
    //    }
    //}


    //private string GetRarityText(
    //    ShopItemRarity rarity)
    //{
    //    switch (rarity)
    //    {
    //        case ShopItemRarity.Common:
    //            return "Common 미끼";

    //        case ShopItemRarity.Rare:
    //            return "Rare 미끼";

    //        case ShopItemRarity.Epic:
    //            return "Epic 미끼";

    //        case ShopItemRarity.Legendary:
    //            return "Legendary 미끼";
    //    }


    //    return "";
    //}


    //private void Buy()
    //{
    //    if (baitData == null)
    //        return;


    //    Debug.Log(
    //        $"{baitData.BaitName} 구매 버튼 클릭"
    //    );
    //}
}