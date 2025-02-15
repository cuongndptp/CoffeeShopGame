using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientImageUI : MonoBehaviour
{
    [SerializeField] Transform ingredientImageTemplate;
    private List<Transform> currentActiveImageTransformList;
    

    private void Awake()
    {
        ingredientImageTemplate.gameObject.SetActive(false);
        currentActiveImageTransformList = new List<Transform>();
    }
    
    public void UpdateRecipeIngredientListUI(List<RecipeIngredient>  RecipeIngredientList)
    {
        ClearActiveUI();
        foreach (var recipeIngredient in RecipeIngredientList)
        {
            for(int i = 0;  i <= recipeIngredient.quantity - 1;  i++)
            {
                Transform newKitchenObjectImageTransform = Instantiate(ingredientImageTemplate, this.transform);
                newKitchenObjectImageTransform.gameObject.SetActive(true);
                currentActiveImageTransformList.Add(newKitchenObjectImageTransform);
                newKitchenObjectImageTransform.Find("IngredientImage").GetComponent<RawImage>().texture = recipeIngredient.inputKitchenObject.icon;
            }
        }
    }

    private void ClearActiveUI()
    {
        foreach(var item in currentActiveImageTransformList)
        {
            Destroy(item.gameObject);
        }
        currentActiveImageTransformList.Clear();
    }

}
