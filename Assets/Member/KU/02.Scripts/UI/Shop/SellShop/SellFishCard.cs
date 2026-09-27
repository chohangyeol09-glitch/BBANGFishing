using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellFishCard :
    MonoBehaviour,
    IPointerClickHandler
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

    [SerializeField]
    private TMP_Text gradeText;


    [Header("선택 표시")]
    [SerializeField]
    private GameObject selectFrame;


    private FishInventoryItem fishItem;

    private SellPageManager sellPageManager;


    public FishInventoryItem FishItem =>
        fishItem;


    private void Awake()
    {
        SetSelected(false);
    }


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


        if (gradeText != null &&
            item.FishData != null)
        {
            gradeText.text =
                item.FishData.Grade.ToString();
        }
    }


    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (fishItem == null)
            return;


        if (sellPageManager == null)
            return;


        sellPageManager.ToggleFishSelection(
            fishItem
        );
    }


    public void SetSelected(bool value)
    {
        if (selectFrame != null)
        {
            selectFrame.SetActive(value);
        }
    }
}