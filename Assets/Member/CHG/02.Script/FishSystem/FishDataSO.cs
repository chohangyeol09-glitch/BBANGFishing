using System;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using CHG._02.Script.CoreSystem;
using DevLib.AnimatorSystem;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    [CreateAssetMenu(
        fileName = "Fish data",
        menuName = "CHG/Fish/Fish data",
        order = 0
    )]
    public class FishDataSO : ScriptableObject
    {
        // =========================================
        // 기존 FishDataSO 내용
        // =========================================

        public string Name;

        public Grade Grade;

        public float Health;

        [Tooltip("낚아 올릴 때 적용하는 기준 무게. 무거울수록 상승 속도가 낮아진다.")]
        [Min(0.01f)] public float Weight;

        [Tooltip("이전 데이터 호환용. 낚아 올리는 힘은 낚싯대 SO의 power와 Weight로 계산한다.")]
        [HideInInspector] public float JumpPower;


        public SkillEntry[] Skills;

        public float DamageMultiplier = 1f;


        public Sprite Sprite;

        public int Price;


        [Header("Lunge")]
        public bool CanLunge;

        public float LungeFlightTime = 1f;

        public float LungeFrontDistance = 1.5f;

        public float LungeHeightOffset;

        public float ParryRange = 3f;

        public float ReturnFlightTime = 1f;



        // =========================================
        // 인벤토리 / 판매용 추가 정보
        // =========================================

        [Header("물고기 설명")]

        [TextArea(3, 6)]
        public string History;


        [Header("랜덤 무게")]

        public float MinWeight = 1f;

        public float MaxWeight = 5f;



        // =========================================
        // 기존 FishSO 호환용
        // =========================================

        public string FishName =>
            Name;


        public Sprite FishSprite =>
            Sprite;


        public Grade Rarity =>
            Grade;


        public int BasePrice =>
            Price;



        // =========================================
        // 실제 잡힌 물고기 무게 생성
        // =========================================

        public float GetRandomWeight()
        {
            return UnityEngine.Random.Range(
                MinWeight,
                MaxWeight
            );
        }



        // =========================================
        // 실제 무게에 따른 판매 가격
        // =========================================

        public int GetPrice(float actualWeight)
        {
            // 기준 무게가 잘못 설정된 경우
            if (Weight <= 0f)
            {
                return Price;
            }


            float weightRatio =
                actualWeight / Weight;


            return Mathf.RoundToInt(
                Price * weightRatio
            );
        }



        private void OnValidate()
        {
            if (MinWeight < 0f)
            {
                MinWeight = 0f;
            }


            if (MaxWeight < MinWeight)
            {
                MaxWeight = MinWeight;
            }


            if (Price < 0)
            {
                Price = 0;
            }
        }
    }
}
