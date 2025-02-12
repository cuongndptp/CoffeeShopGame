using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform shopContentTemplate;
    [SerializeField] private Transform contentTransform;

    private void Start()
    {
        transform.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseShop);
        InitializeShopUI();
        shopContentTemplate.gameObject.SetActive(false);
    }

    public void CloseShop()
    {
        transform.gameObject.SetActive(false); // Hide the Shop UI
        GameInput.Instance.LockCursor(); // Lock and hide the cursor for gameplay
        Time.timeScale = 1f; // Resume the game
        Player.Instance.SetFreezedLook(false);
    }

    public void OpenShop()
    {
        transform.gameObject.SetActive(true);
        GameInput.Instance.UnlockCursor();
        Time.timeScale = 0f;
        Player.Instance.SetFreezedLook(true);
    }

    public void CreateItemButton(KitchenObjectSO kitchenObjectSO)
    {
        Transform shopItemTransform = Instantiate(shopContentTemplate, contentTransform);
        shopItemTransform.Find("NameText").GetComponent<TextMeshProUGUI>().SetText(kitchenObjectSO.Name);
        shopItemTransform.Find("DescText").GetComponent<TextMeshProUGUI>().SetText(kitchenObjectSO.description);
        shopItemTransform.Find("PriceText").GetComponent<TextMeshProUGUI>().SetText(kitchenObjectSO.price.ToString());
        shopItemTransform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            ShopManager.Instance.TryBuyObject(kitchenObjectSO);
        });



        shopItemTransform.gameObject.SetActive(true);
        //shopItemTransform.Find("ObjectSprite").GetComponent<Image>().sprite = objectSprite;
    }

    private void InitializeShopUI()
    {
        List<KitchenObjectSO> availableObjects = ShopManager.Instance.GetAvailableObjects();
        foreach(KitchenObjectSO kitchenObjectSO in availableObjects)
        {
            CreateItemButton(kitchenObjectSO);
        }
    }

    
}
