using UnityEngine;

[CreateAssetMenu(fileName = "New Fish",menuName = "KU/Fish")]
public class FishSO : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField]
    private string fishName;

    [SerializeField]
    private Sprite fishSprite;

    [TextArea(3, 6)]
    [SerializeField]
    private string history;


    [Header("희귀도")]
    [SerializeField]
    private FishRarity rarity;


    [Header("무게")]
    [SerializeField]
    private float minWeight = 1f;

    [SerializeField]
    private float maxWeight = 5f;


    [Header("가격")]
    [SerializeField]
    private int basePrice = 5;


    public string FishName => fishName;

    public Sprite FishSprite => fishSprite;

    public string History => history;

    public FishRarity Rarity => rarity;

    public float MinWeight => minWeight;

    public float MaxWeight => maxWeight;

    public int BasePrice => basePrice;


    public float GetRandomWeight()
    {
        return Random.Range(
            minWeight,
            maxWeight
        );
    }


    public int GetPrice(float weight)
    {
        return Mathf.RoundToInt(
            basePrice * weight
        );
    }


    private void OnValidate()
    {
        if (minWeight < 0f)
        {
            minWeight = 0f;
        }

        if (maxWeight < minWeight)
        {
            maxWeight = minWeight;
        }

        if (basePrice < 0)
        {
            basePrice = 0;
        }
    }
}