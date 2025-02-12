using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatPoint : MonoBehaviour
{
    [SerializeField] private Transform deliveryZoneTransform;
    [SerializeField] private Transform seatZoneTransform;

    private DeliveryZone deliveryZone;
    
    private bool isAvailable = true;
    private CustomerAI servingCustomer;

    public event EventHandler OnSeatPointAvailable;
    public event EventHandler OnSeatPointUnavailable;
    // Start is called before the first frame update
    void Start()
    {
        deliveryZone = deliveryZoneTransform.GetComponent<DeliveryZone>();
    }

    public Transform GetDeliveryZoneTransform() { return deliveryZoneTransform; }

    public void SetAvailability(bool availability)
    {
        if (isAvailable != availability)
        {
            if(availability == true)
            {
                isAvailable = availability;
                // Trigger the availability change event
                OnSeatPointAvailable?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                isAvailable = availability;
                OnSeatPointUnavailable?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool IsAvailable()
    {
        return isAvailable;
    }

    public Transform GetSeatZoneTransform()
    {
        return seatZoneTransform;
    }

    public DeliveryZone GetDeliveryZone()
    {
        return deliveryZone;
    }

    public void SetServingCustomer(CustomerAI servingCustomer)
    {
        this.servingCustomer = servingCustomer;
        if(servingCustomer != null)
        {
            servingCustomer.OnOrderReceived += ServingCustomer_OnOrderReceived;
        }
    }

    private void ServingCustomer_OnOrderReceived(object sender, EventArgs e)
    {
        deliveryZone.SetDishSOs(servingCustomer.GetCurrentOrder().GetDishSOList());
    }

    public CustomerAI GetServingCustomer()
    {
        return servingCustomer;
    }
}
