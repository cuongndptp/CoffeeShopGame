using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    //Singleton
    public static ShopManager Instance;

    
    [SerializeField] private List<KitchenObjectSO> availableObjects;
    [SerializeField] private Transform storeTransform;

    private void Awake()
    {
        Instance = this;
    }

    public List<KitchenObjectSO> GetAvailableObjects()
    {
        return availableObjects;
    }

    public void TryBuyObject(KitchenObjectSO kitchenObjectSO)
    {
        if (MoneyManager.Instance.GetPlayerMoney() >= kitchenObjectSO.price)
        {
            MoneyManager.Instance.SpendMoney(kitchenObjectSO.price);
            Player.Instance.TryReleaseObject();
            Transform newKitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            KitchenObject newKitchenObject = newKitchenObjectTransform.GetComponent<KitchenObject>();
            newKitchenObject.PlayerPickUp(Player.Instance);
        }
    }

    public void SetStoreActive(bool isActivated)
    {
        storeTransform.gameObject.SetActive(isActivated);
    }
}
