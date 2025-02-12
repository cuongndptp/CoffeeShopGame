using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;
    public List<Order> activeOrders;

    private void Awake()
    {
        Instance = this;
        activeOrders = new List<Order>();
    }

    // Add an order to the list
    public void AddOrder(Order order)
    {
        if (!activeOrders.Contains(order))
        {
            activeOrders.Add(order);
        }
        else
        {
            Debug.LogWarning("Attempted to add a duplicate order.");
        }
    }
    
}
