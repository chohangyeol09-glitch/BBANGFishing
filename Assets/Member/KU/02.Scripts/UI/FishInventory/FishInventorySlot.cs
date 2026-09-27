using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishInventorySlot :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("물고기 이미지")]
    [SerializeField]
    private Image fishImage;


    private FishInventoryItem item;

    private FishTooltipUI tooltipUI;
    private FishDetailUI detailUI;


    public bool HasItem => item != null;

    public FishInventoryItem Item => item;


    public void Initialize(
        FishTooltipUI tooltip,
        FishDetailUI detail)
    {
        tooltipUI = tooltip;
        detailUI = detail;

        Clear();
    }


    public void SetItem(
        FishInventoryItem newItem)
    {
        item = newItem;


        if (item == null ||
            item.FishData == null)
        {
            Clear();
            return;
        }


        if (fishImage != null)
        {
            fishImage.sprite =
                item.FishSprite;

            fishImage.enabled = true;
        }
    }


    public void Clear()
    {
        item = null;


        if (fishImage != null)
        {
            fishImage.sprite = null;
            fishImage.enabled = false;
        }
    }


    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (item == null)
            return;


        if (tooltipUI != null)
        {
            tooltipUI.Show(item);
        }
    }


    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (tooltipUI != null)
        {
            tooltipUI.Hide();
        }
    }


    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (item == null)
            return;


        if (detailUI != null)
        {
            detailUI.Show(item);
        }
    }
}