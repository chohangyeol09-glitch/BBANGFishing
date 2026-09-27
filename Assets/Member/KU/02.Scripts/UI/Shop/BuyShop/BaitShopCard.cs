using NKT.Fishing.Bait;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaitShopCard : MonoBehaviour
{
    [Header("미끼 정보")]
    [SerializeField]
    private Image baitImage;

    [SerializeField]
    private TMP_Text baitNameText;

    [SerializeField]
    private TMP_Text rarityText;

    [SerializeField]
    private TMP_Text priceText;


    [Header("필요 물고기")]
    [SerializeField]
    private Image requiredFishImage;


    [Header("카드 배경")]
    [SerializeField]
    private Image backgroundImage;


    [Header("등급별 배경 색상")]
    [SerializeField]
    private Color commonColor =
        new Color32(220, 220, 220, 255);

    [SerializeField]
    private Color rareColor =
        new Color32(112, 183, 255, 255);

    [SerializeField]
    private Color epicColor =
        new Color32(185, 131, 255, 255);

    [SerializeField]
    private Color legendaryColor =
        new Color32(255, 213, 106, 255);


    [Header("버튼")]
    [SerializeField]
    private Button buyButton;


    private BaitSO baitData;


    private void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(
                OnClickBuy
            );
        }
    }


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
                bait.grade.ToString();
        }


        if (priceText != null)
        {
            priceText.text =
                $"{bait.price:N0}원";
        }


        if (requiredFishImage != null)
        {
            if (bait.fishesToBuy != null)
            {
                requiredFishImage.gameObject
                    .SetActive(true);

                requiredFishImage.sprite =
                    bait.fishesToBuy.Sprite;
            }
            else
            {
                requiredFishImage.gameObject
                    .SetActive(false);
            }
        }


        // 등급에 따라 배경 색상 변경
        ApplyGradeColor(
            bait.grade
        );
    }


    private void ApplyGradeColor(
        BaitGrade grade)
    {
        if (backgroundImage == null)
            return;


        switch (grade)
        {
            case BaitGrade.Common:

                backgroundImage.color =
                    commonColor;

                break;


            case BaitGrade.Rare:

                backgroundImage.color =
                    rareColor;

                break;


            case BaitGrade.Epic:

                backgroundImage.color =
                    epicColor;

                break;


            case BaitGrade.Legendary:

                backgroundImage.color =
                    legendaryColor;

                break;
        }
    }


    private void OnClickBuy()
    {
        if (baitData == null)
            return;


        Debug.Log(
            $"{baitData.baitName} 구매"
        );
    }
}