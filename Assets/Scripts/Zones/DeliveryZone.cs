using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private List<DishSO> orderDishSOs;
    private List<DishSO> remainingDishSOs;
    private List<DishSO> deliveredDishSOList;

    public event EventHandler<OnOrderDeliveredEventArgs> OnOrderDelivered;
    public event EventHandler OnOrderAdded;
    public class OnOrderDeliveredEventArgs : EventArgs
    {
        public bool success;
    }

    // Start is called before the first frame update
    void Start()
    {
        orderDishSOs = new List<DishSO>();
        remainingDishSOs = new List<DishSO>();
        deliveredDishSOList = new List<DishSO>();
    }

    public void SetDishSOs(List<DishSO> dishSOList)
    {
        orderDishSOs = dishSOList;
        remainingDishSOs = new List<DishSO>(orderDishSOs);
    }



    //This is called when player put the dish on delveryzone
    public void TryAddOrder(Dish deliveredDish)
    {
        if (orderDishSOs == null || remainingDishSOs == null || deliveredDishSOList == null)
        {
            Debug.LogError("DeliveryZone is not properly initialized. Ensure SetDishSOs is called.");
            return;
        }

        DishSO deliveredDishSO = deliveredDish.GetDishSO();
        if (remainingDishSOs.Count > 0)
        {
            //Check if remaining dishSOs contains this dishSO.
            if (remainingDishSOs.Contains(deliveredDishSO))
            {
                deliveredDishSOList.Add(deliveredDishSO);
                remainingDishSOs.Remove(deliveredDishSO);
                OnOrderAdded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnOrderDelivered?.Invoke(this, new OnOrderDeliveredEventArgs
                {
                    success = false
                });
            }
        }

        if (remainingDishSOs.Count == 0)
        {
            ProcessOrder();
        }


    }

    private void ProcessOrder()
    {

        // Check if all dishes in the order are delivered
        bool success = orderDishSOs.Count == deliveredDishSOList.Count &&
                       orderDishSOs.All(dish => deliveredDishSOList.Contains(dish));

        if (success)
        {
            // Trigger the event
            OnOrderDelivered?.Invoke(this, new OnOrderDeliveredEventArgs { success = success });

        }
        else
        {
            Debug.Log("Order Failed");
            OnOrderDelivered?.Invoke(this, new OnOrderDeliveredEventArgs { success = false });
        }

        // Clear lists for the next order
        ClearAllList();
    }

    public void ClearAllList()
    {
        orderDishSOs.Clear();
        remainingDishSOs.Clear();
        deliveredDishSOList.Clear();
    }
}
