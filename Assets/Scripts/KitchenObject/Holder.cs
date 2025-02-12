using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Holder : KitchenObject
{
    [SerializeField] private Transform[] holderPoints;
    private List<HolderSlot> holderSlots = new List<HolderSlot>();
    private bool ReadyToArrange;
    protected override void Awake()
    {
        base.Awake();
        
        foreach (Transform point in holderPoints)
        {
            holderSlots.Add(new HolderSlot(point));
        }
    }

    private void Start()
    {
        ReadyToArrange = false;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // Player is holding an Ingredient
            if (player.GetKitchenObject() is Ingredient ingredient)
            {
                if (TryAddIngredient(ingredient))
                {
                    player.SetKitchenObject(null);
                }
                else
                {
                    Debug.Log("No available slot in this holder.");
                }
            }
        }
        else
        {
            // Player picks up this Holder
            ReadyToArrange = false;
            player.SetMode(Player.Mode.Put);
            player.SetKitchenObject(this);
            HoldTo(player.GetInteractPoint());
        }
    }

    public bool TryArrangeDish(out Transform dish)
    {
        if (ReadyToArrange)
        {
            if(holderSlots.Count >= 0)
            {
                // Gather all ingredients in the holderSlots
                List<Ingredient> currentIngredients = new List<Ingredient>();
                foreach (HolderSlot slot in holderSlots)
                {
                    
                    if (slot.ingredient != null)
                    {
                        currentIngredients.Add(slot.ingredient);
                    }
                }

                // Check for matching recipes
                foreach (RecipeSO recipe in RecipeManager.Instance.GetAllRecipes())
                {
                    if (IsRecipeMatch(recipe, currentIngredients))
                    {
                        Debug.Log("Recipe MATCHED!");
                        // Spawn the dish prefab
                        dish = Instantiate(recipe.outputDish.prefab.transform, transform.position, Quaternion.identity);

                        // Clear all slots and destroy the holder
                        foreach (HolderSlot slot in holderSlots)
                        {
                            if (slot.ingredient != null)
                            {
                                Destroy(slot.ingredient.gameObject);
                                slot.ingredient = null;
                            }
                        }
                        Destroy(gameObject); // Destroy this holder object

                        return true; // Successfully arranged a dish
                    }
                }

                dish = null;
                return false; // No matching recipe found
            }
        }
        dish = null;
        return false;
    }


    private bool IsRecipeMatch(RecipeSO recipe, List<Ingredient> currentIngredients)
    {
        // Check if the number of ingredients matches
        List<RecipeIngredient> currentRecipeIngredients = new List<RecipeIngredient>();
        foreach (Ingredient ingredient in currentIngredients)
        {
            // Use the KitchenObjectSO as the unique identifier for ingredients
            KitchenObjectSO ingredientSO = ingredient.GetKitchenObjectSO();

            // Try to find an existing match in the currentRecipeIngredients list
            RecipeIngredient existingRecipeIngredient = currentRecipeIngredients
                .FirstOrDefault(ri => ri.inputKitchenObject == ingredientSO);

            if (existingRecipeIngredient == null)
            {
                // If no match is found, add a new RecipeIngredient
                currentRecipeIngredients.Add(new RecipeIngredient
                {
                    inputKitchenObject = ingredientSO,
                    quantity = 1
                });
            }
            else
            {
                // If a match is found, increment the quantity
                existingRecipeIngredient.quantity++;
            }
        }
        if (recipe.recipeIngredients.Count != currentRecipeIngredients.Count)
        {
            return false;
        }
        // Check if all recipe ingredients are present
        //Loop through the recipe's ingredients
        foreach (RecipeIngredient recipeIngredient in recipe.recipeIngredients)
        {
            
            RecipeIngredient match = currentRecipeIngredients.Find(
                ri => ri.inputKitchenObject == recipeIngredient.inputKitchenObject && ri.quantity == recipeIngredient.quantity
            );

            if (match != null)
            {
                currentRecipeIngredients.Remove(match); // Remove the matched ingredient
            }
            else
            {
                return false; // A required ingredient is missing
            }
        }

        return true; // All ingredients match the recipe
    }
    public bool TryAddIngredient(Ingredient ingredient)
    {
        foreach (HolderSlot slot in holderSlots)
        {
            if (slot.ingredient == null)
            {
                slot.ingredient = ingredient;
                ingredient.HoldTo(slot.holderPoint);
                ingredient.SetHolder(this);
                
                return true;
            }
        }
        return false;
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        foreach (HolderSlot slot in holderSlots)
        {
            if (slot.ingredient == ingredient)
            {
                slot.ingredient = null;
                return;
            }
        }
    }

    public bool IsReadyToArrange()
    {
        return ReadyToArrange;
    }

    public void SetReadyToArrange(bool readyToArrange)
    {
        this.ReadyToArrange = readyToArrange;
    }

    public List<HolderSlot> GetHolderSlots() { return holderSlots; }

    [System.Serializable]
    public class HolderSlot
    {
        public Transform holderPoint;
        public Ingredient ingredient;

        public HolderSlot(Transform holderPoint)
        {
            this.holderPoint = holderPoint;
            this.ingredient = null;
        }
    }
}

