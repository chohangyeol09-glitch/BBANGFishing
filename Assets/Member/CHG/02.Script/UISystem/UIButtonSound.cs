using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CHG._02.Script.UISystem
{
    /// <summary>
    /// 버튼 GameObject에 붙이면 호버/클릭 소리를 낸다.
    /// EventSystem은 같은 오브젝트의 핸들러 전부에 이벤트를 보내므로
    /// ShopHoverButton, ArrowButton 등 기존 스크립트를 고치지 않고 함께 쓸 수 있다.
    /// </summary>
    public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private UISoundType hoverSound = UISoundType.Hover;
        [SerializeField] private UISoundType clickSound = UISoundType.Click;

        private Selectable _selectable;

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable()) UISoundPlayer.Play(hoverSound);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (IsInteractable()) UISoundPlayer.Play(clickSound);
        }

        // 비활성화된 Button 위에서는 소리를 내지 않는다 (Selectable이 없으면 항상 재생)
        private bool IsInteractable() => _selectable == null || _selectable.IsInteractable();
    }
}
