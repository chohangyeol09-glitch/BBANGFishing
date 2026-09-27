using TMPro;
using UnityEngine;

public class MoneyTextUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text moneyText;


    private void Start()
    {
        if (MoneyManager.Instance == null)
            return;


        MoneyManager.Instance.OnMoneyChanged +=
            Refresh;


        Refresh(
            MoneyManager.Instance.Money
        );
    }


    private void OnDestroy()
    {
        if (MoneyManager.Instance == null)
            return;


        MoneyManager.Instance.OnMoneyChanged -=
            Refresh;
    }


    private void Refresh(int money)
    {
        if (moneyText == null)
            return;


        moneyText.text =
            money.ToString("N0");
    }
}