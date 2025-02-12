using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatZone : MonoBehaviour
{
    [SerializeField] private SeatPoint seatPoint;
    public event EventHandler<OnServingCustomerArrivedEventArgs> OnServingCustomerArrived;
    public class OnServingCustomerArrivedEventArgs : EventArgs
    {
        public CustomerAI servingCustomerAI;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a CustomerAI
        CustomerAI customerAI = other.GetComponent<CustomerAI>();
        if (customerAI != null)
        {
            // Check if this is the serving customer for the seat point
            if (customerAI.GetState() == CustomerAI.AIState.Walking && seatPoint.GetServingCustomer() == customerAI)
            {
                OnServingCustomerArrived?.Invoke(this, new OnServingCustomerArrivedEventArgs
                {
                    servingCustomerAI = customerAI
                });
            }
        }
    }


}
