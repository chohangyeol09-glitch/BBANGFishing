using UnityEngine;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.UI
{
    // 기본 Toggle은 이미지 하나를 켜짐/꺼짐에 따라 페이드로 보였다 안 보였다 할 뿐, 서로 다른
    // 그림으로 바뀌지는 않는다. 이 스크립트를 체크마크(또는 아이콘) 오브젝트에 붙이면
    // On/Off 상태에 맞는 서로 다른 스프라이트로 바로 교체해준다.
    [RequireComponent(typeof(Image))]
    public class ToggleIconSwap : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        private void Awake()
        {
            if (toggle == null) toggle = GetComponentInParent<Toggle>();
            if (icon == null) icon = GetComponent<Image>();

            toggle.onValueChanged.AddListener(SetIcon);
            SetIcon(toggle.isOn);
        }

        private void SetIcon(bool isOn)
        {
            icon.sprite = isOn ? onSprite : offSprite;
        }
    }
}
