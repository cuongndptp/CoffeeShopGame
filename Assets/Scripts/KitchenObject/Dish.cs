using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : KitchenObject
{
    [SerializeField] private DishSO dishSO;
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
           
        }
        else
        {
            // Player picks up this Dish
            player.SetKitchenObject(this);
            HoldTo(player.GetInteractPoint());
            player.SetMode(Player.Mode.Put);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public DishSO GetDishSO()
    {
        return dishSO;
    }
}
