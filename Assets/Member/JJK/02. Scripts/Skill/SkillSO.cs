using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public abstract class SkillSO : ScriptableObject
    {
        [field: SerializeField]
        public string SkillName { get; private set; } = "New Skill";

        [field: SerializeField]
        public float Cooldown { get; private set; } = 5f;

        [field: SerializeField]
        public float Duration { get; private set; } = 3f;

        [field: SerializeField]
        public Sprite Icon { get; private set; }


        public virtual bool HasDuration =>
            Duration > 0f;


        public abstract void OnActivate(
            PlayerSkillContext context
        );


        public virtual void OnUpdate(
            PlayerSkillContext context,
            float elapsedTime)
        {
        }


        public virtual void OnDeactivate(
            PlayerSkillContext context)
        {
        }



        // =========================================
        // 상점용 추가 데이터
        // =========================================

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



        // =========================================
        // 기존 UI 코드 호환용
        // =========================================

        public string Description =>
            GetDescription(
                1,
                false
            );


        public int PurchasePrice =>
            purchasePrice;



        // =========================================
        // 효과 값
        // =========================================

        public float GetValue(
            int level)
        {
            return baseValue +
                   valueIncreasePerLevel *
                   (level - 1);
        }



        // =========================================
        // 업그레이드 가격
        // =========================================

        public int GetUpgradeCost(
            int level)
        {
            return baseUpgradeCost +
                   upgradeCostIncrease *
                   (level - 1);
        }



        // =========================================
        // 설명
        // =========================================

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


            if (string.IsNullOrEmpty(
                descriptionFormat))
            {
                return "";
            }


            return descriptionFormat.Replace(
                "{value}",
                valueText
            );
        }
    }
}