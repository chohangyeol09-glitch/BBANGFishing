using Member.JJK._02._Scripts.Skill;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [Header("슬롯 상태")]
    [SerializeField]
    private GameObject slotOut;

    [SerializeField]
    private GameObject slotIn;


    [Header("장착된 스킬")]
    [SerializeField]
    private Image skillIcon;

    [SerializeField]
    private TMP_Text skillNameText;

    [SerializeField]
    private TMP_Text levelText;

    [SerializeField]
    private TMP_Text descriptionText;


    [Header("업그레이드")]
    [SerializeField]
    private Button upgradeButton;

    [SerializeField]
    private TMP_Text upgradeCostText;


    private SkillRuntimeData skillData;

    private SkillPageManager manager;


    public bool IsEmpty =>
        skillData == null;


    public SkillRuntimeData SkillData =>
        skillData;


    private void Awake()
    {
        SetEmpty();
    }


    public void Setup(
        SkillRuntimeData data,
        SkillPageManager skillManager)
    {
        if (data == null ||
            data.Skill == null)
            return;


        skillData = data;

        manager = skillManager;


        if (slotOut != null)
        {
            slotOut.SetActive(false);
        }


        if (slotIn != null)
        {
            slotIn.SetActive(true);
        }


        if (upgradeButton != null)
        {
            upgradeButton.onClick
                .RemoveAllListeners();


            upgradeButton.onClick
                .AddListener(
                    Upgrade
                );
        }


        Refresh();
    }


    public void Refresh()
    {
        if (skillData == null ||
            skillData.Skill == null)
            return;


        SkillSO skill =
            skillData.Skill;


        if (skillIcon != null)
        {
            skillIcon.sprite =
                skill.Icon;
        }


        if (skillNameText != null)
        {
            skillNameText.text =
                skill.SkillName;
        }


        if (levelText != null)
        {
            levelText.text =
                $"Lv.{skillData.Level}";
        }


        if (descriptionText != null)
        {
            descriptionText.richText = true;


            descriptionText.text =
                skill.Description;
        }


        // JJK SkillSO는 레벨 개념이 없어서 업그레이드는 지원하지 않는다.
        if (upgradeCostText != null)
        {
            upgradeCostText.text =
                "MAX";
        }


        if (upgradeButton != null)
        {
            upgradeButton.interactable =
                false;
        }
    }


    public void SetEmpty()
    {
        skillData = null;


        if (slotOut != null)
        {
            slotOut.SetActive(true);
        }


        if (slotIn != null)
        {
            slotIn.SetActive(false);
        }
    }


    private void Upgrade()
    {
        if (manager == null)
            return;


        if (skillData == null)
            return;


        manager.UpgradeSkill(
            skillData,
            this
        );
    }
}
