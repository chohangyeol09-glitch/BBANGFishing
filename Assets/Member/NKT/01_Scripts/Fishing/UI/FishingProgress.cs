using NKT.Fishing.Rob;
using UnityEngine;
using UnityEngine.UI;

namespace NKT.Fishing.UI
{
    public class FishingProgress : MonoBehaviour
    {
        [SerializeField] private CastCharger castCharger;
        [SerializeField] private Image progressSprite;
        [SerializeField] private RectTransform backgroundTransform;
        [SerializeField] private RectTransform arrowPivot;
        [SerializeField] private float startAngle = 0f;
        [SerializeField] private float sweepAngle = -180f;

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
            progressSprite.fillAmount = power * 0.5f;
            
            float angle = startAngle + sweepAngle * power;
            arrowPivot.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void Hide(float power) => Hide();

        private void Hide()
        {
            backgroundTransform.gameObject.SetActive(false);
        }
    }
}
