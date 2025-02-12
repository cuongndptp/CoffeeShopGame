using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : MonoBehaviour
{
    private enum StoveState
    {
        WaitingForIngredients, Baking, Overbaking, Finished
    }

    private List<IngredientRecipeSO> stoveRecipes; //All recipe for stoves

    private List<RecipeIngredient> currentIngredients = new List<RecipeIngredient>();
    private KitchenObjectSO stoveJunkSO;
    private StoveState state;

    private KitchenObjectSO matchedKitchenObjectSO;
    private IngredientRecipeSO matchedIngredientRecipeSO;
    private KitchenObjectSO outputKitchenObjectSO;
    

    private bool isOn = false;
    private float timer = 0f;
    private float maxTimer = 0f;

    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progress;
    }

    private void Start()
    {
        stoveRecipes = RecipeManager.Instance.GetStoveRecipes();
        stoveJunkSO = RecipeManager.Instance.GetStoveJunkSO();
        state = StoveState.WaitingForIngredients;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progress = 0f
        });
    }

    public void Interact(Player player) // Player presses E
    {
        if (state == StoveState.Finished)
        {
            CollectDish(player);
            return;
        }

        if (player.GetKitchenObject() != null && state == StoveState.WaitingForIngredients)
        {
            KitchenObject playerKitchenObject = player.GetKitchenObject();
            KitchenObjectSO playerKitchenObjectSO = playerKitchenObject.GetKitchenObjectSO();

            if (playerKitchenObjectSO.type == KitchenObjectSO.Type.BaseIngredient || playerKitchenObjectSO.type == KitchenObjectSO.Type.Ingredient)
            {
                Debug.Log("Adding ingredient to stove");

                bool ingredientFound = false;
                foreach (var ingredient in currentIngredients)
                {
                    if (ingredient.inputKitchenObject == playerKitchenObjectSO)
                    {
                        ingredient.quantity += 1; // Increase quantity if already exists
                        ingredientFound = true;
                        break;
                    }
                }

                if (!ingredientFound)
                {
                    currentIngredients.Add(new RecipeIngredient { inputKitchenObject = playerKitchenObjectSO, quantity = 1 });
                }

                playerKitchenObject.Release(player);
                playerKitchenObject.DestroySelf();
            }
            else
            {
                Debug.Log("Invalid ingredient type");
            }
        }
    }

    private void Update()
    {
        if (isOn)
        {
            timer -= Time.deltaTime;
            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
            {
                progress = timer / maxTimer
            });

            if (timer <= 0f)
            {
                if (state == StoveState.Baking)
                {
                    outputKitchenObjectSO = matchedKitchenObjectSO;
                    if(matchedIngredientRecipeSO == null)
                    {
                        maxTimer = 30f;
                        timer = maxTimer; //Stove Junk process time
                    }else if(matchedIngredientRecipeSO != null)
                    {
                        maxTimer = matchedIngredientRecipeSO.processTime;
                        timer = maxTimer;
                    }
                    state = StoveState.Overbaking;
                    Debug.Log("Overbaking started!");
                    return;
                }
                if (state == StoveState.Overbaking)
                {
                    outputKitchenObjectSO = stoveJunkSO;
                    state = StoveState.Finished;
                    Debug.Log("Food burnt!");
                }
            }
        }
    }

    public void InteractAlternative() //Player press F
    {
        if (state == StoveState.WaitingForIngredients && currentIngredients.Count > 0 && !isOn)
        {
            StartBaking();
        }
        else if (state == StoveState.Baking)
        {
            StopBaking();
        }
        else if (state == StoveState.Overbaking)
        {
            CompleteOverbaking();
        }

    }

    private void StartBaking()
    {
        isOn = true;
        state = StoveState.Baking;

        foreach (var recipe in stoveRecipes)
        {
            if (IsIngredientsMatch(currentIngredients, recipe.recipeIngredients))
            {
                matchedIngredientRecipeSO = recipe;
                matchedKitchenObjectSO = recipe.outputIngredient;
                maxTimer = recipe.processTime;
                timer = maxTimer;
                Debug.Log("Recipe Matched! Baking...");
                return;
            }
        }

        // No match found, default to stove junk
        matchedKitchenObjectSO = stoveJunkSO;
        matchedIngredientRecipeSO = null;
        maxTimer = 30f;
        timer = maxTimer;
        Debug.Log("No recipe matched. Cooking junk...");
    }

    private void StopBaking()
    {
        isOn = false;
        maxTimer = 0f;
        timer = maxTimer;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progress = 0f
        });
        matchedIngredientRecipeSO = null;
        matchedKitchenObjectSO = null;
        state = StoveState.WaitingForIngredients;
        Debug.Log("Baking stopped.");
    }

    private void CompleteOverbaking()
    {
        isOn = false;
        state = StoveState.Finished;
        maxTimer = 0f;
        timer = maxTimer;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            progress = 0f
        });
        Debug.Log("Dish is ready!");
    }
    private bool IsIngredientsMatch(List<RecipeIngredient> playerIngredients, List<RecipeIngredient> requiredIngredients)
    {
        if (playerIngredients.Count != requiredIngredients.Count)
            return false;

        foreach (var required in requiredIngredients)
        {
            bool found = false;
            foreach (var playerIngredient in playerIngredients)
            {
                if (playerIngredient.inputKitchenObject == required.inputKitchenObject && playerIngredient.quantity == required.quantity)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }

        return true;
    }

    private void CollectDish(Player player)
    {
        if (outputKitchenObjectSO != null)
        {
            Transform outputKitchenObjectTransform = Instantiate(outputKitchenObjectSO.prefab);
            KitchenObject outputKitchenObject = outputKitchenObjectTransform.GetComponent<KitchenObject>();
            player.TryReleaseObject();
            outputKitchenObject.PlayerPickUp(player);
            outputKitchenObjectSO = null;
            matchedIngredientRecipeSO = null;
            matchedKitchenObjectSO = null;
            state = StoveState.WaitingForIngredients;
            currentIngredients.Clear();
            isOn = false;
            Debug.Log("Dish collected!");
        }
    }

}
