using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] private ShopUI shopUI;

    private bool shopOpened = false;
    public void Interact()
    {
        if (!shopOpened)
        {
            shopUI.OpenShop();
        }
        else
        {
            shopUI.CloseShop();
        }
    }

    
}
