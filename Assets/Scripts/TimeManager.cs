using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public enum TimeOfDay
    {
        Day, Evening
    }

    [SerializeField] private Material daySkybox;
    [SerializeField] private Material eveningSkybox;
    [SerializeField] private Light directionalLight;

    [SerializeField]
    public float dayLength;
    private float dayTimer = 0f;
    private TimeOfDay currentTimeOfDay;
    private int dayCount = 0;
    //Singleton
    public static TimeManager Instance;
    private void Start()
    {
        ChangeTimeOfDay(TimeOfDay.Day);
        dayTimer = dayLength;
        Instance = this;
    }

    private void Update()
    {
        LightSkyboxMovement();
        switch (currentTimeOfDay)
        {
            case TimeOfDay.Day:
                ManageDayTime();
                break;

            case TimeOfDay.Evening: //Nothing really just wait for day pass to run - DayPass()
                break;
        }
    }

    private void ManageDayTime()
    {
        dayTimer -= Time.deltaTime;
        if(dayTimer <= 0f )
        {
            ChangeTimeOfDay(TimeOfDay.Evening);
        }
    }

    public void DayPass()
    {
        dayCount++;
        //Reset day timer
        dayTimer = dayLength;
        ChangeTimeOfDay(TimeOfDay.Day);
        //Only save game when a day passes.
        GameSaveManager.Instance.SaveGame();
    }


    public TimeOfDay GetCurrentTime()
    {
        return currentTimeOfDay;
    }

    private void UpdateLightning()
    {
        if(currentTimeOfDay == TimeOfDay.Day)
        {
            RenderSettings.skybox = daySkybox;
            RenderSettings.ambientIntensity = 0.8f;
            directionalLight.intensity = 0.6f;
        }
        else if(currentTimeOfDay == TimeOfDay.Evening)
        {
            RenderSettings.skybox = eveningSkybox;
            RenderSettings.ambientIntensity = 0.4f;
            directionalLight.intensity = 0.4f;
        }
    }

    private void ChangeTimeOfDay(TimeOfDay timeOfDay)
    {
        currentTimeOfDay = timeOfDay;
        UpdateLightning();
    }

    public int GetDayCount()
    {
        return dayCount;
    }

    public void SetDayCount(int dayCount)
    {
        this.dayCount = dayCount;
    }

    private void LightSkyboxMovement()
    {
        // Calculate the percentage of the day completed
        float dayProgress = (dayLength - dayTimer) / dayLength / 50f;

        // Rotate directional light smoothly
        float rotationAngle = Mathf.Lerp(0f, 180f, dayProgress); // Simulates sun movement
        directionalLight.transform.rotation = Quaternion.Euler(rotationAngle, -30f, 0f);

        // Rotate skybox material for a dynamic effect
        RenderSettings.skybox.SetFloat("_Rotation", Mathf.Lerp(0f, 360f, dayProgress));
    }
}
