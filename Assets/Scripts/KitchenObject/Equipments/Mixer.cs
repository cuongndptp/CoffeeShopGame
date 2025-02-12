using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mixer : Equipment
{

    private List<KitchenObjectSO> baseIngredients;

    private void Start()
    {
        baseIngredients = new List<KitchenObjectSO>();
        
    }

    public override void Interact(Player player)
    {
        if(player.GetKitchenObject() != null && player.GetKitchenObject() is BaseIngredient)
        {
            KitchenObject kitchenObject = player.GetKitchenObject();
            var kitchenObjectSO = kitchenObject.GetKitchenObjectSO();
            if (kitchenObjectSO.type == KitchenObjectSO.Type.BaseIngredient)
            {
                baseIngredients.Add(kitchenObjectSO);
                player.GetKitchenObject().DestroySelf();
            }
        }
    }

    public void InteractAlternative(Player player)
    {
        if(!MixingSystem.Instance.IsMixing())
        {
            MixingSystem.Instance.StartMixing(baseIngredients, this);
        }
        else
        {
            MixingSystem.Instance.StopMixing();
        }
    }

    public void ClearIngredients()
    {
        baseIngredients?.Clear();
    }
}
