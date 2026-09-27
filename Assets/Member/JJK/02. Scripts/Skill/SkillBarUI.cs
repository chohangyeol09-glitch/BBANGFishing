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
            for (int i = 0; i < slotUIs.Count; i++)
            {
                SkillSO skill = i < skillController.SlotCount ? skillController.GetSkill(i) : null;
                if (skill == null)
                {
                    slotUIs[i].gameObject.SetActive(false);
                    continue;
                }

                slotUIs[i].Bind(skill, FormatKey(skillController.GetKey(i).ToString()));
                _boundSlotIndices.Add(i);
                _boundSlotUIs.Add(slotUIs[i]);
            }
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
