using System;
using CHG._02.Script.CoreSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace NKT.Fishing.MiniGame
{
    public class SkillCheckMiniGame : MultiStepMiniGame
    {
        [SerializeField] private JudgementPopup popup;

        [Header("UI")]
        [SerializeField] private RectTransform needle;
        [SerializeField] private RectTransform goodZone;
        [SerializeField] private RectTransform perfectZone;
        [SerializeField] private Image goodZoneImage;
        [SerializeField] private Image perfectZoneImage;

        [Header("난이도")]
        [SerializeField] private Vector2 speedRange = new Vector2(220f, 400f);
        [SerializeField] private Vector2 goodSizeRange = new Vector2(60f, 32f);
        [SerializeField] private Vector2 perfectSizeRange = new Vector2(18f, 9f);
        [SerializeField] private Vector2 zoneStartRange = new Vector2(120f, 290f);
        [SerializeField] private float noteInterval = 0.4f;

        private float _speed;
        private float _goodSize;
        private float _perfectSize;

        private float _angle;
        private float _zoneStart;
        private float _perfectStart;
        private float _delayTimer;
        private bool _active;
        
        protected override void OnGameStart(Grade grade, float f)
        {
            _speed = Mathf.Lerp(speedRange.x, speedRange.y, f);
            _goodSize = Mathf.Lerp(goodSizeRange.x, goodSizeRange.y, f);
            _perfectSize = Mathf.Lerp(perfectSizeRange.x, perfectSizeRange.y, f);

            HideCheck();
        }

        protected override void NextStep()
        {
            _active = false;
            _delayTimer = noteInterval;
            HideCheck();
        }

        private void Update()
        {
            if (IsDone) return;

            if (!_active)
            {
                _delayTimer -= Time.deltaTime;
                if (_delayTimer <= 0f) SpawnCheck();
                return;
            }
            
            _angle += _speed * Time.deltaTime;
            needle.localRotation = Quaternion.Euler(0f, 0f, -_angle);

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Judge();
                return;
            }

            if (_angle >= 360f)
            {
                _active = false;
                popup.Show(Vector2.zero, Judgement.Miss);
                HideCheck();
                ReportStep(false);
            }
        }

        private void SpawnCheck()
        {
            _zoneStart = Random.Range(zoneStartRange.x, zoneStartRange.y);
            _perfectStart = _zoneStart + (_goodSize - _perfectSize) * 0.5f;
            _angle = 0f;

            goodZoneImage.fillAmount = _goodSize / 360f;
            perfectZoneImage.fillAmount = _perfectSize / 360f;
            
            goodZone.localRotation = Quaternion.Euler(0f, 0f, -_zoneStart);
            perfectZone.localRotation = Quaternion.Euler(0f, 0f, -_perfectStart);
            needle.localRotation = Quaternion.identity;
            
            needle.gameObject.SetActive(true);
            goodZone.gameObject.SetActive(true);
            perfectZone.gameObject.SetActive(true);
            
            _active = true;
        }

        private void Judge()
        {
            _active = false;
            HideCheck();

            float rel = _angle - _zoneStart;
            float perfectRel = _angle - _perfectStart;

            if (perfectRel >= 0f && perfectRel <= _perfectSize)
            {
                popup.Show(Vector2.zero, Judgement.Perfect);
                ReportStep(true);
            }
            else if (rel >= 0f && rel <= _goodSize)
            {
                popup.Show(Vector2.zero, Judgement.Good);
                ReportStep(true);
            }
            else
            {
                popup.Show(Vector2.zero, Judgement.Miss);
                ReportStep(false);
            }
        }

        private void HideCheck()
        {
            needle.gameObject.SetActive(false);
            goodZone.gameObject.SetActive(false);
            perfectZone.gameObject.SetActive(false);
        }
    }
}
