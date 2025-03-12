using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Image clockImage;
    [SerializeField] private TextMeshProUGUI timeOfDayText;

    private void Start()
    {
        TimeManager.Instance.OnTimerCounting += Instance_OnTimerCounting;
        TimeManager.Instance.OnTimeOfDayChanged += Instance_OnTimeOfDayChanged;
    }

    private void Instance_OnTimeOfDayChanged(object sender, TimeManager.OnTimeOfDayChangedEventArgs e)
    {
        timeOfDayText.text = e.timeOfDay.ToString();
    }

    private void Instance_OnTimerCounting(object sender, TimeManager.OnTimerCountingEventArgs e)
    {
        clockImage.fillAmount = e.dayTimeNormalized;
    }
}
