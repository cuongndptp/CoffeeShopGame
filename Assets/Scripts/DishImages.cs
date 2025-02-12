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
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        dishImageTemplate.gameObject.SetActive(false);
        customer.OnOrderReceived += Customer_OnOrderReceived;
    }

    private void Customer_OnOrderReceived(object sender, CustomerAI.OnOrderReceivedEventArgs e)
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
        foreach (var dishSO in dishSOs)
        {
            Transform dishImageInstance = Instantiate(dishImageTemplate, this.transform);
            dishImageInstance.gameObject.SetActive(true);
            RawImage rawImage = dishImageInstance.GetComponentInChildren<RawImage>();
            if(rawImage != null)
            {
                rawImage.texture = dishSO.icon;
            }
            
        }
    }

}
