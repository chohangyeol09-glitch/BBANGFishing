using UnityEngine;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.Weapon
{
    public class CrosshairUI : MonoBehaviour
    {
        [SerializeField] private AimModule aimModule;
        [SerializeField] private float lineLength = 10f;
        [SerializeField] private float lineThickness = 2f;
        [SerializeField] private float lineGap = 6f;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private float fadeSpeed = 12f;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            BuildCrosshair();
        }

        private void Update()
        {
            float targetAlpha = aimModule != null && aimModule.IsAiming ? 0f : 1f;
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
        }

        private void BuildCrosshair()
        {
            var canvasGo = new GameObject("CrosshairCanvas", typeof(RectTransform), typeof(Canvas));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var rootGo = new GameObject("Crosshair", typeof(RectTransform), typeof(CanvasGroup));
            rootGo.transform.SetParent(canvasGo.transform, false);
            var rootRect = (RectTransform)rootGo.transform;
            rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.anchoredPosition = Vector2.zero;
            _canvasGroup = rootGo.GetComponent<CanvasGroup>();

            float offset = lineGap + lineLength / 2f;
            CreateBar(rootRect, new Vector2(0f, offset), new Vector2(lineThickness, lineLength));
            CreateBar(rootRect, new Vector2(0f, -offset), new Vector2(lineThickness, lineLength));
            CreateBar(rootRect, new Vector2(offset, 0f), new Vector2(lineLength, lineThickness));
            CreateBar(rootRect, new Vector2(-offset, 0f), new Vector2(lineLength, lineThickness));
        }

        private void CreateBar(RectTransform parent, Vector2 anchoredPosition, Vector2 size)
        {
            var barGo = new GameObject("Bar", typeof(RectTransform), typeof(Image));
            barGo.transform.SetParent(parent, false);

            var rect = (RectTransform)barGo.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            barGo.GetComponent<Image>().color = color;
        }
    }
}
