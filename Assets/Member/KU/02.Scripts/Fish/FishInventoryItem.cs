using System;
using UnityEngine;

[Serializable]
public class FishInventoryItem
{
    [SerializeField]
    private FishSO fish;

    [SerializeField]
    private float weight;


    public FishSO Fish => fish;

    public float Weight => weight;

    public int Price
    {
        get
        {
            if (fish == null)
                return 0;

            return fish.GetPrice(weight);
        }
    }


    public FishInventoryItem(
        FishSO fish,
        float weight)
    {
        this.fish = fish;
        this.weight = weight;
    }
}