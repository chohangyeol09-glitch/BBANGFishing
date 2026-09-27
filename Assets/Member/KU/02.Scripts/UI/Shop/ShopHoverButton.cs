using UnityEngine;
using UnityEngine.EventSystems;

public class ShopHoverButton :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    [Header("상점 선택 매니저")]
    [SerializeField]
    private ShopSelectManager selectManager;


    [Header("이 버튼의 페이지")]
    [SerializeField]
    private ShopPageType pageType;


    private RectTransform rectTransform;


    private void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();
    }


    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (selectManager == null)
            return;


        selectManager.SelectButton(
            rectTransform
        );
    }


    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (selectManager == null)
            return;


        selectManager.OpenPage(
            pageType
        );
    }
}