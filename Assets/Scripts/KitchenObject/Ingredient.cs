using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : KitchenObject
{

    private Holder currentHolder;

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // Player is holding a Holder
            if (player.GetKitchenObject() is Holder holderOnHand)
            {
                //Add ingredients to holder on hand
                if (holderOnHand.TryAddIngredient(this))
                {
                    currentHolder = holderOnHand;
                }
                else
                {
                    Debug.Log("No available slot in the holder.");
                }
            }
        }
        else
        {
            // Player picks up this Ingredient
            if (currentHolder != null)
            {
                currentHolder.RemoveIngredient(this);
                currentHolder = null;
            }
            player.SetKitchenObject(this);
            HoldTo(player.GetInteractPoint());
        }
    }

    public void SetHolder(Holder holder)
    {
        currentHolder = holder;
    }

    public void RemoveFromHolder()
    {
        if (currentHolder != null)
        {
            currentHolder.RemoveIngredient(this);
            currentHolder = null;
        }
    }

    public override void Release(Player player)
    {
        base.Release(player);
        RemoveFromHolder();
    }

    public Holder GetHolder()
    {
        return currentHolder;
    }

    
}
