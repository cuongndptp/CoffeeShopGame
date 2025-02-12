using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : Equipment
{
    [SerializeField]
    private float coffeeCapacity = 10f;
    [SerializeField]
    private Transform cupOfCoffeePrefab;
    public override void Interact(Player player)
    {
        //If player is holding a coffee pot
        if (player.GetKitchenObject() is CoffeePot coffeePot)
        {
            if (coffeePot.GetCurrentCoffeeValue() >= coffeeCapacity)
            {
                // Reduce the coffee in the pot
                coffeePot.SetCurrentCoffeeValue(coffeePot.GetCurrentCoffeeValue() - coffeeCapacity);

                // Instantiate the cup of coffee prefab at this cup's position
                Transform coffeeInstance = Instantiate(cupOfCoffeePrefab, transform.position, transform.rotation);

                // Destroy or disable this cup object
                gameObject.SetActive(false);  // Or you can deactivate: gameObject.SetActive(false);

                
            }
            else
            {
                Debug.Log("Not enough coffee in the pot!");
            }
        }
        else if (!player.HasKitchenObject())
        {
            HoldTo(player.GetInteractPoint());
            player.SetKitchenObject(this);
        }
    }
}
