using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishTooltipUI : MonoBehaviour
{
    [Header("Tooltip Root")]
    [SerializeField]
    private GameObject tooltipRoot;


    [Header("Canvas")]
    [SerializeField]
    private Canvas canvas;


    [Header("Tooltip")]
    [SerializeField]
    private RectTransform tooltipRect;


    [Header("Text")]
    [SerializeField]
    private TMP_Text fishNameText;

    [SerializeField]
    private TMP_Text weightText;


    [Header("마우스와의 거리")]
    [SerializeField]
    private Vector2 offset =
        new Vector2(20f, -20f);


    private void Awake()
    {
        Hide();
    }


    private void Update()
    {
        if (tooltipRoot == null)
            return;


        if (!tooltipRoot.activeSelf)
            return;


        FollowMouse();
    }


    private void FollowMouse()
    {
        if (Mouse.current == null)
            return;


        if (canvas == null)
            return;


        if (tooltipRect == null)
            return;


        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        Camera uiCamera = null;


        if (canvas.renderMode !=
            RenderMode.ScreenSpaceOverlay)
        {
            uiCamera =
                canvas.worldCamera;
        }


        RectTransform canvasRect =
            canvas.transform as RectTransform;


        if (RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                canvasRect,
                mousePosition,
                uiCamera,
                out Vector2 localPoint))
        {
            tooltipRect.anchoredPosition =
                localPoint + offset;
        }
    }


    public void Show(
        FishInventoryItem item)
    {
        if (item == null ||
            item.FishData == null)
            return;


        if (tooltipRoot != null)
        {
            tooltipRoot.SetActive(true);
        }


        if (fishNameText != null)
        {
            fishNameText.text =
                item.FishName;
        }


        if (weightText != null)
        {
            weightText.text =
                $"{item.Weight:0.0}kg";
        }
    }


    public void Hide()
    {
        if (tooltipRoot != null)
        {
            tooltipRoot.SetActive(false);
        }
    }
}