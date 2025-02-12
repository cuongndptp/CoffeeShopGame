using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeePot : Equipment
{
    private bool isFilled = false;
    private Collider coffeePotCollider;
    [SerializeField]
    private float basedMaxCapacity = 60f;
    private float capacityModifier = 1f;
    private float currentCoffeeValue;

    public bool IsFilled() => isFilled;


    private void Start()
    {
        coffeePotCollider = GetComponent<Collider>();
        currentCoffeeValue = 0f;
    }

    private void Update()
    {
        if (currentCoffeeValue < GetCapacity())
        {
            isFilled = false;
        }
        else if (currentCoffeeValue >= GetCapacity())
        {
            isFilled = true;
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {

            HoldTo(player.GetInteractPoint());
            player.SetKitchenObject(this);
            coffeePotCollider.enabled = false;
        }
    }

    public override void Release(Player player)
    {
        base.Release(player);
        coffeePotCollider.enabled = true;
    }

    public void SetCurrentCoffeeValue(float currentCoffeeValue)
    {
        this.currentCoffeeValue = currentCoffeeValue;
    }
    public float GetCurrentCoffeeValue()
    {
        return currentCoffeeValue;
    }

    public float GetMaxCapacity()
    {
        return basedMaxCapacity;
    }

    public void SetIsFilled(bool isFilled)
    {
        this.isFilled = isFilled;
    }

    private float GetCapacity()
    {
        return basedMaxCapacity * capacityModifier;
    }
}
