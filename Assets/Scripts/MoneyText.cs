using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    // Start is called before the first frame update
    private void Start()
    {
        MoneyManager.Instance.OnMoneyChanged += Instance_OnMoneyChanged;
        textMeshProUGUI.text = GetMoneyText(MoneyManager.Instance.GetPlayerMoney());
    }

    private void Instance_OnMoneyChanged(object sender, System.EventArgs e)
    {
        textMeshProUGUI.text = GetMoneyText(MoneyManager.Instance.GetPlayerMoney());
    }

    private string GetMoneyText(float money)
    {
        return "Money: " + money;
    }

}
