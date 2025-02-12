using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepableObject : MonoBehaviour
{
    public void Interact()
    {
        Debug.Log("Sleeping now!");
        TimeManager.Instance.DayPass();
    }
}
