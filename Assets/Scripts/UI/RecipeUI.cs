using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Transform contentTemplate;
    [SerializeField] private Transform ingredientContentTemplate;

    [SerializeField] private Button dishButton;
    [SerializeField] private Button stoveButton;
    [SerializeField] private Button mixerButton;

    public static RecipeUI Instance;
    private bool isOpened;
    private List<Transform> activeContentList;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        isOpened = false;

        activeContentList = new List<Transform>();

        dishButton.onClick.AddListener(OpenDish);
        stoveButton.onClick.AddListener(OpenStove);
        mixerButton.onClick.AddListener(OpenMixer);

        contentTemplate.gameObject.SetActive(false);
        ingredientContentTemplate.gameObject.SetActive(false);
        OpenDish();
    }

    public void OpenCloseShop()
    {
        if (!isOpened)
        {
            OpenRecipe();
        }
        else
        {
            CloseShop();
        }
    }

    public void OpenRecipe()
    {
        gameObject.SetActive(true);
        GameInput.Instance.UnlockCursor();
        Time.timeScale = 0f;
        Player.Instance.SetFreezedLook(true);
        isOpened = true;
    }

    public void CloseShop()
    {
        transform.gameObject.SetActive(false); // Hide the Shop UI
        GameInput.Instance.LockCursor(); // Lock and hide the cursor for gameplay
        Time.timeScale = 1f; // Resume the game
        Player.Instance.SetFreezedLook(false);
        isOpened = false;
    }

    private void OpenDish()
    {
        DestroyActiveContentItems();
        List<RecipeSO> recipeSOList = RecipeManager.Instance.GetAllRecipes();
        foreach(var recipe in recipeSOList)
        {
            Transform newRecipeContent = AddContentItem();
            activeContentList.Add(newRecipeContent);
            newRecipeContent.Find("ResultImage").GetComponent<RawImage>().texture = recipe.outputDish.icon;
            Transform ingredientContentTransform = newRecipeContent.Find("IngredientContent");

            foreach(var recipeIngredient in recipe.recipeIngredients)
            {
                Transform newIngredientContentTransform = AddIngredientContentItem(ingredientContentTransform);
                newIngredientContentTransform.Find("IngredientImage").GetComponent<RawImage>().texture = recipeIngredient.inputKitchenObject.icon;
                newIngredientContentTransform.Find("QuantityText").GetComponent<TextMeshProUGUI>().text = recipeIngredient.quantity.ToString();
            }
        }
    }

    private void DestroyActiveContentItems()
    {
        foreach (Transform content in activeContentList)
        {
            Destroy(content.gameObject);
        }
        activeContentList.Clear();
    }

    private void OpenStove()
    {
        DestroyActiveContentItems();
        List<IngredientRecipeSO> ingredientRecipeSOList = RecipeManager.Instance.GetStoveRecipes();
        foreach (var ingredientRecipe in ingredientRecipeSOList)
        {
            Transform newRecipeContent = AddContentItem();
            activeContentList.Add(newRecipeContent);
            newRecipeContent.Find("ResultImage").GetComponent<RawImage>().texture = ingredientRecipe.outputIngredient.icon;
            Transform ingredientContentTransform = newRecipeContent.Find("IngredientContent");

            foreach (var recipeIngredient in ingredientRecipe.recipeIngredients)
            {
                Transform newIngredientContentTransform = AddIngredientContentItem(ingredientContentTransform);
                newIngredientContentTransform.Find("IngredientImage").GetComponent<RawImage>().texture = recipeIngredient.inputKitchenObject.icon;
                newIngredientContentTransform.Find("QuantityText").GetComponent<TextMeshProUGUI>().text = recipeIngredient.quantity.ToString();
            }
        }
    }

    private void OpenMixer()
    {
        DestroyActiveContentItems();
        List<IngredientRecipeSO> ingredientRecipeSOList = RecipeManager.Instance.GetMixerRecipeSOs();
        foreach (var ingredientRecipe in ingredientRecipeSOList)
        {
            Transform newRecipeContent = AddContentItem();
            activeContentList.Add(newRecipeContent);
            newRecipeContent.Find("ResultImage").GetComponent<RawImage>().texture = ingredientRecipe.outputIngredient.icon;
            Transform ingredientContentTransform = newRecipeContent.Find("IngredientContent");

            foreach (var recipeIngredient in ingredientRecipe.recipeIngredients)
            {
                Transform newIngredientContentTransform = AddIngredientContentItem(ingredientContentTransform);
                newIngredientContentTransform.Find("IngredientImage").GetComponent<RawImage>().texture = recipeIngredient.inputKitchenObject.icon;
                newIngredientContentTransform.Find("QuantityText").GetComponent<TextMeshProUGUI>().text = recipeIngredient.quantity.ToString();
            }
        }
    }

    private Transform AddContentItem()
    {
        Transform newContentTransform = Instantiate(contentTemplate, contentTransform);
        newContentTransform.gameObject.SetActive(true);
        return newContentTransform;
    }

    private Transform AddIngredientContentItem(Transform ingredientContentTransform)
    {
        Transform newIngredientContentTransform = Instantiate(ingredientContentTemplate, ingredientContentTransform);
        newIngredientContentTransform.gameObject.SetActive(true);
        return newIngredientContentTransform;
    }
}
