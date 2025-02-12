    using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TotalRatingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    void Start()
    {
        ReputationManager.Instance.OnRatingChanged += Instance_OnRatingChanged;
        textMeshProUGUI.text = GetText(ReputationManager.Instance.GetTotalRating());
    }

    private void Instance_OnRatingChanged(object sender, ReputationManager.OnRatingChangedEventArgs e)
    {
        textMeshProUGUI.text = GetText(e.totalRating);
    }

    private string GetText(float rating)
    {
        return "Rating: " + rating;
    }
}
