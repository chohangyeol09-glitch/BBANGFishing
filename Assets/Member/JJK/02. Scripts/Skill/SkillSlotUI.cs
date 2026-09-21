using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.Skill
{
    public class SkillSlotUI : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private TMP_Text keyText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Color activeColor = new Color(1f, 0.85f, 0.2f, 1f);

        private Color _backgroundColor = Color.white;
        private float _cooldown;

        public void Bind(SkillSO skill, string keyLabel)
        {
            _cooldown = skill.Cooldown;

            if (background != null)
                _backgroundColor = background.color;

            if (icon != null)
            {
                icon.sprite = skill.Icon;
                icon.enabled = skill.Icon != null;
            }

            if (nameText != null)
            {
                nameText.text = skill.SkillName;
                nameText.gameObject.SetActive(skill.Icon == null);
            }

            if (keyText != null)
                keyText.text = keyLabel;

            if (cooldownOverlay != null)
            {
                cooldownOverlay.type = Image.Type.Filled;
                cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
                cooldownOverlay.fillOrigin = (int)Image.Origin360.Top;
                cooldownOverlay.fillClockwise = false;
            }

            Refresh(0f, false);
        }

        public void Refresh(float cooldownRemaining, bool isActive)
        {
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = _cooldown > 0f ? Mathf.Clamp01(cooldownRemaining / _cooldown) : 0f;

            if (cooldownText != null)
            {
                cooldownText.text = cooldownRemaining <= 0f ? string.Empty
                    : cooldownRemaining >= 1f ? Mathf.CeilToInt(cooldownRemaining).ToString()
                    : cooldownRemaining.ToString("0.0");
            }

            if (background != null)
                background.color = isActive ? activeColor : _backgroundColor;
        }
    }
}
