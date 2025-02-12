using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient Recipe")]
public class IngredientRecipeSO : ScriptableObject
{
    public List<RecipeIngredient> recipeIngredients;
    public KitchenObjectSO outputIngredient; // Output is a kitchen object, coule be a base ingredient or an ingredient (e.g., dough, batter)
    public float processTime;
}

/*
public class RecipeIngredient
{
    public KitchenObjectSO inputKitchenObject; // Ingredient type.
    public int quantity; // How many units are needed.
}
*/