using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMaker : Appliance
{
    private bool hasPot = false;
    private CoffeePot coffeePot;
    [SerializeField] private Transform AttachedCoffeePotVisual;
    [SerializeField] private Transform HolderPoint;
    private float refillSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        AttachedCoffeePotVisual.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (hasPot && coffeePot != null)
        {
            if (!coffeePot.IsFilled() && coffeePot.GetCurrentCoffeeValue() < coffeePot.GetMaxCapacity())
            {
                coffeePot.SetCurrentCoffeeValue(
                    Mathf.Min(coffeePot.GetCurrentCoffeeValue() + Time.deltaTime * refillSpeed, coffeePot.GetMaxCapacity())
                );
            }
        }
    }

    public override void Interact(Player player)
    {
        if (hasPot == false)
        {
            if (player.GetKitchenObject() is CoffeePot playerCoffeePot){
                AttachedCoffeePotVisual.gameObject.SetActive(true);
                coffeePot = playerCoffeePot;
                coffeePot.gameObject.SetActive(false);
                coffeePot.HoldTo(HolderPoint);
                player.SetKitchenObject(null);
                hasPot = true;
            }

        }
        else if (hasPot == true && !player.HasKitchenObject())
        {
            AttachedCoffeePotVisual.gameObject.SetActive(false);
            player.SetKitchenObject(coffeePot);
            coffeePot.HoldTo(player.GetInteractPoint());
            coffeePot.gameObject.SetActive(true);
            hasPot = false;
        }
    }

    public bool HasPot()
    {
        return hasPot;
    }
    public void SetPot(bool hasPot)
    {
        this.hasPot = hasPot;
    }
}
