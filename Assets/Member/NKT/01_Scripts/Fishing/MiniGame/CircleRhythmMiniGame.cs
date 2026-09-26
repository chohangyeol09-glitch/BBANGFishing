using System;
using CHG._02.Script.CoreSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace NKT.Fishing.MiniGame
{
    public class CircleRhythmMiniGame : MultiStepMiniGame
    {
        [SerializeField] private JudgementPopup popup;
        
        [Header("UI")]
        [SerializeField] private RectTransform area;
        [SerializeField] private RectTransform targetCircle;
        [SerializeField] private RectTransform approachCircle;

        [Header("크기")] 
        [SerializeField] private float targetSize = 120f;
        [SerializeField] private float startScale = 3.2f;
        
        [Header("난이도")]
        [SerializeField] private Vector2 shrinkTimeRange = new Vector2(1.4f, 0.7f);
        [SerializeField] private Vector2 perfectWindowRange = new Vector2(0.07f, 0.035f);
        [SerializeField] private Vector2 goodWindowRange = new Vector2(0.18f, 0.08f);
        [SerializeField] private float noteInterval = 0.35f;

        private float _shrinkTime;
        private float _perfectWindow;
        private float _goodWindow;
        
        private float _timer;
        private float _delayTimer;
        private bool _noteActive;


        protected override void OnGameStart(Grade grade, float f)
        {
            _shrinkTime = Mathf.Lerp(shrinkTimeRange.x, shrinkTimeRange.y, f);
            _perfectWindow = Mathf.Lerp(perfectWindowRange.x, perfectWindowRange.y, f);
            _goodWindow = Mathf.Lerp(goodWindowRange.x, goodWindowRange.y, f);

            HideCircle();
        }

        protected override void NextStep()
        {
            _noteActive = false;
            _delayTimer = noteInterval;
            HideCircle();
        }

        private void Update()
        {
            if (IsDone) return;
            
            if (!_noteActive)
            {
                _delayTimer -= Time.deltaTime;
                if (_delayTimer <= 0f) SpawnNote();
                return;
            }
            
            _timer += Time.deltaTime;
            UpdateCircle();

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Judge();
                return;
            }

            if (_timer > _shrinkTime + _goodWindow)
            {
                _noteActive = false;
                popup.Show(targetCircle.anchoredPosition, Judgement.Miss);
                HideCircle();
                ReportStep(false);
            }
        }

        private void SpawnNote()
        {
            Rect r = area.rect;
            float margin = targetSize * 0.5f;

            Vector2 pos = new Vector2(
                Random.Range(-r.width * 0.5f + margin, r.width * 0.5f - margin),
                Random.Range(-r.height * 0.5f + margin, r.height * 0.5f - margin));
            
            targetCircle.anchoredPosition = pos;
            approachCircle.anchoredPosition = pos;
            targetCircle.sizeDelta = new Vector2(targetSize, targetSize);
            
            targetCircle.gameObject.SetActive(true);
            approachCircle.gameObject.SetActive(true);

            _timer = 0f;
            _noteActive = true;
            UpdateCircle();
        }

        private void UpdateCircle()
        {
            float p = Mathf.Clamp01(_timer / _shrinkTime);
            float size = targetSize * Mathf.Lerp(startScale, 1f, p);
            
            approachCircle.sizeDelta = new Vector2(size, size);
        }

        private void Judge()
        {
            _noteActive = false;
            Vector2 notePos = targetCircle.anchoredPosition;
            HideCircle();

            float diff = Mathf.Abs(_timer - _shrinkTime);

            if (diff <= _perfectWindow)
            {
                popup.Show(notePos, Judgement.Perfect);
                ReportStep(true);
            }
            else if (diff <= _goodWindow)
            {
                popup.Show(notePos, Judgement.Good);
                ReportStep(true);
            }
            else
            {
                popup.Show(notePos, Judgement.Miss);
                ReportStep(false);
            }
        }

        private void HideCircle()
        {
            targetCircle.gameObject.SetActive(false);
            approachCircle.gameObject.SetActive(false);
        }
    }
}