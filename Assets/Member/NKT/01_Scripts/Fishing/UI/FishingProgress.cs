using NKT.Fishing.Rob;
using UnityEngine;
using UnityEngine.UI;

namespace NKT.Fishing.UI
{
    public class FishingProgress : MonoBehaviour
    {
        [SerializeField] private CastCharger castCharger;
        [SerializeField] private Image progressSprite;
        [SerializeField] private RectTransform arrowTransform;
        [SerializeField] private RectTransform backgroundTransform;

        private void Awake()
        {
            castCharger.OnChargeStarted += Show;
            castCharger.OnValueChanged += GaugeCharging;
            castCharger.OnCharged += Hide;
            castCharger.OnChargeCanceled += Hide;
        }

        private void Start()
        {
            Hide();
        }

        private void OnDestroy()
        {
            if (castCharger == null) return;

            castCharger.OnChargeStarted -= Show;
            castCharger.OnValueChanged -= GaugeCharging;
            castCharger.OnCharged -= Hide;
            castCharger.OnChargeCanceled -= Hide;
        }

        private void Show()
        {
            backgroundTransform.gameObject.SetActive(true);
        }

        private void GaugeCharging(float power)
        {
            float halfHeight = backgroundTransform.rect.height * 0.5f;

            Vector2 pos = arrowTransform.anchoredPosition;
            pos.y = Mathf.Lerp(-halfHeight, halfHeight, power);
            arrowTransform.anchoredPosition = pos;

            progressSprite.fillAmount = power;
        }

        private void Hide(float power) => Hide();

        private void Hide()
        {
            backgroundTransform.gameObject.SetActive(false);
        }
    }
}
