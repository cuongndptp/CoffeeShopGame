using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static DeliveryZone;

public class Order : MonoBehaviour
{
    private List<DishSO> dishSOList;
    private float serveTime = 0f;
    private int numberOfDishes;

    private float orderServeTimer;
    private float totalDishEatingTime = 0f;
    private bool orderReceived = false;
    //private bool orderFinished = false;
    private DeliveryZone deliveryZone;
    private bool isFoodDelivered = false;

    public event EventHandler OnServeTimerRunOut;


    private CustomerAI customer;

    private void GenerateOrderData()
    {
        List<DishSO> allDishSO = DishManager.Instance.GetAllDishSO();

        if (allDishSO == null || allDishSO.Count == 0)
        {
            Debug.LogWarning("DishManager returned an empty list of dishes!");
            return;
        }

        numberOfDishes = UnityEngine.Random.Range(1, allDishSO.Count + 2);
        //Get a random number of dishes
        //Based on the number of dishes, get a random list of dishSO from the allDishSO list.
        dishSOList = allDishSO.OrderBy(x => UnityEngine.Random.value).Take(numberOfDishes).ToList();
        //serveTime is serveTime = total sum of dishSO.serveTime
        serveTime = dishSOList.Sum(dishSO => dishSO.serveTime);
        foreach (DishSO dishSO in dishSOList)
        {
            totalDishEatingTime += dishSO.eatingTime;
        }
    }

    public void GenerateOrder()
    {
        GenerateOrderData();
        //Start counting
        orderServeTimer = serveTime;
        //Add this order to the OrderManager's list
        OrderManager.Instance.AddOrder(this);
    }

    public int GetNumberOfDishes()
    {
        return numberOfDishes;
    }

    public float GetServeTime()
    {
        return serveTime;
    }

    public float GetRewardedMoney()
    {
        var rewardedMoney = 0f;

        if (dishSOList.Count > 0)
        {

            foreach (DishSO dishSO in dishSOList)
            {
                rewardedMoney += dishSO.price;
            }
        }

        return rewardedMoney;
    }

    private void Update()
    {
        if (orderReceived && !isFoodDelivered)
        {
            if (orderServeTimer > 0f)
            {
                orderServeTimer -= Time.deltaTime;
            }
            else
            {
                OnServeTimerRunOut?.Invoke(this, EventArgs.Empty);
                isFoodDelivered = true;
            }
        }

    }
    public void SetCustomer(CustomerAI customer)
    {
        this.customer = customer;
    }

    public float GetCurrentServeTime()
    {
        return orderServeTimer;
    }

    public float GetTotalServeTime()
    {
        return serveTime;
    }

    public void SetOrderReceived(bool orderReceived)
    {
        this.orderReceived = orderReceived;
    }

    public List<DishSO> GetDishSOList()
    {
        return dishSOList;
    }

    public void SetDeliveryZone(DeliveryZone deliveryZone)
    {
        this.deliveryZone = deliveryZone;
        this.deliveryZone.OnOrderDelivered += DeliveryZone_OnOrderDelivered;
    }

    private void DeliveryZone_OnOrderDelivered(object sender, OnOrderDeliveredEventArgs e)
    {
        isFoodDelivered = true;
    }

    public float GetTotalEatingTime()
    {
        return totalDishEatingTime;
    }

    public float GetServeTimeRate()
    {
        return orderServeTimer / serveTime;
    }
}
