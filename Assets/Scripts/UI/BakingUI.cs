using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BakingUI : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Stove stove;

    private void Start()
    {
        stove.OnProgressChanged += Stove_OnProgressChanged;
    }
    private void Update()
    {
        
    }


    private void Stove_OnProgressChanged(object sender, Stove.OnProgressChangedEventArgs e)
    {
        slider.value = e.progress;
        if(e.progress <= 0)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
