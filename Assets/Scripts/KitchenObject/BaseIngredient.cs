using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseIngredient : KitchenObject
{
    public override void Interact(Player player)
    {
        if(player.GetKitchenObject() == null)
        {
            PlayerPickUp(player);
        }
    }
}
