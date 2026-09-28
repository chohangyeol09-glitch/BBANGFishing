using System;
using UnityEngine;

[Serializable]
public class UpgradeStatData
{
    [Header("종류")]
    public UpgradeStatType statType;


    [Header("UI 정보")]
    public Sprite statSprite;

    public string upgradeName;

    [TextArea(2, 4)]
    public string description;


    [Header("레벨")]
    public int currentLevel = 1;

    public int maxLevel = 10;


    [Header("능력치")]
    public float baseValue = 1f;

    public float valueIncreasePerLevel = 1f;

    public string valueSuffix;


    [Header("비용")]
    public int baseCost = 100;

    public int costIncreasePerLevel = 50;


    public float CurrentValue
    {
        get
        {
            return baseValue +
                   valueIncreasePerLevel *
                   (currentLevel - 1);
        }
    }


    public int CurrentCost
    {
        get
        {
            return baseCost +
                   costIncreasePerLevel *
                   (currentLevel - 1);
        }
    }


    public bool IsMaxLevel
    {
        get
        {
            return currentLevel >= maxLevel;
        }
    }


    public bool Upgrade()
    {
        if (IsMaxLevel)
            return false;


        currentLevel++;

        return true;
    }
}