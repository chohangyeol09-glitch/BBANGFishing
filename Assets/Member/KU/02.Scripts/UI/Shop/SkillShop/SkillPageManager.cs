using System.Collections.Generic;
using Member.JJK._02._Scripts.Skill;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPageManager : MonoBehaviour
{
    [Header("판매할 스킬들")]
    [SerializeField]
    private SkillSO[] skillPool;


    [Header("실제 장착 대상")]
    [SerializeField]
    private PlayerSkillController playerSkillController;


    [Header("왼쪽 - 현재 나온 스킬")]
    [SerializeField]
    private Image currentSkillImage;

    [SerializeField]
    private TMP_Text currentSkillNameText;

    [SerializeField]
    private TMP_Text currentSkillDescriptionText;


    [Header("리롤")]
    [SerializeField]
    private Button rerollButton;

    [SerializeField]
    private TMP_Text rerollPriceText;

    [SerializeField]
    private int baseRerollPrice = 1000;

    [SerializeField]
    private int rerollPriceIncrease = 2000;


    [Header("구매")]
    [SerializeField]
    private Button buyButton;

    [SerializeField]
    private TMP_Text buyPriceText;


    [Header("장착 슬롯")]
    [SerializeField]
    private SkillSlotUI[] skillSlots;


    private readonly List<SkillRuntimeData>
        equippedSkills =
        new List<SkillRuntimeData>();


    private SkillSO currentSkill;

    private int currentRerollPrice;


    public int EquippedSkillCount =>
        equippedSkills.Count;


    private void Awake()
    {
        currentRerollPrice =
            baseRerollPrice;


        if (rerollButton != null)
        {
            rerollButton.onClick
                .RemoveAllListeners();


            rerollButton.onClick
                .AddListener(
                    Reroll
                );
        }


        if (buyButton != null)
        {
            buyButton.onClick
                .RemoveAllListeners();


            buyButton.onClick
                .AddListener(
                        BuyCurrentSkill
                );
        }


        InitializeSlots();
    }


    private void OnEnable()
    {
        if (currentSkill == null)
        {
            RollRandomSkill();
        }


        RefreshCurrentSkillUI();

        RefreshRerollUI();
    }


    private void InitializeSlots()
    {
        if (skillSlots == null)
            return;


        for (int i = 0;
             i < skillSlots.Length;
             i++)
        {
            if (skillSlots[i] != null)
            {
                skillSlots[i]
                    .SetEmpty();
            }
        }
    }


    // =========================
    // 랜덤 스킬
    // =========================

    private void RollRandomSkill()
    {
        List<SkillSO> availableSkills =
            new List<SkillSO>();


        for (int i = 0;
             i < skillPool.Length;
             i++)
        {
            SkillSO skill =
                skillPool[i];


            if (skill == null)
                continue;


            if (IsEquipped(skill))
                continue;


            availableSkills.Add(
                skill
            );
        }


        if (availableSkills.Count == 0)
        {
            currentSkill = null;

            RefreshCurrentSkillUI();

            return;
        }


        int randomIndex =
            Random.Range(
                0,
                availableSkills.Count
            );


        currentSkill =
            availableSkills[randomIndex];


        RefreshCurrentSkillUI();
    }


    private bool IsEquipped(
        SkillSO skill)
    {
        for (int i = 0;
             i < equippedSkills.Count;
             i++)
        {
            if (equippedSkills[i].Skill ==
                skill)
            {
                return true;
            }
        }


        return false;
    }


    // =========================
    // 리롤
    // =========================

    private void Reroll()
    {
        if (MoneyManager.Instance == null)
            return;


        if (currentSkill == null)
            return;


        bool success =
            MoneyManager.Instance
                .TrySpendMoney(
                    currentRerollPrice
                );


        if (!success)
            return;


        currentRerollPrice +=
            rerollPriceIncrease;


        SkillSO previousSkill =
            currentSkill;


        RollRandomSkill();


        // 가능한 경우 방금 나온 것과
        // 같은 스킬은 다시 한 번 돌려봄
        if (currentSkill == previousSkill &&
            GetAvailableSkillCount() > 1)
        {
            RollRandomSkill();
        }


        RefreshRerollUI();
    }


    private int GetAvailableSkillCount()
    {
        int count = 0;


        for (int i = 0;
             i < skillPool.Length;
             i++)
        {
            if (skillPool[i] == null)
                continue;


            if (!IsEquipped(skillPool[i]))
            {
                count++;
            }
        }


        return count;
    }


    // =========================
    // 구매
    // =========================

    private void BuyCurrentSkill()
    {
        if (currentSkill == null)
            return;


        if (equippedSkills.Count >= 3)
        {
            Debug.Log(
                "스킬은 최대 3개까지 장착할 수 있습니다."
            );

            return;
        }


        if (MoneyManager.Instance == null)
            return;


        bool success =
            MoneyManager.Instance
                .TrySpendMoney(
                    currentSkill.PurchasePrice
                );


        if (!success)
            return;


        SkillRuntimeData newSkill =
            new SkillRuntimeData(
                currentSkill
            );


        equippedSkills.Add(
            newSkill
        );


        SkillSlotUI emptySlot =
            FindEmptySlot();


        if (emptySlot != null)
        {
            emptySlot.Setup(
                newSkill,
                this
            );
        }


        ApplySkillEffect(
            newSkill
        );


        Debug.Log(
            $"{currentSkill.SkillName} 구매 및 장착"
        );


        // 구매하면 리롤 가격 초기화
        currentRerollPrice =
            baseRerollPrice;


        // 다음 스킬 등장
        RollRandomSkill();


        RefreshRerollUI();

        RefreshCurrentSkillUI();
    }


    private SkillSlotUI FindEmptySlot()
    {
        if (skillSlots == null)
            return null;


        for (int i = 0;
             i < skillSlots.Length;
             i++)
        {
            if (skillSlots[i] != null &&
                skillSlots[i].IsEmpty)
            {
                return skillSlots[i];
            }
        }


        return null;
    }


    // =========================
    // 업그레이드
    // =========================

    // JJK SkillSO는 레벨/수치 개념이 없어서 업그레이드는 지원하지 않는다.
    // (SkillSlotUI가 항상 MAX로 표시하고 업그레이드 버튼을 꺼두기 때문에 평소엔 호출될 일이 없다.)
    public void UpgradeSkill(
        SkillRuntimeData skillData,
        SkillSlotUI slotUI)
    {
        Debug.Log(
            "이 스킬은 업그레이드를 지원하지 않습니다."
        );
    }


    private void ApplySkillEffect(
        SkillRuntimeData skillData)
    {
        if (skillData == null ||
            skillData.Skill == null)
            return;


        if (playerSkillController != null)
        {
            playerSkillController.EquipSkill(
                skillData.Skill
            );
        }


        Debug.Log(
            $"{skillData.Skill.SkillName} 장착"
        );
    }


    // =========================
    // UI
    // =========================

    private void RefreshCurrentSkillUI()
    {
        if (currentSkill == null)
        {
            if (currentSkillImage != null)
            {
                currentSkillImage.enabled =
                    false;
            }


            if (currentSkillNameText != null)
            {
                currentSkillNameText.text =
                    "스킬 없음";
            }


            if (currentSkillDescriptionText != null)
            {
                currentSkillDescriptionText.text =
                    "";
            }


            if (buyPriceText != null)
            {
                buyPriceText.text =
                    "-";
            }


            if (buyButton != null)
            {
                buyButton.interactable =
                    false;
            }


            return;
        }


        if (currentSkillImage != null)
        {
            currentSkillImage.enabled =
                true;


            currentSkillImage.sprite =
                currentSkill.Icon;
        }


        if (currentSkillNameText != null)
        {
            currentSkillNameText.text =
                currentSkill.SkillName;
        }


        if (currentSkillDescriptionText != null)
        {
            currentSkillDescriptionText.text =
                currentSkill.Description;
        }


        if (buyPriceText != null)
        {
            buyPriceText.text =
                currentSkill
                    .PurchasePrice
                    .ToString("N0");
        }


        if (buyButton != null)
        {
            buyButton.interactable =
                equippedSkills.Count < 3;
        }
    }


    private void RefreshRerollUI()
    {
        if (rerollPriceText != null)
        {
            rerollPriceText.text =
                currentRerollPrice
                    .ToString("N0");
        }


        if (rerollButton != null)
        {
            rerollButton.interactable =
                currentSkill != null &&
                equippedSkills.Count < 3;
        }
    }
}
