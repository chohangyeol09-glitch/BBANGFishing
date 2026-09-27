using System;
using CHG._02.Script.CoreSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace NKT.Fishing.MiniGame
{
    public class TrackingMiniGame : ReelMiniGameBase
    {
        [Header("UI")]
        [SerializeField] private RectTransform track;
        [SerializeField] private RectTransform bar;
        [SerializeField] private RectTransform fishIcon;
        [SerializeField] private Image progressFill;

        [Header("조작")]
        [SerializeField] private float barPower = 3.5f;
        [SerializeField] private float gravity = 1.8f;
        
        [Header("난이도")]
        [SerializeField] private Vector2 barSizeRange = new Vector2(0.28f, 0.12f);
        [SerializeField] private Vector2 fishSpeedRange = new Vector2(0.35f, 0.9f);
        [SerializeField] private Vector2 drainRange = new Vector2(0.25f, 0.5f);
        [SerializeField] private float fillSpeed = 0.45f;
        [SerializeField] private float startProgress = 0.35f;

        private float _barPos;
        private float _barVel;
        private float _barHalf;

        private float _fishPos;
        private float _fishTarget;
        private float _fishVel;
        private float _fishSpeed;
        private float _retargetTimer;

        private float _progress;
        private float _drainSpeed;
        private bool _finished;
        
        protected override void StartGame(Grade grade)
        {
            float t = (int)grade / 3f;

            _barHalf = Mathf.Lerp(barSizeRange.x, barSizeRange.y, t) * 0.5f;
            _fishSpeed = Mathf.Lerp(fishSpeedRange.x, fishSpeedRange.y, t);
            _drainSpeed = Mathf.Lerp(drainRange.x, drainRange.y, t);
            
            VariableReset();

            ApplyBarSize();
            Redraw();
        }

        protected override void StopGame()
        {
            _finished = true;
        }

        private void Update()
        {
            if (_finished) return;
            
            float dt = Time.deltaTime;

            UpdateBar(dt);
            UpdateFish(dt);
            Redraw();
            UpdateProgress(dt);
        }

        private void VariableReset()
        {
            _barPos = 0.5f;
            _barVel = 0f;
            _fishPos = 0.5f;
            _fishTarget = 0.5f;
            _fishVel = 0f;
            _retargetTimer = 0f;
            _progress = startProgress;
            _finished = false;
        }

        private void UpdateBar(float dt)
        {
            bool pressed = Mouse.current != null && Mouse.current.leftButton.isPressed;

            if (pressed) _barVel += barPower * dt;
            _barVel -= gravity * dt;
            _barPos += _barVel * dt;

            float min = _barHalf;
            float max = 1f - _barHalf;

            if (_barPos < min)
            {
                _barPos = min;
                _barVel = 0f;
            }
            else if (_barPos > max)
            {
                _barPos = max;
                _barVel = 0f;
            }
        }
        
        private void UpdateFish(float dt)
        {
            _retargetTimer -= dt;

            if (_retargetTimer <= 0f)
            {
                _fishTarget = Random.Range(0.05f, 0.95f);
                _retargetTimer = Random.Range(0.4f, 1.4f);
            }
            
            _fishPos = Mathf.SmoothDamp(_fishPos, _fishTarget, ref _fishVel, 0.4f / _fishSpeed);
            _fishPos = Mathf.Clamp01(_fishPos);
        }
                
        private void UpdateProgress(float dt)
        {
            bool inBar = Mathf.Abs(_fishPos - _barPos) <= _barHalf;

            _progress = Mathf.Clamp01(_progress + (inBar ? fillSpeed : -_drainSpeed) * dt);

            if (_progress >= 1f)
            {
                _finished = true;
                Finish(true);
            }
            else if (_progress <= 0f)
            {
                _finished = true;
                Finish(false);
            }
        }

        private void Redraw()
        {
            float h = track.rect.height;
            float half = h * 0.5f;
            
            bar.anchoredPosition = new Vector2(bar.anchoredPosition.x, Mathf.Lerp(-half, half, _barPos));
            fishIcon.anchoredPosition = new Vector2(fishIcon.anchoredPosition.x, Mathf.Lerp(-half, half, _fishPos));
            
            progressFill.fillAmount = _progress;
        }
        
        private void ApplyBarSize()
        {
            float h = track.rect.height;
            bar.sizeDelta = new Vector2(bar.sizeDelta.x, _barHalf * 2f * h);
        }
    }
}