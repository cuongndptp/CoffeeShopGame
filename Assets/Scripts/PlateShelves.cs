using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateShelves : MonoBehaviour
{
    [SerializeField]
    private Transform platePrefab;

    public void Interact(Player player)
    {
        if(player.GetKitchenObject() == null) {
            Transform newPlatePrefab = Instantiate(platePrefab);
            Holder plate = newPlatePrefab.GetComponent<Holder>();
            player.SetKitchenObject(plate);
            plate.HoldTo(player.GetInteractPoint());
        }
    }
}
