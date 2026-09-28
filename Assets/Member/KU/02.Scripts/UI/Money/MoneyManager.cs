using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }


    [Header("돈")]
    [SerializeField]
    private int startingMoney = 0;


    [Header("테스트")]
    [SerializeField]
    private int testAddMoney = 100;


    private int money;


    public int Money => money;


    public event Action<int> OnMoneyChanged;


    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;


        money = startingMoney;
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;

    }


    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;


        money += amount;


        OnMoneyChanged?.Invoke(
            money
        );


        Debug.Log(
            $"돈 +{amount} / 현재 돈 : {money}"
        );
    }


    public bool CanAfford(int amount)
    {
        if (amount < 0)
            return false;


        return money >= amount;
    }


    public bool TrySpendMoney(int amount)
    {
        if (amount < 0)
            return false;


        if (!CanAfford(amount))
        {
            Debug.Log(
                $"돈 부족 / 필요 : {amount} / 현재 : {money}"
            );

            return false;
        }


        money -= amount;


        OnMoneyChanged?.Invoke(
            money
        );


        Debug.Log(
            $"돈 -{amount} / 현재 돈 : {money}"
        );


        return true;
    }


    public void SetMoney(int amount)
    {
        money =
            Mathf.Max(
                0,
                amount
            );


        OnMoneyChanged?.Invoke(
            money
        );
    }
}