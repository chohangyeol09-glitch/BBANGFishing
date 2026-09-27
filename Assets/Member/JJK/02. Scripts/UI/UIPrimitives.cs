using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.UI
{
    // 설정창처럼 코드로 UI를 직접 만들 때 쓰는 둥근 모서리 패널/버튼/슬라이더 생성 도우미.
    public static class UIPrimitives
    {
        private static readonly Dictionary<int, Sprite> RoundedSpriteCache = new();

        public static Sprite RoundedSprite(int radius = 16)
        {
            if (RoundedSpriteCache.TryGetValue(radius, out Sprite cached)) return cached;

            int size = radius * 2 + 4;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pixel = new Vector2(x + 0.5f, y + 0.5f);
                    Vector2? cornerCenter = null;

                    if (x < radius && y < radius) cornerCenter = new Vector2(radius, radius);
                    else if (x >= size - radius && y < radius) cornerCenter = new Vector2(size - radius, radius);
                    else if (x < radius && y >= size - radius) cornerCenter = new Vector2(radius, size - radius);
                    else if (x >= size - radius && y >= size - radius) cornerCenter = new Vector2(size - radius, size - radius);

                    float alpha = cornerCenter.HasValue
                        ? Mathf.Clamp01(radius - Vector2.Distance(pixel, cornerCenter.Value) + 0.5f)
                        : 1f;

                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            texture.Apply();

            var sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
            RoundedSpriteCache[radius] = sprite;
            return sprite;
        }

        public static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        public static RectTransform Panel(Transform parent, string name, Color color, int radius = 16)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.sprite = RoundedSprite(radius);
            image.type = Image.Type.Sliced;
            image.color = color;
            image.raycastTarget = false;
            return (RectTransform)go.transform;
        }

        public static TMP_Text Text(Transform parent, string name, string content, float fontSize, Color color,
            TextAlignmentOptions alignment = TextAlignmentOptions.MidlineLeft, FontStyles style = FontStyles.Normal)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            if (TMP_Settings.defaultFontAsset != null) text.font = TMP_Settings.defaultFontAsset;
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.fontStyle = style;
            text.raycastTarget = false;
            return text;
        }

        public static Button Button(Transform parent, string name, Color bgColor, int radius = 10)
        {
            RectTransform rect = Panel(parent, name, bgColor, radius);
            var image = rect.GetComponent<Image>();
            image.raycastTarget = true;

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.9f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.selectedColor = Color.white;
            button.colors = colors;

            return button;
        }

        public static Slider CreateSlider(Transform parent, string name, float min, float max, float value,
            Color trackColor, Color fillColor)
        {
            RectTransform root = Panel(parent, name, trackColor, 8);
            GetComponentImage(root).raycastTarget = true;

            var slider = root.gameObject.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.direction = Slider.Direction.LeftToRight;
            slider.transition = Selectable.Transition.None;

            var fillAreaGo = new GameObject("Fill Area", typeof(RectTransform));
            fillAreaGo.transform.SetParent(root, false);
            var fillAreaRect = (RectTransform)fillAreaGo.transform;
            Stretch(fillAreaRect, 3f);

            RectTransform fillRect = Panel(fillAreaRect, "Fill", fillColor, 6);
            Stretch(fillRect);

            var handleAreaGo = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleAreaGo.transform.SetParent(root, false);
            var handleAreaRect = (RectTransform)handleAreaGo.transform;
            Stretch(handleAreaRect, 8f);

            RectTransform handleRect = Panel(handleAreaRect, "Handle", Color.white, 9);
            handleRect.sizeDelta = new Vector2(18f, 18f);
            handleRect.anchorMin = handleRect.anchorMax = new Vector2(0.5f, 0.5f);
            GetComponentImage(handleRect).raycastTarget = true;

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = GetComponentImage(handleRect);

            slider.SetValueWithoutNotify(value);
            return slider;
        }

        // 클릭할 때마다 on/off가 뒤집히는 Toggle이며, 두 반쪽의 색이 현재 값에 맞춰 갈아입는다.
        // (좌우 반쪽을 각각 직접 클릭해 선택하는 방식은 아니고, 전체를 눌러 상태를 전환하는 방식이다.)
        public static Toggle TogglePill(Transform parent, string name, string onText, string offText, bool isOn,
            Color activeColor, Color inactiveColor)
        {
            RectTransform root = Panel(parent, name, inactiveColor, 8);
            GetComponentImage(root).raycastTarget = true;
            var toggle = root.gameObject.AddComponent<Toggle>();
            toggle.transition = Selectable.Transition.None;

            RectTransform onRect = Panel(root, "On", isOn ? activeColor : inactiveColor, 8);
            onRect.anchorMin = Vector2.zero;
            onRect.anchorMax = new Vector2(0.5f, 1f);
            onRect.offsetMin = onRect.offsetMax = Vector2.zero;
            TMP_Text onLabel = Text(onRect, "Label", onText, 16f, isOn ? Color.black : Color.white, TextAlignmentOptions.Midline);
            Stretch((RectTransform)onLabel.transform);

            RectTransform offRect = Panel(root, "Off", !isOn ? activeColor : inactiveColor, 8);
            offRect.anchorMin = new Vector2(0.5f, 0f);
            offRect.anchorMax = Vector2.one;
            offRect.offsetMin = offRect.offsetMax = Vector2.zero;
            TMP_Text offLabel = Text(offRect, "Label", offText, 16f, !isOn ? Color.black : Color.white, TextAlignmentOptions.Midline);
            Stretch((RectTransform)offLabel.transform);

            Image onImage = GetComponentImage(onRect);
            Image offImage = GetComponentImage(offRect);

            toggle.targetGraphic = onImage;
            toggle.isOn = isOn;
            toggle.onValueChanged.AddListener(value =>
            {
                onImage.color = value ? activeColor : inactiveColor;
                offImage.color = !value ? activeColor : inactiveColor;
                onLabel.color = value ? Color.black : Color.white;
                offLabel.color = !value ? Color.black : Color.white;
            });

            return toggle;
        }

        private static Image GetComponentImage(RectTransform rect) => rect.GetComponent<Image>();

        public static VerticalLayoutGroup VerticalGroup(RectTransform rect, float spacing, RectOffset padding,
            bool controlWidth, bool controlHeight, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            var group = rect.gameObject.AddComponent<VerticalLayoutGroup>();
            group.spacing = spacing;
            group.padding = padding;
            group.childAlignment = alignment;
            group.childControlWidth = controlWidth;
            group.childControlHeight = controlHeight;
            group.childForceExpandWidth = controlWidth;
            group.childForceExpandHeight = false;
            return group;
        }

        public static HorizontalLayoutGroup HorizontalGroup(RectTransform rect, float spacing, RectOffset padding,
            bool controlWidth, bool controlHeight, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var group = rect.gameObject.AddComponent<HorizontalLayoutGroup>();
            group.spacing = spacing;
            group.padding = padding;
            group.childAlignment = alignment;
            group.childControlWidth = controlWidth;
            group.childControlHeight = controlHeight;
            group.childForceExpandWidth = false;
            group.childForceExpandHeight = controlHeight;
            return group;
        }

        public static LayoutElement FixedSize(GameObject go, float? width = null, float? height = null)
        {
            var element = go.AddComponent<LayoutElement>();
            if (width.HasValue)
            {
                element.preferredWidth = width.Value;
                element.minWidth = width.Value;
            }
            if (height.HasValue)
            {
                element.preferredHeight = height.Value;
                element.minHeight = height.Value;
            }
            return element;
        }

        public static LayoutElement FlexibleSize(GameObject go, float flexibleWidth = 1f, float flexibleHeight = 0f)
        {
            var element = go.AddComponent<LayoutElement>();
            element.flexibleWidth = flexibleWidth;
            element.flexibleHeight = flexibleHeight;
            return element;
        }

        // 자식들(Row/Text 등)을 쌓아놓은 만큼만 높이를 차지하게 만든다. Vertical 그룹이 controlHeight=true일 때
        // 그 안의 자식들에 대해 실제 높이를 계산/적용해주므로, 이 카드 자신의 높이는 상위 그룹이 그대로 읽어간다.
        public static void AutoHeight(RectTransform rect)
        {
            var fitter = rect.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
}
