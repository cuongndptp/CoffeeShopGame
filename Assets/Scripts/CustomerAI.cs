using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.FilePathAttribute;

public class CustomerAI : MonoBehaviour
{
    public AIState state = AIState.Idle;
    private SeatZone targetSeatZone;
    private Transform targetSeatZoneTransform = null;
    private DeliveryZone currentDeliveryZone;
    private NavMeshAgent agent;
    SeatPoint targetSeatPoint;
    private Rigidbody rb;
    private float WaitingToGetOrderTimer = 0f;
    [SerializeField]
    private float TimeToGetOrder = 15f;
    private bool hasOrderGet = false;
    private float eatingTimer = 0f;
    private float totalEatingTimer = 0f;
    private Order order;

    private float orderGetTimeRate = 0f;
    private float orderServeTimeRate = 0f;

    [SerializeField]
    private Transform leavingPoint;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs: EventArgs
    {
        public AIState state;
    }

    public event EventHandler<OnOrderReceivedEventArgs> OnOrderReceived;
    public class OnOrderReceivedEventArgs : EventArgs
    {
        public List<DishSO> dishSOs;
    }

    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progress;
    }

    public enum AIState
    {
        Idle,
        Walking,
        WaitingToGetOrder,
        WaitingForOrder,
        Eating,
        ReadyToPay,
        Leaving
    }



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.isStopped = true;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        //leavingPoint = LeavingPoint.Instance.transform;
    }

    void Update()
    {
        switch (state)
        {
            case AIState.Idle:
                FindSeat();
                //If find seat successful, state -> walking
                break;
            case AIState.Walking:

                MoveTo(targetSeatZoneTransform);
                //SeatZone notices when the customer arrived, state -> waitingToGetorder
                break;
            case AIState.WaitingToGetOrder:
                WaitingToGetOrder();
                //If wait to long, state -> Leaving
                //if PlayerGetOrder() in time -> WaitingForOrder
                break;
            case AIState.WaitingForOrder:
                WaitingForOrder(); //Handling the visual while the actual logic is inside Order class
                //If the subscribed event return success = false, state = Leaving
                //Else if success = true, state = Eating
                break;
            case AIState.Eating:
                Eating();
                break;
            case AIState.ReadyToPay:
                WaitingForPaymentProcess();
                break;
            case AIState.Leaving:
                Leaving();
                break;
        }
    }

    private void MoveTo(Transform location)
    {

        agent.SetDestination(location.position);
    }


    private void OnServingCustomerArrived(object sender, SeatZone.OnServingCustomerArrivedEventArgs e)
    {
        if (e.servingCustomerAI == this && state == AIState.Walking)
        {
            if (targetSeatPoint != null)
            {

                Sit();
                ChangeState(AIState.WaitingToGetOrder);
                WaitingToGetOrderTimer = TimeToGetOrder;
            }
        }
    }


    private void WaitingToGetOrder()
    {
        if (hasOrderGet == false)
        {
            WaitingToGetOrderTimer -= Time.deltaTime;
            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
            {
                progress = WaitingToGetOrderTimer / TimeToGetOrder
            });
            //Failed to get order in time
            if (WaitingToGetOrderTimer <= 0)
            {
                //Punishment or something
                //Leave
                OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
                {
                    progress = 0f
                });
                orderGetTimeRate = 0f;
                SubmitRating();
                GetOutOfSeat();
                ChangeState(AIState.Leaving);
            }
        }
    }
    private void FindSeat()
    {
        targetSeatPoint = SeatManager.Instance.GetAvailableSeatPoint();
        if (targetSeatPoint != null)
        {
            targetSeatZoneTransform = targetSeatPoint.GetSeatZoneTransform();
            targetSeatPoint.SetAvailability(false);
            targetSeatPoint.SetServingCustomer(this);

            targetSeatZone = targetSeatZoneTransform.GetComponent<SeatZone>();
            if (targetSeatZone != null)
            {
                targetSeatZone.OnServingCustomerArrived += OnServingCustomerArrived;
            }
            agent.isStopped = false;
            ChangeState(AIState.Walking);
        }
    }

    private void Sit()
    {
        agent.isStopped = true;
        agent.enabled = false;
        transform.SetParent(targetSeatPoint.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        currentDeliveryZone = targetSeatPoint.GetDeliveryZone();
        currentDeliveryZone.OnOrderDelivered += CurrentDeliveryZone_OnOrderDelivered;
    }

    //Deliveried
    private void CurrentDeliveryZone_OnOrderDelivered(object sender, DeliveryZone.OnOrderDeliveredEventArgs e)
    {
        if (e.success)
        {
            
            //Eating
            orderServeTimeRate = order.GetServeTimeRate();
            ChangeState(AIState.Eating);
        }
        else
        {
            orderServeTimeRate = 0f;
            SubmitRating();
            //Leaving
            GetOutOfSeat();
            
            ChangeState(AIState.Leaving);
        }
    }

    public void Interact(Player player)
    {
        if (state == AIState.WaitingToGetOrder)
        {
            PlayerGetOrder();
        }
        else if (state == AIState.ReadyToPay)
        {
            PlayerProcessPayment();
        }
    }

    private void PlayerGetOrder()
    {
        if (hasOrderGet == false)
        {

            hasOrderGet = true;
            order = gameObject.AddComponent<Order>();
            order.GenerateOrder();
            order.SetCustomer(this);
            order.SetOrderReceived(true);
            order.SetDeliveryZone(currentDeliveryZone);
            //Subcribe to ServeTimerRunOut event
            order.OnServeTimerRunOut += Order_OnServeTimerRunOut;

            OnOrderReceived?.Invoke(this, new OnOrderReceivedEventArgs
            {
                dishSOs = order.GetDishSOList(),
            });

            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
            {
                progress = 0f
            });

            //Calculate the get order rating
            orderGetTimeRate = WaitingToGetOrderTimer / TimeToGetOrder;
            PreparetoEat();
            ChangeState(AIState.WaitingForOrder);

            
        }
    }

    private void Order_OnServeTimerRunOut(object sender, EventArgs e)
    {
        GetOutOfSeat();
        ChangeState(AIState.Leaving);
    }


    private void WaitingForOrder()
    {

        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs { progress = order.GetCurrentServeTime() / order.GetServeTime() });

    }

    private void PreparetoEat()
    {
        //CleanDishesUIImages();
        totalEatingTimer = order.GetTotalEatingTime();

        eatingTimer = totalEatingTimer;
    }

    private void Eating()
    {


        eatingTimer -= Time.deltaTime;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progress = eatingTimer / totalEatingTimer
        });
        if (eatingTimer <= 0f)
        {
            ChangeState(AIState.ReadyToPay);
        }
    }

    private void WaitingForPaymentProcess()
    {

    }

    private void PlayerProcessPayment()
    {
        MoneyManager.Instance.IncreasePlayerBalance(order.GetRewardedMoney());
        SubmitRating();
        GetOutOfSeat();
        ChangeState(AIState.Leaving);
    }

    public float GetRating()
    {
        var rating = (0.3f * orderGetTimeRate + 0.7f * orderServeTimeRate - 0.5f) * 10000f;
        return rating;
    }

    public Order GetCurrentOrder()
    {
        return order;
    }

    public void SetCurrentDeliveryZone(DeliveryZone deliveryZone)
    {
        currentDeliveryZone = deliveryZone;
    }

    private void Leaving()
    {
        agent.SetDestination(leavingPoint.position);
        //If distance to leaving < 0.1f, destroy this game object
        if (Vector3.Distance(transform.position, leavingPoint.position) < 0.5f)
        {
            Destroy(gameObject);
        }
    }

    private void GetOutOfSeat()
    {
        


        transform.SetParent(targetSeatZone.transform);


        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        transform.SetParent(null);
        targetSeatPoint.SetAvailability(true);
        //targetSeatPoint.SetServingCustomer(null);
        //currentDeliveryZone.OnOrderDelivered -= CurrentDeliveryZone_OnOrderDelivered;
        //currentDeliveryZone = null;



        //targetSeatZone.OnServingCustomerArrived -= OnServingCustomerArrived;
        //targetSeatZone = null;
        agent.enabled = true;
        agent.isStopped = false;
        //CleanDishesUIImages();

        if(currentDeliveryZone != null)
        {
            currentDeliveryZone.ClearAllList();
        }
    }

    public AIState GetState()
    {
        return state;
    }


    private void CleanDishesUIImages()
    {
        //Hide the UI for the dishes display images
        OnOrderReceived?.Invoke(this, new OnOrderReceivedEventArgs
        {
            dishSOs = null
        });
    }

    public void Leave()
    {
        if ((state != AIState.Idle && state != AIState.Walking && state != AIState.Leaving) && targetSeatZone != null)
        {
            ResetUI();
            GetOutOfSeat();
        }
        ChangeState(AIState.Leaving);
    }

    public void SetLeavingPoint(Transform leavingPoint)
    {
        this.leavingPoint = leavingPoint;
    }

    private void ResetUI()
    {
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progress = 0f
        });
    }

    private void ChangeState(AIState state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
    }

    private void SubmitRating()
    {
        ReputationManager.Instance.AddRating(GetRating());
        Debug.Log("Added: " + GetRating() + " rating point to TotalRating");
    }
}
