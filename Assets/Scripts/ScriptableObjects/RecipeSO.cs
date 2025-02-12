using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredient
{
    public KitchenObjectSO inputKitchenObject; // Ingredient type.
    public int quantity; // How many units are needed.
}

[CreateAssetMenu(fileName = "New Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<RecipeIngredient> recipeIngredients; // List with quantities.
    public float processTime;
    public DishSO outputDish;
}