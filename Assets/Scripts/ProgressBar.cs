using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private CustomerAI customerAI;

    private void Start()
    {
        gameObject.SetActive(false);
        slider.value = 0f;
        customerAI.OnProgressChanged += CustomerAI_OnProgressChanged;
    }

    private void CustomerAI_OnProgressChanged(object sender, CustomerAI.OnProgressChangedEventArgs e)
    {
        slider.value = e.progress;
        if (e.progress > 0f && !gameObject.activeSelf)
        {
            gameObject.SetActive(true); 
        }
        else if (e.progress <= 0f && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

    }
}
