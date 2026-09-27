using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// View: Inspector references, layout and animation.
public sealed class TitleUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private RectTransform buttonGroupRect;
    [SerializeField] private RectTransform titleRect;
    [SerializeField] private RectTransform Rope;
    [SerializeField, Min(0.01f)] private float tweenDuration = 0.7f;
    [SerializeField] private Ease showEase = Ease.OutCubic;
    [SerializeField] private Ease hideEase = Ease.InCubic;
    [SerializeField] private UnityEvent onSettingsRequested = new UnityEvent();
    [SerializeField] private UnityEvent onStartAnimationCompleted = new UnityEvent();

    private TitleUIController _controller;
    private Sequence _sequence;
    private UGUIFloating _floating;
    private bool _floatingWasEnabled;

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (!canvas || !startButton || !settingsButton || !quitButton ||
            !buttonGroupRect || !titleRect)
        {
            Debug.LogError("TitleUI: Canvas, 버튼 3개, 버튼 그룹과 타이틀 Rect를 연결하세요.", this);
            enabled = false;
            return;
        }
        _floating = titleRect.GetComponent<UGUIFloating>();
        _floatingWasEnabled = _floating && _floating.enabled;
        PauseFloating();
        Canvas.ForceUpdateCanvases();
        RectTransform canvasRect = (RectTransform)canvas.rootCanvas.transform;
        if (Rope)
            Rope.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasRect.rect.height);
        Canvas.ForceUpdateCanvases();
        var model = new TitleUIModel(buttonGroupRect.anchoredPosition, titleRect.anchoredPosition,
            GetHiddenPosition(buttonGroupRect, canvasRect, false),
            GetHiddenPosition(titleRect, canvasRect, true));
        _controller = new TitleUIController(this, model);
        startButton.onClick.AddListener(_controller.StartGame);
        settingsButton.onClick.AddListener(_controller.OpenSettings);
        quitButton.onClick.AddListener(_controller.Quit);
        _controller.Show();
    }

    private static Vector2 GetHiddenPosition(RectTransform rect, RectTransform canvas, bool above)
    {
        // Include children and convert the canvas displacement to the parent's coordinates.
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas, rect);
        float distance = above
            ? Mathf.Max(canvas.rect.height, canvas.rect.yMax - bounds.min.y + 20f)
            : Mathf.Min(-canvas.rect.height, canvas.rect.yMin - bounds.max.y - 20f);
        Vector3 delta = rect.parent.InverseTransformVector(canvas.TransformVector(Vector3.up * distance));
        return rect.anchoredPosition + new Vector2(delta.x, delta.y);
    }

    public void SetPositions(Vector2 buttons, Vector2 title)
    {
        buttonGroupRect.anchoredPosition = buttons;
        titleRect.anchoredPosition = title;
    }

    public void Animate(Vector2 buttons, Vector2 title, bool showing, Action completed)
    {
        StopAnimation();
        _sequence = DOTween.Sequence().SetUpdate(true);
        _sequence.Join(buttonGroupRect.DOAnchorPos(buttons, Mathf.Max(0.01f, tweenDuration)).SetEase(Ease.Linear));
        _sequence.Join(titleRect.DOAnchorPos(title, Mathf.Max(0.01f, tweenDuration)).SetEase(Ease.Linear));
        _sequence.SetEase(showing ? showEase : hideEase);
        _sequence.OnComplete(() =>
        {
            _sequence = null;
            if (showing && _floatingWasEnabled && _floating) _floating.enabled = true;
            completed?.Invoke();
        });
    }

    public void SetInteractable(bool value)
    {
        startButton.interactable = value;
        settingsButton.interactable = value;
        quitButton.interactable = value;
    }

    public void StopAnimation()
    {
        _sequence?.Kill();
        _sequence = null;
        PauseFloating();
    }

    private void PauseFloating()
    {
        if (_floatingWasEnabled && _floating) _floating.enabled = false;
    }

    public void NotifySettingsRequested() => onSettingsRequested.Invoke();
    public void NotifyStartCompleted() => onStartAnimationCompleted.Invoke();
    private void OnEnable() => _controller?.Show();
    private void OnDisable() => _controller?.Suspend();

    private void OnDestroy()
    {
        StopAnimation();
        if (_controller == null) return;
        if (startButton) startButton.onClick.RemoveListener(_controller.StartGame);
        if (settingsButton) settingsButton.onClick.RemoveListener(_controller.OpenSettings);
        if (quitButton) quitButton.onClick.RemoveListener(_controller.Quit);
    }
}
