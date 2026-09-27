using System;
using CHG._02.Script.FishSystem;
using UnityEngine;

[Serializable]
public class FishInventoryItem
{
    [SerializeField]
    private FishDataSO fishData;

    [SerializeField]
    private float weight;


    public FishDataSO FishData => fishData;

    public float Weight => weight;


    public string FishName
    {
        get
        {
            if (fishData == null)
                return "";

            return fishData.name;
        }
    }


    public Sprite FishSprite
    {
        get
        {
            if (fishData == null)
                return null;

            return fishData.Sprite;
        }
    }


    public int Price
    {
        get
        {
            if (fishData == null)
                return 0;


            // 기준 무게가 0 이하라면
            // 비례 계산 불가능하므로 기본 가격 반환
            if (fishData.Weight <= 0f)
            {
                return fishData.Price;
            }


            float weightRatio =
                weight / fishData.Weight;


            return Mathf.RoundToInt(
                fishData.Price * weightRatio
            );
        }
    }


    public FishInventoryItem(
        FishDataSO fishData,
        float weight)
    {
        this.fishData = fishData;
        this.weight = weight;
    }
}