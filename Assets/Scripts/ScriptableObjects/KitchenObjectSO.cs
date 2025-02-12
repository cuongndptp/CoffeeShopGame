using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class KitchenObjectSO : ScriptableObject
{
    public enum Type { Holder, Ingredient, Equipment, Stock, BaseIngredient, Dish }
    public Type type;
    public string Name;
    public string description;
    public Transform prefab;
    public float price;
    public Texture icon;
    
}
