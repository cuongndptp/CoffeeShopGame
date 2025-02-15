using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyShelf : MonoBehaviour
{
    [SerializeField]
    private Transform spawnObjectPrefab;

    public void Interact(Player player)
    {
        if(player.GetKitchenObject() == null) {
            Transform newPrefab = Instantiate(spawnObjectPrefab);
            KitchenObject kitchenObject = newPrefab.GetComponent<KitchenObject>();
            kitchenObject.PlayerPickUp(player);
        }
    }
}
