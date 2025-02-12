using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoffeeValueBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private CoffeePot coffeePot;

    private void Update()
    {
        slider.value = coffeePot.GetCurrentCoffeeValue() / coffeePot.GetMaxCapacity();
    }
}
