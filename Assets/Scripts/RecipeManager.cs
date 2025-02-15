using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [SerializeField] private List<RecipeSO> allDishRecipe;
    [SerializeField] private List<IngredientRecipeSO> allIngredientRecipe;
    [SerializeField] private List<IngredientRecipeSO> mixerRecipes;
    [SerializeField] private List<IngredientRecipeSO> stoveRecipes;

    [SerializeField] private KitchenObjectSO stoveJunk;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static List<RecipeIngredient> InitRecipeIngredients(List<KitchenObjectSO> kitchenObjects)
    {
        List<RecipeIngredient> recipeIngredients = new List<RecipeIngredient>();

        foreach (KitchenObjectSO kitchenObject in kitchenObjects)
        {
            recipeIngredients.Add(new RecipeIngredient
            {
                inputKitchenObject = kitchenObject,
                quantity = 1 // Default quantity; adjust as needed.
            });
        }

        return recipeIngredients;
    }

    public List<RecipeSO> GetAllRecipes()
    {
        return allDishRecipe;
    }

    public List<IngredientRecipeSO> GetAllIngredientRecipeSOs() { return allIngredientRecipe; }
    public List<IngredientRecipeSO> GetMixerRecipeSOs() {  return mixerRecipes; }
    public List<IngredientRecipeSO> GetStoveRecipes() { return stoveRecipes; }
    public KitchenObjectSO GetStoveJunkSO()
    {
        return stoveJunk;
    }
}
