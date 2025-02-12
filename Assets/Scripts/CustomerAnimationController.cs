using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CustomerAI customer;
    [SerializeField] private bool isMale;

    private void Start()
    {
        animator = GetComponent<Animator>();
        customer.OnStateChanged += Customer_OnStateChanged;
    }

    private void OnDestroy()
    {
        if (customer != null)
        {
            customer.OnStateChanged -= Customer_OnStateChanged;
        }
    }

    private void Customer_OnStateChanged(object sender, CustomerAI.OnStateChangedEventArgs e)
    {
        CustomerAI.AIState state = e.state;
        if (isMale) //Male
        {
            switch (state)
            {
                case CustomerAI.AIState.Idle:
                    break;
                case CustomerAI.AIState.Walking:
                    animator.SetBool("IsWalkingM", true);
                    break;
                case CustomerAI.AIState.WaitingToGetOrder:
                    animator.SetBool("IsSittingM", true);
                    break;
                case CustomerAI.AIState.Leaving:
                    animator.SetBool("IsLeavingM", true);
                    break;
            }
        }
        else //Female
        {
            switch (state)
            {
                case CustomerAI.AIState.Idle:
                    break;
                case CustomerAI.AIState.Walking:
                    animator.SetBool("IsWalkingF", true);
                    break;
                case CustomerAI.AIState.WaitingToGetOrder:
                    animator.SetBool("IsSittingF", true);
                    break;
                case CustomerAI.AIState.Leaving:
                    animator.SetBool("IsLeavingF", true);
                    break;
            }
        }
    }
}
