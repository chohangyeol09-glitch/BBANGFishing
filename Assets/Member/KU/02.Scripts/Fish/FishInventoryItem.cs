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



    public FishDataSO FishData =>
        fishData;


    public float Weight =>
        weight;



    public string FishName
    {
        get
        {
            if (fishData == null)
                return "";

            return fishData.FishName;
        }
    }



    public string FishContent
    {
        get
        {
            if (fishData == null)
                return "";

            return fishData.History;
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

            return fishData.GetPrice(
                weight
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