using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockVisual : MonoBehaviour
{
    [SerializeField] private Stock stock;
    [SerializeField] private Transform capVisual;
    [SerializeField] private List<Transform> foodVisuals;

    private void Start()
    {
        stock.OnStockOpen += Stock_OnStockOpened;
        stock.OnKitchenObjectTaken += Stock_OnIngredientTaken;
        capVisual.gameObject.SetActive(true);
    }

    private void Stock_OnStockOpened(object sender, Stock.OnStockOpenEventArgs e)
    {
        if(e.isOpened)
        {
            capVisual.gameObject.SetActive(false);
        }
        else
        {
            capVisual.gameObject.SetActive(true);
        }
    }

    private void Stock_OnIngredientTaken(object sender, System.EventArgs e)
    {
        if (foodVisuals != null && foodVisuals.Count > 0)
        {
            // Get the last index of the list
            int lastIndex = foodVisuals.Count - 1;

            // Deactivate the last food visual
            foodVisuals[lastIndex].gameObject.SetActive(false);

            // Remove it from the list
            foodVisuals.RemoveAt(lastIndex);
        }
        else
        {
            Debug.LogWarning("No food visuals left to remove!");
        }
    }

    
}
