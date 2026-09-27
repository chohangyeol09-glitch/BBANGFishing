using UnityEngine;

[CreateAssetMenu(
    fileName = "New Skill",
    menuName = "Shop/Skill"
)]
public class KU_SkillSO : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;

    [TextArea(2, 4)]
    public string descriptionFormat;

    public Sprite skillSprite;


    [Header("구매")]
    public int purchasePrice = 10000;


    [Header("효과")]
    public float baseValue = 10f;

    public float valueIncreasePerLevel = 5f;

    public string valueSuffix = "%";


    [Header("레벨")]
    public int maxLevel = 5;


    [Header("업그레이드 비용")]
    public int baseUpgradeCost = 5000;

    public int upgradeCostIncrease = 5000;


    public float GetValue(int level)
    {
        return baseValue +
               valueIncreasePerLevel *
               (level - 1);
    }


    public int GetUpgradeCost(int level)
    {
        return baseUpgradeCost +
               upgradeCostIncrease *
               (level - 1);
    }


    public string GetDescription(
        int level,
        bool highlightValue)
    {
        float value =
            GetValue(level);


        string valueText =
            $"{value:0.#}{valueSuffix}";


        if (highlightValue)
        {
            valueText =
                $"<color=#000000>{valueText}</color>";
        }


        return descriptionFormat.Replace(
            "{value}",
            valueText
        );
    }
}