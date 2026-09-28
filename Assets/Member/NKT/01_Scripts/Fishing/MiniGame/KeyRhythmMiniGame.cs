using CHG._02.Script.CoreSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NKT.Fishing.MiniGame
{
    public class KeyRhythmMiniGame : MultiStepMiniGame
    {
        [SerializeField] private JudgementPopup popup;

        [Header("UI")]
        [SerializeField] private RectTransform note;
        [SerializeField] private TextMeshProUGUI noteText;
        [SerializeField] private RectTransform judgeLine;
        [SerializeField] private RectTransform[] laneAnchors;
        [SerializeField] private float spawnY = 320f;

        [Header("난이도")]
        [SerializeField] private Vector2 travelTimeRange = new Vector2(1.3f, 0.6f);
        [SerializeField] private Vector2 perfectWindowRange = new Vector2(0.09f, 0.045f);
        [SerializeField] private Vector2 goodWindowRange = new Vector2(0.2f, 0.1f);
        [SerializeField] private float noteInterval = 0.35f;

        private static readonly Key[] Keys = { Key.Z, Key.X, Key.C, Key.V };
        private Vector2 JudgePos => new Vector2(laneAnchors[_keyIndex].anchoredPosition.x, judgeLine.anchoredPosition.y);

        private float _travelTime;
        private float _perfectWindow;
        private float _goodWindow;

        private float _timer;
        private float _delayTimer;
        private int _keyIndex;
        private bool _noteActive;

        
        protected override void OnGameStart(Grade grade, float f)
        {
            _travelTime = Mathf.Lerp(travelTimeRange.x, travelTimeRange.y, f);
            _perfectWindow = Mathf.Lerp(perfectWindowRange.x, perfectWindowRange.y, f);
            _goodWindow = Mathf.Lerp(goodWindowRange.x, goodWindowRange.y, f);

            note.gameObject.SetActive(false);
        }

        protected override void NextStep()
        {
            _noteActive = false;
            _delayTimer = noteInterval;
            note.gameObject.SetActive(false);
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

            float t = _timer / _travelTime;
            note.anchoredPosition = new Vector2(
                JudgePos.x,
                Mathf.LerpUnclamped(spawnY, judgeLine.anchoredPosition.y, t));
                
            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    if (!kb[Keys[i]].wasPressedThisFrame) continue;
                    Judge(i);
                    return;
                }
            }

            if (_timer > _travelTime + _goodWindow)
            {
                _noteActive = false;
                note.gameObject.SetActive(false);
                popup.Show(judgeLine.anchoredPosition, Judgement.Miss);
                ReportStep(false);
            }
        }

        private void SpawnNote()
        {
            _keyIndex = Random.Range(0, Keys.Length);
            noteText.text = Keys[_keyIndex].ToString();

            _timer = 0f;
            note.anchoredPosition = new Vector2(JudgePos.x, spawnY);
            note.gameObject.SetActive(true);
            _noteActive = true;
        }

        private void Judge(int pressed)
        {
            _noteActive = false;
            note.gameObject.SetActive(false);

            Vector2 pos = JudgePos;

            if (pressed != _keyIndex)
            {
                popup.Show(pos, Judgement.Miss);
                ReportStep(false);
                return;
            }

            float diff = Mathf.Abs(_timer - _travelTime);

            if (diff <= _perfectWindow)
            {
                popup.Show(pos, Judgement.Perfect);
                ReportStep(true);
            }
            else if (diff <= _goodWindow)
            {
                popup.Show(pos, Judgement.Good);
                ReportStep(true);
            }
            else
            {
                popup.Show(pos, Judgement.Miss);
                ReportStep(false);
            }
        }
    }
}