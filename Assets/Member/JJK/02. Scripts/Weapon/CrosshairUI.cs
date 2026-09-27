using UnityEngine;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.Weapon
{
    public class CrosshairUI : MonoBehaviour
    {
        [SerializeField] private AimModule aimModule;
        [SerializeField] private RectTransform previewParent;
        [SerializeField] private float lineLength = 10f;
        [SerializeField] private float lineThickness = 2f;
        [SerializeField] private float lineGap = 6f;
        [SerializeField] private bool dotEnabled = true;
        [SerializeField] private float dotSize = 4f;
        [SerializeField] private Color color = Color.white;
        [SerializeField] [Range(0f, 1f)] private float opacity = 1f;
        [SerializeField] private float fadeSpeed = 12f;

        public float LineLength => lineLength;
        public float LineThickness => lineThickness;
        public float LineGap => lineGap;
        public bool DotEnabled => dotEnabled;
        public float DotSize => dotSize;
        public Color Color => color;
        public float Opacity => opacity;

        private CanvasGroup _canvasGroup;
        private RectTransform _topBar;
        private RectTransform _bottomBar;
        private RectTransform _rightBar;
        private RectTransform _leftBar;
        private RectTransform _dotRect;
        private Image _dotImage;
        private Image[] _barImages;

        private bool _built;

        private void Awake() => EnsureBuilt();

        // 설정창처럼 코드로 만든 미리보기 인스턴스는 previewParent를 지정한 다음 이 메서드를 직접 호출해야 한다.
        // (AddComponent 시점에 Awake가 바로 실행돼버리면 previewParent를 아직 못 넣은 채로 빌드돼버리기 때문.)
        public void SetPreviewParent(RectTransform parent) => previewParent = parent;

        public void EnsureBuilt()
        {
            if (_built) return;
            _built = true;
            BuildCrosshair();
        }

        private void Update()
        {
            float targetAlpha = aimModule != null && aimModule.IsAiming ? 0f : opacity;
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
        }

        private void BuildCrosshair()
        {
            RectTransform parent;
            if (previewParent != null)
            {
                parent = previewParent;
            }
            else
            {
                var canvasGo = new GameObject("CrosshairCanvas", typeof(RectTransform), typeof(Canvas));
                canvasGo.transform.SetParent(transform, false);
                var canvas = canvasGo.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;
                parent = (RectTransform)canvasGo.transform;
            }

            var rootGo = new GameObject("Crosshair", typeof(RectTransform), typeof(CanvasGroup));
            rootGo.transform.SetParent(parent, false);
            var rootRect = (RectTransform)rootGo.transform;
            rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.anchoredPosition = Vector2.zero;
            _canvasGroup = rootGo.GetComponent<CanvasGroup>();

            _topBar = CreateBar(rootRect, "Top");
            _bottomBar = CreateBar(rootRect, "Bottom");
            _rightBar = CreateBar(rootRect, "Right");
            _leftBar = CreateBar(rootRect, "Left");

            _dotRect = CreateBar(rootRect, "Dot");
            _dotImage = _dotRect.GetComponent<Image>();

            _barImages = new[]
            {
                _topBar.GetComponent<Image>(), _bottomBar.GetComponent<Image>(),
                _rightBar.GetComponent<Image>(), _leftBar.GetComponent<Image>(), _dotImage
            };

            ApplyLayout();
        }

        private RectTransform CreateBar(RectTransform parent, string barName)
        {
            var barGo = new GameObject(barName, typeof(RectTransform), typeof(Image));
            barGo.transform.SetParent(parent, false);

            var rect = (RectTransform)barGo.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            barGo.GetComponent<Image>().color = color;
            return rect;
        }

        private void ApplyLayout()
        {
            float offset = lineGap + lineLength / 2f;

            _topBar.sizeDelta = new Vector2(lineThickness, lineLength);
            _topBar.anchoredPosition = new Vector2(0f, offset);

            _bottomBar.sizeDelta = new Vector2(lineThickness, lineLength);
            _bottomBar.anchoredPosition = new Vector2(0f, -offset);

            _rightBar.sizeDelta = new Vector2(lineLength, lineThickness);
            _rightBar.anchoredPosition = new Vector2(offset, 0f);

            _leftBar.sizeDelta = new Vector2(lineLength, lineThickness);
            _leftBar.anchoredPosition = new Vector2(-offset, 0f);

            _dotRect.sizeDelta = Vector2.one * dotSize;
            _dotRect.anchoredPosition = Vector2.zero;
            _dotImage.enabled = dotEnabled;
        }

        public void SetLineLength(float value)
        {
            lineLength = value;
            ApplyLayout();
        }

        public void SetLineThickness(float value)
        {
            lineThickness = value;
            ApplyLayout();
        }

        public void SetLineGap(float value)
        {
            lineGap = value;
            ApplyLayout();
        }

        public void SetDotEnabled(bool value)
        {
            dotEnabled = value;
            ApplyLayout();
        }

        public void SetDotSize(float value)
        {
            dotSize = value;
            ApplyLayout();
        }

        public void SetColor(Color value)
        {
            color = value;
            if (_barImages == null) return;

            foreach (Image image in _barImages)
                image.color = color;
        }

        public void SetOpacity(float value)
        {
            opacity = Mathf.Clamp01(value);
        }
    }
}
