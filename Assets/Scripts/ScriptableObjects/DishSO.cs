using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dish")]
public class DishSO : ScriptableObject
{
    public float serveTime;
    public string dishName;
    public float eatingTime;
    public float price; // Float is fine, but use decimal for higher precision in currencies.
    public GameObject prefab; // Visual representation of the final dish.
    public RecipeSO recipe; // Reference to the recipe for making this dish.
    public Texture icon;
}
