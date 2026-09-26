using System;
using DG.Tweening;
using UnityEngine;

public class ArrowButtonGroup : MonoBehaviour
{
    [SerializeField] private ArrowButton[] arrowButtons;
    [SerializeField] private RectTransform arrow;
    private int _prevSelectedArrowButtonIndex = 0;
    
    private void OnEnable()
    {
        int _arrowButtonCount = arrowButtons.Length;
        for (int i = 0; i < _arrowButtonCount; i++)
        {
            arrowButtons[i].OnMouseEnterEvent += HandleOnMouseEnterEvent;
        }
    }

    private void OnDisable()
    {
        int _arrowButtonCount = arrowButtons.Length;
        for (int i = 0; i < _arrowButtonCount; i++)
        {
            arrowButtons[i].OnMouseEnterEvent -= HandleOnMouseEnterEvent;
        }    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();

        Vector3 position = arrow.localPosition;
        position.y = arrowButtons[_prevSelectedArrowButtonIndex]
            .transform.localPosition.y;

        arrow.localPosition = position;
    }

    private void HandleOnMouseEnterEvent(ArrowButton obj)
    {
        int _currentSelectedArrowButtonIndex = System.Array.IndexOf(arrowButtons, obj);
        /*
        if (_prevSelectedArrowButtonIndex == _currentSelectedArrowButtonIndex) return; // 전에 이미 선택된 버튼이면 건너뛰기
        */

        int _arrowRotateWay = (_prevSelectedArrowButtonIndex - _currentSelectedArrowButtonIndex) > 0 ? 1 : -1;
        float _currentButtonYPos = obj.transform.localPosition.y;
        ArrowButton _currentButton =  arrowButtons[_currentSelectedArrowButtonIndex];
        
        arrow.DOKill();
        _currentButton.transform.DOKill();
        _currentButton.transform.localScale = Vector3.one;
        
        arrow.DOLocalMoveY(_currentButtonYPos, 0.2f);
        arrow.DOLocalRotate(new Vector3(0, 0, 20 * _arrowRotateWay), 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutBack)
            .OnComplete(() => arrow.localRotation = Quaternion.identity);
        _currentButton.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => _currentButton.transform.localScale = Vector3.one);
        
        _prevSelectedArrowButtonIndex = System.Array.IndexOf(arrowButtons, obj);
    }
}
