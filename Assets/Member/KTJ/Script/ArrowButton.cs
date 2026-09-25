using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ArrowButton : MonoBehaviour, IPointerEnterHandler
{
    public event Action<ArrowButton> OnMouseEnterEvent;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnterEvent?.Invoke(this);
    }
}
