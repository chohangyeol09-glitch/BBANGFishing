using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.JJK._02._Scripts.Skill
{
    public class PlayerSkillController : MonoBehaviour
    {
        [Serializable]
        private class SkillSlot
        {
            public SkillSO skill;
            public Key key = Key.Digit1;

            [NonSerialized] public float cooldownEndTime;
            [NonSerialized] public float activeEndTime;
            [NonSerialized] public bool isActive;
        }

        [SerializeField] private List<SkillSlot> skillSlots = new();

        private PlayerSkillContext _context;
        private PlayerCombatState _combatState;

        private void Awake()
        {
            _combatState = GetComponent<PlayerCombatState>();
            _context = new PlayerSkillContext(_combatState);
        }

        private void Update()
        {
            HandleInput();
            TickActiveSkills();
        }

        private void OnDestroy()
        {
            foreach (SkillSlot slot in skillSlots)
            {
                if (!slot.isActive) continue;

                slot.skill.OnDeactivate(_context);
                slot.isActive = false;
            }
        }

        private void HandleInput()
        {
            foreach (SkillSlot slot in skillSlots)
            {
                if (slot.skill == null) continue;
                if (Keyboard.current[slot.key].wasPressedThisFrame)
                    TryActivate(slot);
            }
        }

        private void TryActivate(SkillSlot slot)
        {
            // 스킬 쿨다운/지속시간은 시간 느려짐 스킬 등 timeScale 변화의 영향을 받지 않도록 unscaledTime 기준으로 관리한다.
            if (Time.unscaledTime < slot.cooldownEndTime) return;
            if (slot.isActive) return;

            slot.skill.OnActivate(_context);
            slot.cooldownEndTime = Time.unscaledTime + slot.skill.Cooldown;

            if (slot.skill.HasDuration)
            {
                slot.isActive = true;
                slot.activeEndTime = Time.unscaledTime + slot.skill.Duration;
            }
        }

        private void TickActiveSkills()
        {
            foreach (SkillSlot slot in skillSlots)
            {
                if (!slot.isActive) continue;

                float elapsedTime = slot.skill.Duration - (slot.activeEndTime - Time.unscaledTime);
                slot.skill.OnUpdate(_context, elapsedTime);

                if (Time.unscaledTime >= slot.activeEndTime)
                {
                    slot.skill.OnDeactivate(_context);
                    slot.isActive = false;
                }
            }
        }

        public int SlotCount => skillSlots.Count;

        public SkillSO GetSkill(int slotIndex) => skillSlots[slotIndex].skill;

        public Key GetKey(int slotIndex) => skillSlots[slotIndex].key;

        public bool IsActive(int slotIndex) => skillSlots[slotIndex].isActive;

        public float GetCooldownRemaining(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= skillSlots.Count) return 0f;
            return Mathf.Max(0f, skillSlots[slotIndex].cooldownEndTime - Time.unscaledTime);
        }
    }
}
