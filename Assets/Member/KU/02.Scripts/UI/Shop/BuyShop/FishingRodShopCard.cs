using NKT.Fishing.Rob;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingRodShopCard : MonoBehaviour
{
    [Header("낚싯대 정보")]
    [SerializeField]
    private Image rodImage;

    [SerializeField]
    private TMP_Text rodNameText;

    [SerializeField]
    private TMP_Text rarityText;

    [SerializeField]
    private TMP_Text priceText;


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


    [Header("구매")]
    [SerializeField]
    private Button buyButton;

    [SerializeField]
    private TMP_Text buyButtonText;


    [Header("잠금")]
    [SerializeField]
    private GameObject lockOverlay;


    private FishingRobSO rodData;

    private BuyPageManager buyPageManager;

    private int rodIndex;


    private bool isUnlocked;

    private bool isPurchased;


    public int RodIndex =>
        rodIndex;


    public FishingRobSO RodData =>
        rodData;


    private void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(
                OnClickBuy
            );
        }
    }


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
                rod.grade.ToString();
        }


        if (priceText != null)
        {
            priceText.text =
                $"{rod.price:N0}원";
        }


        // 등급에 맞는 배경 적용
        ApplyGradeColor(
            rod.grade
        );
    }


    private void ApplyGradeColor(
        RobGrade grade)
    {
        if (backgroundImage == null)
            return;


        switch (grade)
        {
            case RobGrade.Common:

                backgroundImage.color =
                    commonColor;

                break;


            case RobGrade.Rare:

                backgroundImage.color =
                    rareColor;

                break;


            case RobGrade.Epic:

                backgroundImage.color =
                    epicColor;

                break;


            case RobGrade.Legendary:

                backgroundImage.color =
                    legendaryColor;

                break;
        }
    }


    public void RefreshState(
        bool unlocked,
        bool purchased)
    {
        isUnlocked = unlocked;

        isPurchased = purchased;


        if (lockOverlay != null)
        {
            lockOverlay.SetActive(
                !isUnlocked
            );
        }


        if (buyButton != null)
        {
            buyButton.interactable =
                isUnlocked &&
                !isPurchased;
        }


        if (buyButtonText != null)
        {
            if (!isUnlocked)
            {
                buyButtonText.text =
                    "잠금";
            }
            else if (isPurchased)
            {
                buyButtonText.text =
                    "구매 완료";
            }
            else
            {
                buyButtonText.text =
                    "구매";
            }
        }
    }


    private void OnClickBuy()
    {
        if (buyPageManager == null)
            return;


        buyPageManager.TryBuyRod(
            rodIndex
        );
    }
}