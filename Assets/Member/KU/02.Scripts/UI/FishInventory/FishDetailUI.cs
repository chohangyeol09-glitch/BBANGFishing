using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishDetailUI : MonoBehaviour
{
    [Header("전체 UI")]
    [SerializeField]
    private GameObject detailRoot;


    [Header("물고기 이미지")]
    [SerializeField]
    private Image fishImage;


    [Header("텍스트")]
    [SerializeField]
    private TMP_Text fishNameText;

    [SerializeField]
    private TMP_Text weightText;

    [SerializeField]
    private TMP_Text gradeText;

    [SerializeField]
    private TMP_Text priceText;


    private void Awake()
    {
        Hide();
    }


    public void Show(
        FishInventoryItem item)
    {
        if (item == null ||
            item.FishData == null)
            return;


        if (detailRoot != null)
        {
            detailRoot.SetActive(true);
        }


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
                $"{item.Weight:0.0}kg";
        }


        if (gradeText != null)
        {
            gradeText.text =
                item.FishData.Grade.ToString();
        }


        if (priceText != null)
        {
            priceText.text =
                $"{item.Price}원";
        }
    }


    public void Hide()
    {
        if (detailRoot != null)
        {
            detailRoot.SetActive(false);
        }
    }
}