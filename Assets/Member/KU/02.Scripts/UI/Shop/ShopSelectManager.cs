using TMPro;
using UnityEngine;

public class ShopSelectManager : MonoBehaviour
{
    [Header("선택 프레임")]
    [SerializeField]
    private RectTransform selectFrame;


    [Header("현재 페이지 텍스트")]
    [SerializeField]
    private TMP_Text pageTitleText;


    [Header("메뉴 페이지")]
    [SerializeField]
    private GameObject menuPage;


    [Header("상점 페이지")]
    [SerializeField]
    private GameObject buyPage;

    [SerializeField]
    private GameObject sellPage;

    [SerializeField]
    private GameObject upgradePage;

    [SerializeField]
    private GameObject skillPage;


    private RectTransform currentButton;

    private bool isMenuOpen = true;


    public bool IsMenuOpen => isMenuOpen;


    private void OnEnable()
    {
        OpenMenu();
    }


    private void OnDisable()
    {
        HideSelectFrame();
    }


    public void SelectButton(RectTransform button)
    {
        if (button == null)
            return;


        if (!isMenuOpen)
            return;


        if (selectFrame == null)
            return;


        currentButton = button;


        if (!selectFrame.gameObject.activeSelf)
        {
            selectFrame.gameObject.SetActive(true);
        }


        selectFrame.position =
            button.position;
    }


    public void OpenPage(ShopPageType pageType)
    {
        CloseAllPages();

        HideSelectFrame();


        isMenuOpen = false;


        switch (pageType)
        {
            case ShopPageType.Buy:

                if (buyPage != null)
                {
                    buyPage.SetActive(true);
                }

                SetPageTitle("구매");

                break;


            case ShopPageType.Sell:

                if (sellPage != null)
                {
                    sellPage.SetActive(true);
                }

                SetPageTitle("판매");

                break;


            case ShopPageType.Upgrade:

                if (upgradePage != null)
                {
                    upgradePage.SetActive(true);
                }

                SetPageTitle("업그레이드");

                break;


            case ShopPageType.Skill:

                if (skillPage != null)
                {
                    skillPage.SetActive(true);
                }

                SetPageTitle("스킬");

                break;
        }
    }


    public void OpenMenu()
    {
        CloseAllPages();


        isMenuOpen = true;


        if (menuPage != null)
        {
            menuPage.SetActive(true);
        }


        HideSelectFrame();


        SetPageTitle("홈");
    }


    public bool TryHandleEscape()
    {
        if (!isMenuOpen)
        {
            OpenMenu();

            return true;
        }


        return false;
    }


    private void CloseAllPages()
    {
        if (menuPage != null)
        {
            menuPage.SetActive(false);
        }


        if (buyPage != null)
        {
            buyPage.SetActive(false);
        }


        if (sellPage != null)
        {
            sellPage.SetActive(false);
        }


        if (upgradePage != null)
        {
            upgradePage.SetActive(false);
        }


        if (skillPage != null)
        {
            skillPage.SetActive(false);
        }
    }


    private void SetPageTitle(string title)
    {
        if (pageTitleText == null)
            return;


        pageTitleText.text = title;
    }


    public void HideSelectFrame()
    {
        currentButton = null;


        if (selectFrame != null)
        {
            selectFrame.gameObject.SetActive(false);
        }
    }
}