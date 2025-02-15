using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DishImages : MonoBehaviour
{
    [SerializeField]
    private CustomerAI customer;
    [SerializeField]
    private Transform dishImageTemplate;

    private DeliveryZone deliveryZone;
    private List<Transform> currentActiveItem;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        dishImageTemplate.gameObject.SetActive(false);
        customer.OnDishListChanged += Customer_OnDishListChanged;
        customer.OnStartLeaving += Customer_OnStartLeaving;
        currentActiveItem = new List<Transform>();
    }

    

    private void Customer_OnDishListChanged(object sender, CustomerAI.OnDishListChangedEventArgs e)
    {
        if(e.dishSOs != null)
        {
            //For each dishSO in dishSOs
            //Instantiate a dishImageTemplate as child of this instance
            //dishImageTemplate.GetComponent<TextMeshProUGUI>().text = dishSO.dishName;
            //That set tempate setActive to true
            DisplayDishImages(e.dishSOs);
        }else
        {
            HideDishImages();
        }
    }
    private void DisplayDishImages(List<DishSO> dishSOs)
    {
        gameObject.SetActive(true);
        UpdateDishImagesList(dishSOs);
    }

    private void HideDishImages()
    {
        gameObject.SetActive(false);
    }

    private void UpdateDishImagesList(List<DishSO> dishSOs)
    {
        foreach(var item in currentActiveItem)
        {
            Destroy(item.gameObject);
        }
        currentActiveItem.Clear();

        foreach (var dishSO in dishSOs)
        {
            Transform dishImageInstance = Instantiate(dishImageTemplate, this.transform);
            currentActiveItem.Add(dishImageInstance);
            dishImageInstance.gameObject.SetActive(true);
            RawImage rawImage = dishImageInstance.GetComponentInChildren<RawImage>();
            if(rawImage != null)
            {
                rawImage.texture = dishSO.icon;
            }
            
        }
    }

    public void SetDeliveryZone(DeliveryZone deliveryZone)
    {
        this.deliveryZone = deliveryZone;
        deliveryZone.OnOrderAdded += DeliveryZone_OnOrderAdded;
    }
    private void Customer_OnStartLeaving(object sender, System.EventArgs e)
    {
        if(deliveryZone != null)
        {
            deliveryZone.OnOrderAdded -= DeliveryZone_OnOrderAdded;
        }
    }
    private void DeliveryZone_OnOrderAdded(object sender, DeliveryZone.OnOrderAddedEventArgs e)
    {
        if (e.dishSOs != null)
        {
            //For each dishSO in dishSOs
            //Instantiate a dishImageTemplate as child of this instance
            //dishImageTemplate.GetComponent<TextMeshProUGUI>().text = dishSO.dishName;
            //That set tempate setActive to true
            DisplayDishImages(e.dishSOs);
        }
        else
        {
            HideDishImages();
        }
    }

    
}
