using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NKT.Fishing.Rob
{
    [CreateAssetMenu(fileName = "FishingRob SO", menuName = "KT/Fishing/Rob", order = 0)]
    public class FishingRobSO : ScriptableObject
    {
        
        public RobObject prefab;
        public float power = 12;
        
        public float maxDistance;
        public float minDistance;

        public float minBiteDelay;
        public float maxBiteDelay;

        public string rodName;
        public Sprite rodSprite;
        public ShopItemRarity rarity;
        public float price;
        public RobGrade grade;

        public float GetBiteDelay()
        {
            return Random.Range(minBiteDelay, maxBiteDelay);
        }
    }

    [Serializable]
    public struct RobObject
    {
        public GameObject RobGameobject;
        public GameObject BobberGameobject;
    }

    public enum RobGrade
    {
        Common, Rare, Epic, Legendary
    }
}