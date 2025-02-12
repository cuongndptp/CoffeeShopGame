using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReputationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    void Start()
    {
        ReputationManager.Instance.OnReputationChanged += Instance_OnReputationChanged;
        textMeshProUGUI.text = GetText(ReputationManager.Instance.GetReputationFloat());
    }

    private void Instance_OnReputationChanged(object sender, ReputationManager.OnReputationChangedEventArgs e)
    {
        textMeshProUGUI.text = GetText(e.reputationStars);
    }


    private string GetText(float reputationStars)
    {
        return "ReputationStars: " + reputationStars;
    }
}
