using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace NKT.Fishing.MiniGame
{
    public enum Judgement
    {
        Perfect,
        Good,
        Miss
    }
    public class JudgementPopup : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private TextMeshProUGUI Text;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("연출")]
        [SerializeField] private float yOffset = 70f;
        [SerializeField] private float riseDistance = 30f;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float punchScale = 1.3f;
        
        [Header("색")]
        [SerializeField] private Color perfectColor;
        [SerializeField] private Color goodColor;
        [SerializeField] private Color missColor;

        private Coroutine _routine;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
        }
        
        public void Show(Vector2 notePos, Judgement judgement)
        {
            Text.text = judgement switch
            {
                Judgement.Perfect => "PERFECT!!",
                Judgement.Good => "GOOD",
                _ => "MISS",
            };

            Text.color = judgement switch
            {
                Judgement.Perfect => perfectColor,
                Judgement.Good => goodColor,
                _ => missColor,
            };
            
            if(_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(PopRoutine(notePos + Vector2.up * yOffset));
        }

        private IEnumerator PopRoutine(Vector2 startPos)
        {
            float t = 0f;

            while (t < 1f)
            {
                t = Mathf.Min(t + Time.deltaTime / duration, 1f);

                root.anchoredPosition = startPos + Vector2.up * (riseDistance * t);
                canvasGroup.alpha = 1f - t * t;

                float s = t < 0.25f
                    ? Mathf.Lerp(0.6f, punchScale, t / 0.25f)
                    : Mathf.Lerp(punchScale, 1f, (t - 0.25f) / 0.75f);
                
                root.localScale = Vector3.one * s;
                
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
            _routine = null;
        }
    }
}