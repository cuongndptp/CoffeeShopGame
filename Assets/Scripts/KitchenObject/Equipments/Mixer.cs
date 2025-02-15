using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mixer : Equipment, IDisplayableIngredientUI
{

    private List<KitchenObjectSO> baseIngredients;

    //UI
    [SerializeField] private Transform ingredientImageUITransform;

    public Transform IngredientImageUITransform
    {
        get => ingredientImageUITransform;
        set => ingredientImageUITransform = value;
    }
    private IngredientImageUI ingredientImageUI;

    private void Start()
    {
        baseIngredients = new List<KitchenObjectSO>();
        ingredientImageUI = IngredientImageUITransform.GetComponent<IngredientImageUI>();
    }

    public override void Interact(Player player)
    {
        if(player.GetKitchenObject() != null && player.GetKitchenObject() is BaseIngredient)
        {
            KitchenObject kitchenObject = player.GetKitchenObject();
            var kitchenObjectSO = kitchenObject.GetKitchenObjectSO();
            if (kitchenObjectSO.type == KitchenObjectSO.Type.BaseIngredient)
            {
                baseIngredients.Add(kitchenObjectSO);
                UpdateIngredientUI();
                player.GetKitchenObject().DestroySelf();
            }
        }
    }

    public void InteractAlternative(Player player)
    {
        if(!MixingSystem.Instance.IsMixing())
        {
            MixingSystem.Instance.StartMixing(baseIngredients, this);
        }
        else
        {
            MixingSystem.Instance.StopMixing();
        }
    }

    public void ClearIngredients()
    {
        baseIngredients?.Clear();
        UpdateIngredientUI();
    }

    public void UpdateIngredientUI()
    {
        ingredientImageUI.UpdateRecipeIngredientListUI(RecipeManager.InitRecipeIngredients(baseIngredients));
    }
}
