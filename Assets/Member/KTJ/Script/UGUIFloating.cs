using UnityEngine;

/// <summary>
/// UGUI 오브젝트를 기준 위치 위아래로 부드럽게 반복 이동합니다.
/// LayoutGroup이 위치를 제어한다면 그 아래의 시각 요소 자식에 붙이세요.
/// 같은 RectTransform의 위치를 제어하는 Animator/Tween과 함께 사용하지 마세요.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Effects/UGUI Floating")]
public sealed class UGUIFloating : MonoBehaviour
{
    [SerializeField, Min(0f)]
    [Tooltip("기준 위치에서 위아래로 이동하는 최대 거리 (UI 단위)")]
    private float amplitude = 10f;

    [SerializeField, Min(0.01f)]
    [Tooltip("위아래 움직임 한 주기에 걸리는 시간 (초)")]
    private float cycleDuration = 2f;

    [SerializeField]
    [Tooltip("Time.timeScale이 0이어도 애니메이션을 재생합니다.")]
    private bool useUnscaledTime = true;

    private RectTransform _rectTransform;
    private Vector2 _basePosition;
    private double _phase;
    private bool _hasBasePosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _phase = 0d;
        _hasBasePosition = false;
    }

    private void LateUpdate()
    {
        if (!_hasBasePosition)
        {
            // 최초 레이아웃 계산이 끝난 위치를 기준으로 삼습니다.
            Canvas.ForceUpdateCanvases();
            _basePosition = _rectTransform.anchoredPosition;
            _hasBasePosition = true;
        }

        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        _phase = (_phase + deltaTime / Mathf.Max(0.01f, cycleDuration)) % 1d;
        float offset = Mathf.Sin((float)(_phase * Mathf.PI * 2d)) * amplitude;
        _rectTransform.anchoredPosition = _basePosition + Vector2.up * offset;
    }

    private void OnDisable()
    {
        if (_hasBasePosition && _rectTransform != null)
        {
            _rectTransform.anchoredPosition = _basePosition;
        }

        _hasBasePosition = false;
    }
}
