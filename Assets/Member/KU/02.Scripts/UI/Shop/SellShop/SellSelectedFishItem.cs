using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellSelectedFishItem : MonoBehaviour
{
    [Header("물고기 이미지")]
    [SerializeField]
    private Image fishImage;


    [Header("텍스트")]
    [SerializeField]
    private TMP_Text fishNameText;

    [SerializeField]
    private TMP_Text weightText;

    [SerializeField]
    private TMP_Text priceText;


    [Header("취소 버튼")]
    [SerializeField]
    private Button cancelButton;


    private FishInventoryItem fishItem;

    private SellPageManager sellPageManager;


    public FishInventoryItem FishItem =>
        fishItem;


    public void Setup(
        FishInventoryItem item,
        SellPageManager manager)
    {
        if (item == null)
            return;


        fishItem = item;

        sellPageManager = manager;


        if (fishImage != null)
        {
            fishImage.sprite =
                item.FishSprite;
        }


        if (fishNameText != null)
        {
            fishNameText.text =
                item.FishName;
        }


        if (weightText != null)
        {
            weightText.text =
                $"{item.Weight:0.0} kg";
        }


        if (priceText != null)
        {
            priceText.text =
                $"{item.Price}원";
        }


        if (cancelButton != null)
        {
            cancelButton.onClick
                .RemoveAllListeners();


            cancelButton.onClick
                .AddListener(Cancel);
        }
    }


    private void Cancel()
    {
        if (fishItem == null)
            return;


        if (sellPageManager == null)
            return;


        sellPageManager.DeselectFish(
            fishItem
        );
    }
}