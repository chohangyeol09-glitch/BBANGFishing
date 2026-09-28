using NKT.Fishing.Bait;
using NKT.Player.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKT.UI
{
    //지금 끼운 미끼와 남은 횟수를 보여준다
    public class BaitHudUI : MonoBehaviour
    {
        [SerializeField] private BaitModule bait;

        [Header("UI")]
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText;

        [Header("표시")]
        [SerializeField] private string infiniteLabel = "∞";
        [SerializeField] private int lowCount = 3;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color lowColor = new Color(1f, 0.45f, 0.35f);

        private void OnEnable()
        {
            if (bait == null) return;

            bait.OnBaitChange += Refresh;
        }

        private void OnDisable()
        {
            if (bait == null) return;

            bait.OnBaitChange -= Refresh;
        }

        private void Start()
        {
            if (bait == null)
            {
                Debug.LogWarning("BaitHudUI : BaitModule이 연결되지 않았습니다.", this);
                return;
            }

            //OnBaitChange는 바뀔 때만 온다. 시작 상태는 직접 읽어와야 한다
            Refresh(bait.CurrentBait, bait.RemainingUses);
        }

        private void Refresh(BaitSO data, int remaining)
        {
            if (data == null)
            {
                Clear();
                return;
            }

            if (icon != null)
            {
                icon.sprite = data.baitSprite;
                icon.enabled = data.baitSprite != null;
            }

            if (nameText != null)
                nameText.text = data.baitName;

            if (countText == null) return;

            //무제한 미끼는 RemainingUses가 -1로 온다
            bool infinite = remaining < 0;

            countText.text = infinite ? infiniteLabel : remaining.ToString();
            countText.color = !infinite && remaining <= lowCount ? lowColor : normalColor;
        }

        private void Clear()
        {
            if (icon != null) icon.enabled = false;
            if (nameText != null) nameText.text = string.Empty;
            if (countText != null) countText.text = string.Empty;
        }
    }
}
