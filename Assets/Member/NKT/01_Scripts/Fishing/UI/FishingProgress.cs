using System;
using System.Collections;
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
        }

        private void OnDestroy()
        {
            castCharger.OnChargeStarted -= Show;
            castCharger.OnValueChanged -= GaugeCharging;
            castCharger.OnCharged -= Hide;
        }

        private void Show()
        {
            backgroundTransform.gameObject.SetActive(true);
        }

        private void GaugeCharging(float obj)
        {
            float halfHeight = backgroundTransform.rect.height * 0.5f;

            Vector2 pos = arrowTransform.anchoredPosition;
            pos.y = Mathf.Lerp(-halfHeight, halfHeight, obj);
            arrowTransform.anchoredPosition = pos;

            progressSprite.fillAmount = obj;
        }

        private void Hide(float obj)
        {
            backgroundTransform.gameObject.SetActive(false);
        }
    }
}