using System;
using CHG._02.Script;
using CHG._02.Script.FishSystem;
using UnityEngine;

namespace NKT.Fishing.Bait
{
    public enum BaitGrade
    {
        Common, Rare, Epic, Legendary
    }

    [Serializable]
    public struct GradeWeight
    {
        public Grade grade;
        public float weight;
    }
    
    [CreateAssetMenu(fileName = "Bait SO", menuName = "KT/Fishing/Bait", order = 0)]
    public class BaitSO : ScriptableObject
    {
        public string baitName;
        public Sprite baitSprite;

        public BaitGrade grade;
        public ShopItemRarity rarity;

        public int maxUses = 1;
        public bool isInfinite => maxUses <= 0;
        
        public GradeWeight[] weights;
        
        [Header("보스 미끼용")]
        public FishDataSO bossFishData;

        [Header("필요한물고기")]
        public FishDataSO[] fishesToBuy;
    }
}