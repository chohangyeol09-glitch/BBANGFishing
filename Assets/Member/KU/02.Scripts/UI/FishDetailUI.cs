using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishDetailUI : MonoBehaviour
{
    [Header("전체 UI")]
    [SerializeField]
    private GameObject detailRoot;


    [Header("물고기 이미지")]
    [SerializeField]
    private Image fishImage;


    [Header("텍스트")]
    [SerializeField]
    private TMP_Text fishNameText;

    [SerializeField]
    private TMP_Text weightText;

    [SerializeField]
    private TMP_Text historyText;


    private void Awake()
    {
        if (detailRoot != null)
        {
            detailRoot.SetActive(false);
        }
    }


    public void Show(
        FishInventoryItem item)
    {
        if (item == null ||
            item.Fish == null)
            return;


        FishSO fish = item.Fish;


        if (detailRoot != null)
        {
            detailRoot.SetActive(true);
        }


        if (fishImage != null)
        {
            fishImage.sprite =
                fish.FishSprite;
        }


        if (fishNameText != null)
        {
            fishNameText.text =
                fish.FishName;
        }


        if (weightText != null)
        {
            weightText.text =
                $"{item.Weight:0.0}kg";
        }


        if (historyText != null)
        {
            historyText.text =
                fish.History;
        }
    }


    public void Hide()
    {
        if (detailRoot != null)
        {
            detailRoot.SetActive(false);
        }
    }
}