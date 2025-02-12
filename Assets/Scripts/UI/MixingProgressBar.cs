using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        MixingSystem.Instance.OnProgressChanged += Instance_OnProgressChanged;
    }

    private void Instance_OnProgressChanged(object sender, MixingSystem.OnProgressChangedEventArgs e)
    {
        slider.value = e.progress;
    }

}
