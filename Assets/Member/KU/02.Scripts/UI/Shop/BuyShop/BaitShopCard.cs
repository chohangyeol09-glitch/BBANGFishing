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

    [SerializeField]
    private TextMeshProUGUI requiredFishCountText;


    [Header("카드 배경")]
    [SerializeField]
    private Image backgroundImage;


    [Header("등급별 배경 색상")]
    [SerializeField]
    private Color commonColor =
        new Color32(
            220,
            220,
            220,
            255
        );

    [SerializeField]
    private Color rareColor =
        new Color32(
            112,
            183,
            255,
            255
        );

    [SerializeField]
    private Color epicColor =
        new Color32(
            185,
            131,
            255,
            255
        );

    [SerializeField]
    private Color legendaryColor =
        new Color32(
            255,
            213,
            106,
            255
        );


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



    private void OnEnable()
    {
        // 인벤토리가 변하면
        // 필요한 물고기 숫자 즉시 갱신
        if (FishInventoryManager.Instance != null)
        {
            FishInventoryManager.Instance
                .OnInventoryChanged +=
                RefreshRequiredFishUI;
        }


        RefreshRequiredFishUI();
    }



    private void OnDisable()
    {
        if (FishInventoryManager.Instance != null)
        {
            FishInventoryManager.Instance
                .OnInventoryChanged -=
                RefreshRequiredFishUI;
        }
    }



    public void Setup(
        BaitSO bait)
    {
        if (bait == null)
            return;


        baitData =
            bait;


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
                requiredFishImage
                    .gameObject
                    .SetActive(true);


                requiredFishImage.sprite =
                    bait.fishesToBuy.Sprite;
            }
            else
            {
                requiredFishImage
                    .gameObject
                    .SetActive(false);
            }
        }


        ApplyGradeColor(
            bait.grade
        );


        RefreshRequiredFishUI();
    }



    // =========================================
    // 필요한 물고기 UI 갱신
    // =========================================

    private void RefreshRequiredFishUI()
    {
        if (baitData == null)
            return;


        // 필요한 물고기가 없는 미끼
        if (baitData.fishesToBuy == null)
        {
            if (requiredFishImage != null)
            {
                requiredFishImage
                    .gameObject
                    .SetActive(false);
            }


            if (requiredFishCountText != null)
            {
                requiredFishCountText
                    .gameObject
                    .SetActive(false);
            }


            return;
        }


        if (requiredFishImage != null)
        {
            requiredFishImage
                .gameObject
                .SetActive(true);


            requiredFishImage.sprite =
                baitData
                    .fishesToBuy
                    .Sprite;
        }


        int currentFishCount = 0;


        if (FishInventoryManager.Instance != null)
        {
            currentFishCount =
                FishInventoryManager.Instance
                    .GetFishCount(
                        baitData.fishesToBuy
                    );
        }


        if (requiredFishCountText != null)
        {
            requiredFishCountText
                .gameObject
                .SetActive(true);


            // 미끼 하나당 물고기 1마리 필요
            requiredFishCountText.text =
                $"{currentFishCount} / 1";
        }
    }



    // =========================================
    // 등급별 배경 색상
    // =========================================

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



    // =========================================
    // 구매
    // =========================================

    private void OnClickBuy()
    {
        if (baitData == null)
            return;


        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning(
                "MoneyManager가 없습니다."
            );

            return;
        }


        if (FishInventoryManager.Instance == null)
        {
            Debug.LogWarning(
                "FishInventoryManager가 없습니다."
            );

            return;
        }



        // =========================================
        // 필요한 물고기 확인
        // =========================================

        if (baitData.fishesToBuy != null)
        {
            bool hasFish =
                FishInventoryManager.Instance
                    .HasFish(
                        baitData.fishesToBuy,
                        1
                    );


            if (!hasFish)
            {
                Debug.Log(
                    $"{baitData.baitName} 구매 실패 : " +
                    $"{baitData.fishesToBuy.Name}이 필요합니다."
                );

                return;
            }
        }



        // =========================================
        // 돈 확인
        // =========================================

        if (!MoneyManager.Instance
            .CanAfford(
                baitData.price
            ))
        {
            Debug.Log(
                $"{baitData.baitName} 구매 실패 : " +
                $"돈이 부족합니다."
            );

            return;
        }



        // =========================================
        // 돈 차감
        // =========================================

        bool moneySuccess =
            MoneyManager.Instance
                .TrySpendMoney(
                    baitData.price
                );


        if (!moneySuccess)
            return;



        // =========================================
        // 필요한 물고기 1마리 소비
        // =========================================

        if (baitData.fishesToBuy != null)
        {
            bool fishSuccess =
                FishInventoryManager.Instance
                    .ConsumeFish(
                        baitData.fishesToBuy,
                        1
                    );


            // 혹시 실패했으면 돈 환불
            if (!fishSuccess)
            {
                MoneyManager.Instance
                    .AddMoney(
                        baitData.price
                    );


                Debug.LogWarning(
                    "물고기 소비에 실패하여 돈을 환불했습니다."
                );

                return;
            }
        }



        // =========================================
        // 구매 성공
        // =========================================

        Debug.Log(
            $"{baitData.baitName} 구매 완료"
        );


        // 여기서 나중에 실제 미끼 지급 처리
        // 필요하면 연결하면 됨.
        //
        // 예:
        // BaitInventoryManager.Instance.AddBait(baitData);


        RefreshRequiredFishUI();
    }
}