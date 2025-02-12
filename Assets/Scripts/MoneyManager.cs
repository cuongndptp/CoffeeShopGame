using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField]
    private float money;

    public static MoneyManager Instance;
    public event EventHandler OnMoneyChanged;


    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    public float GetPlayerMoney()
    {
        return money;
    }
    public void IncreasePlayerBalance(float addedValue)
    {
        money += addedValue;
        OnMoneyChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SetPlayerMoney(float money)
    {
        this.money = money;
        OnMoneyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool SpendMoney(float amount)
    {
        if (amount > 0 && money >= amount)
        {
            money -= amount;
            OnMoneyChanged?.Invoke(this, EventArgs.Empty); 
            return true; // Transaction successful
        }
        return false; // Transaction failed (insufficient funds)
    }

}
