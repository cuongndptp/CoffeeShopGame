using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stock : KitchenObject
{
    [SerializeField] private bool isOpenable;
    private bool isOpened = false;
    [SerializeField] private Transform kitchenObjectPrefab;
    [SerializeField]
    private int stockQuantity;

    public event EventHandler<OnStockOpenEventArgs> OnStockOpen;
    public class OnStockOpenEventArgs: EventArgs
    {
        public bool isOpened;
    }

    public event EventHandler OnKitchenObjectTaken;
    private void Start()
    {
        if(!isOpenable) { isOpened = true; }
    }

    public override void Interact(Player player)
    {
        //If player is not holding anything -> pick it up
        if (!isOpened)
        {
            if(player.GetKitchenObject() == null)
            {
                PlayerPickUp(player);
            }
        }
        else
        {
            if(player.GetKitchenObject() == null)
            {
                //Pick up the food inside
                Transform newIngredientTransform = Instantiate(kitchenObjectPrefab);
                //Call event to remove the food visual
                KitchenObject kitchenObject = newIngredientTransform.GetComponent<KitchenObject>();
                kitchenObject.PlayerPickUp(player);
                OnKitchenObjectTaken?.Invoke(this, EventArgs.Empty);
            }

        }
        
    }

    public void InteractAlternative(Player player)
    {
        if(isOpenable)
        {
            if (!isOpened)
            {
                OnStockOpen?.Invoke(this, new OnStockOpenEventArgs
                {
                    isOpened = true
                });
                isOpened = true;
            }
            else
            {
                OnStockOpen?.Invoke(this, new OnStockOpenEventArgs
                {
                    isOpened = false
                });
                isOpened = false;
            }
        }
        else
        {
            if (player.GetKitchenObject() == null)
            {
                PlayerPickUp(player);
            }
        }
    }
}
