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
    [SerializeField] private TextMeshProUGUI tutorialText;
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

    public void OpenCloseRecipeBook()
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
        foreach (var recipe in recipeSOList)
        {
            Transform newRecipeContent = AddContentItem();
            activeContentList.Add(newRecipeContent);
            newRecipeContent.Find("ResultImage").GetComponent<RawImage>().texture = recipe.outputDish.icon;
            Transform ingredientContentTransform = newRecipeContent.Find("IngredientContent");

            foreach (var recipeIngredient in recipe.recipeIngredients)
            {
                Transform newIngredientContentTransform = AddIngredientContentItem(ingredientContentTransform);
                newIngredientContentTransform.Find("IngredientImage").GetComponent<RawImage>().texture = recipeIngredient.inputKitchenObject.icon;
                newIngredientContentTransform.Find("QuantityText").GetComponent<TextMeshProUGUI>().text = recipeIngredient.quantity.ToString();
            }
        }
        tutorialText.text = DishTutorialText;
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
        tutorialText.text = StoveTutorialText;
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
        tutorialText.text = MixerTutorialText;
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

    private const string DishTutorialText =
        "Tutorial:\n" +
        "To complete a dish, you need a Plate and 1 to 4 ingredients.\n\n" +
        "- Pick up a plate by interacting (E) with it.\n" +
        "- Add ingredients to the dish by interacting (E) with them while holding a plate.\n" +
        "- Alternatively, you can pick up ingredients first and then place them on the plate.\n\n" +
        "Once you have enough ingredients, Left Click to place the dish on a nearby highlighted arrange zone.\n\n" +
        "Finally, interact (F) with the dish:\n" +
        "- If the ingredients are correct, the dish will be completed! \n" +
        "- If not, nothing will happen.";

    private const string StoveTutorialText =
        "Tutorial:\n" +
        "To cook, pick up an ingredient (E) and place it in the Stove (E).\n\n" +
        "Press (F) to start the stove. The timer will run twice:\n" +
        "- If you turn it off before the first run finishes, you can change the ingredient.\n" +
        "- After the first run, stopping the stove before the second timer finishes will give you the food **if the ingredients are correct**.\n" +
        "- If the timer runs out or the ingredients are wrong, you will get a Stove Junk.";

    private const string MixerTutorialText =
        "Tutorial:\n" +
        "Pick up ingredients (E) and place them in the Mixer (E).\n\n" +
        "If the ingredients are correct, press (F) to start mixing.\n\n" +
        "In the mixing screen, move your mouse to complete a circle and progress the mixing process.";
}
