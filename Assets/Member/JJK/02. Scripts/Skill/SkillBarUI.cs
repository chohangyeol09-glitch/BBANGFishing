using System.Collections.Generic;
using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public class SkillBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerSkillController skillController;
        [SerializeField] private List<SkillSlotUI> slotUIs = new();

        private readonly List<int> _boundSlotIndices = new();
        private readonly List<SkillSlotUI> _boundSlotUIs = new();

        private void Start()
        {
            skillController.SkillEquipped += OnSkillEquipped;

            for (int i = 0; i < slotUIs.Count; i++)
            {
                SkillSO skill = i < skillController.SlotCount ? skillController.GetSkill(i) : null;
                if (skill == null)
                {
                    slotUIs[i].SetEmpty(FormatKey(skillController.GetKey(i).ToString()));
                    continue;
                }

                BindSlot(i, skill);
            }
        }

        private void OnDestroy()
        {
            if (skillController != null)
                skillController.SkillEquipped -= OnSkillEquipped;
        }

        private void OnSkillEquipped(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slotUIs.Count) return;

            BindSlot(slotIndex, skillController.GetSkill(slotIndex));
        }

        private void BindSlot(int slotIndex, SkillSO skill)
        {
            slotUIs[slotIndex].Bind(skill, FormatKey(skillController.GetKey(slotIndex).ToString()));
            _boundSlotIndices.Add(slotIndex);
            _boundSlotUIs.Add(slotUIs[slotIndex]);
        }

        private void Update()
        {
            for (int n = 0; n < _boundSlotUIs.Count; n++)
            {
                int slotIndex = _boundSlotIndices[n];
                _boundSlotUIs[n].Refresh(skillController.GetCooldownRemaining(slotIndex), skillController.IsActive(slotIndex));
            }
        }

        private static string FormatKey(string keyName) =>
            keyName.StartsWith("Digit") ? keyName.Substring("Digit".Length) : keyName;
    }
}
