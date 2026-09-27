using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStatItemUI : MonoBehaviour
{
    [Header("이미지")]
    [SerializeField]
    private Image statImage;


    [Header("기본 정보")]
    [SerializeField]
    private TMP_Text upgradeNameText;

    [SerializeField]
    private TMP_Text descriptionText;


    [Header("능력치 정보")]
    [SerializeField]
    private TMP_Text currentValueText;

    [SerializeField]
    private TMP_Text levelText;

    [SerializeField]
    private TMP_Text costText;


    [Header("버튼")]
    [SerializeField]
    private Button upgradeButton;


    private UpgradeStatData statData;

    private UpgradePageManager upgradeManager;


    public void Setup(
        UpgradeStatData data,
        UpgradePageManager manager)
    {
        if (data == null)
            return;


        statData = data;

        upgradeManager = manager;


        if (upgradeButton != null)
        {
            upgradeButton.onClick
                .RemoveAllListeners();


            upgradeButton.onClick
                .AddListener(Upgrade);
        }


        Refresh();
    }


    public void Refresh()
    {
        if (statData == null)
            return;


        RefreshBasicInfo();

        RefreshStatInfo();

        RefreshButton();
    }


    private void RefreshBasicInfo()
    {
        if (statImage != null)
        {
            statImage.sprite =
                statData.statSprite;


            statImage.enabled =
                statData.statSprite != null;
        }


        if (upgradeNameText != null)
        {
            upgradeNameText.text =
                statData.upgradeName;
        }


        if (descriptionText != null)
        {
            descriptionText.text =
                statData.description;
        }
    }


    private void RefreshStatInfo()
    {
        if (currentValueText != null)
        {
            currentValueText.text =
                $"현재 : {GetValueText()}";
        }


        if (levelText != null)
        {
            levelText.text =
                $"Lv. {statData.currentLevel}";
        }


        if (costText != null)
        {
            if (statData.IsMaxLevel)
            {
                costText.text =
                    "MAX";
            }
            else
            {
                costText.text =
                    $"{statData.CurrentCost}$";
            }
        }
    }


    private void RefreshButton()
    {
        if (upgradeButton == null)
            return;


        upgradeButton.interactable =
            !statData.IsMaxLevel;
    }


    private string GetValueText()
    {
        if (statData == null)
            return "";


        switch (statData.statType)
        {
            case UpgradeStatType.AttackSpeed:

                return
                    $"{statData.CurrentValue:0.0}" +
                    statData.valueSuffix;


            case UpgradeStatType.FishCapacity:

                return
                    $"{Mathf.RoundToInt(statData.CurrentValue)}" +
                    statData.valueSuffix;


            default:

                return
                    $"{statData.CurrentValue:0}" +
                    statData.valueSuffix;
        }
    }


    private void Upgrade()
    {
        if (statData == null)
            return;


        if (upgradeManager == null)
            return;


        upgradeManager.UpgradeStat(
            statData,
            this
        );
    }
}