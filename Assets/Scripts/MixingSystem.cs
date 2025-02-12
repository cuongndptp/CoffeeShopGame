using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingSystem : MonoBehaviour
{
    [SerializeField] private Transform mixingUITransform;
    [SerializeField] private RectTransform mixingCircle;
    [SerializeField] private Button closeButton;

    private bool isMixing = false;
    //Circular movement detection
    private Vector2 circleCenter; // The center of the circle
    private float lastAngle; // Previous angle
    private float totalAngleTraveled; // Total angle traveled (to detect full circle)

    //Singleton
    public static MixingSystem Instance;

    //Recipe matching
    private List<IngredientRecipeSO> ingredientRecipeSOs; //List of possible recipes

    //Mixing process
    private float maxProgress = 0f;
    private float currentProgress = 0f;
    private float progressValue = 2f;
    private Transform outputIngredientTransform;
    private Mixer activeMixer;

    //UI Update
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progress;
    }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mixingUITransform.gameObject.SetActive(false);
        totalAngleTraveled = 0f; // Initialize the angle traveled
        ingredientRecipeSOs = RecipeManager.Instance.GetAllIngredientRecipeSOs();
        closeButton.onClick.AddListener(StopMixing);
    }

    public void StartMixing(List<KitchenObjectSO> baseIngredients, Mixer mixer)
    {
        activeMixer = mixer;
        if (baseIngredients.Count > 0)
        {
            bool matched = false;

            foreach (var recipe in ingredientRecipeSOs)
            {
                if (IsIngredientsMatch(baseIngredients, recipe.recipeIngredients))
                {
                    matched = true;
                    mixingUITransform.gameObject.SetActive(true);
                    Player.Instance.SetFreezedLook(true);
                    circleCenter = mixingCircle.position; // Get the UI center position
                    

                    // Initialize the first angle
                    Vector2 direction = (Vector2)Input.mousePosition - circleCenter;
                    lastAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    GameInput.Instance.UnlockCursor();

                    isMixing = true;

                    //Mixing process
                    maxProgress = recipe.processTime;
                    outputIngredientTransform = recipe.outputIngredient.prefab;

                    break;
                }
            }

            if (!matched)
            {
                Debug.Log("No recipe matched with the provided ingredients.");
                activeMixer.ClearIngredients();
                activeMixer = null;
                return; // Don't start mixing if no match
            }
        }
        else
        {
            Debug.Log("No ingredient is added!");
        }

        
    }

    public void StopMixing()
    {
        mixingUITransform.gameObject.SetActive(false);
        Player.Instance.SetFreezedLook(false);
        isMixing = false;
        GameInput.Instance.LockCursor();
        maxProgress = 0f;
        currentProgress = 0f;
        outputIngredientTransform = null;
        activeMixer.ClearIngredients();
        activeMixer = null;
    }

    private void Update()
    {
        if (isMixing)
        {
            DetectCircleCompletion();
        }
    }

    

    private void DetectCircleCompletion()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 direction = mousePosition - circleCenter; // Get direction from center
        float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Convert to degrees

        // Calculate the angle difference, considering wrap-around
        float angleDifference = currentAngle - lastAngle;

        // Ensure the angle difference is in the range of -180 to 180 degrees
        if (angleDifference > 180f)
            angleDifference -= 360f;
        else if (angleDifference < -180f)
            angleDifference += 360f;

        // Accumulate the total angle traveled
        totalAngleTraveled += angleDifference;

        // Log completion of a full circle
        if (Mathf.Abs(totalAngleTraveled) >= 360f)
        {

            MixingProgress();

            totalAngleTraveled = 0f; // Reset for the next circle
        }

        // Update the last angle
        lastAngle = currentAngle;
    }

    private void MixingProgress()
    {
        if(currentProgress < maxProgress)
        {
            currentProgress += progressValue;
            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs
            {
                progress = currentProgress / maxProgress
            });
        }
        else if(currentProgress >= maxProgress)
        {
            Player.Instance.TryReleaseObject();
            Transform newIngredientTransform = Instantiate(outputIngredientTransform);
            KitchenObject newKitchenObject = newIngredientTransform.GetComponent<KitchenObject>();
            newKitchenObject.PlayerPickUp(Player.Instance);

            //Reset values
            StopMixing();

        }
    }

    private bool IsIngredientsMatch(List<KitchenObjectSO> playerIngredients, List<RecipeIngredient> requiredIngredients)
    {
        // First, create a dictionary from the player's ingredients for quick lookup
        Dictionary<KitchenObjectSO, int> playerIngredientsDict = new Dictionary<KitchenObjectSO, int>();

        foreach (var playerIngredient in playerIngredients)
        {
            if (playerIngredientsDict.ContainsKey(playerIngredient))
            {
                playerIngredientsDict[playerIngredient]++; // Increment the count if the ingredient already exists
            }
            else
            {
                playerIngredientsDict.Add(playerIngredient, 1); // Otherwise, add it with a count of 1
            }
        }

        // Now check if all required ingredients match the player's ingredients
        foreach (var requiredIngredient in requiredIngredients)
        {
            if (!playerIngredientsDict.ContainsKey(requiredIngredient.inputKitchenObject)) // Check if the ingredient exists in the player's list
            {
                return false; // If the player doesn't have the required ingredient, return false
            }

            if (playerIngredientsDict[requiredIngredient.inputKitchenObject] < requiredIngredient.quantity) // Check if the player has enough of that ingredient
            {
                return false; // If the player has less than required, return false
            }
        }

        return true; // All required ingredients matched
    }


    public bool IsMixing()
    {
        return isMixing;
    }

}
