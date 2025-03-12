using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitGameButton;

    //[SerializeField] private string mainSceneName; // Change from Scene to string for SceneManager

    void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(ContinueGame);
        quitGameButton.onClick.AddListener(QuitGame);

        // Disable "Continue" button if no save exists
        if (!GameSaveManager.Instance || !System.IO.File.Exists(Application.persistentDataPath + "/savegame.json"))
        {
            continueButton.interactable = false;
        }
    }

    private void NewGame()
    {
        Debug.Log("Starting a new game...");

        // Delete save file
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.DeleteSave();
        }

        // Load main scene
        SceneManager.sceneLoaded += OnNewGameSceneLoaded; // Attach event listener
        SceneManager.LoadScene(1);
    }

    private void OnNewGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("New game scene loaded. Saving game...");

        // Ensure we're in the correct scene before saving
        if (scene.buildIndex == 1 && GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SaveGame();
        }

        // Remove listener to prevent it from triggering multiple times
        SceneManager.sceneLoaded -= OnNewGameSceneLoaded;
    }

    private void ContinueGame()
    {
        Debug.Log("Continuing game...");

        // Load the main scene first
        SceneManager.sceneLoaded += OnContinueGameSceneLoaded; // Attach event listener
        SceneManager.LoadScene(1);
    }

    private void OnContinueGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Main scene loaded. Loading saved game...");

        if (scene.buildIndex == 1 && GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.LoadGame();
        }

        // Remove listener after loading to prevent multiple calls
        SceneManager.sceneLoaded -= OnContinueGameSceneLoaded;
    }

    //private IEnumerator InitializeGameSaveAfterNewGame()
    //{
    //    yield return new WaitForSeconds(1f); // Wait for the scene to load
    //    if (GameSaveManager.Instance != null)
    //    {
    //        GameSaveManager.Instance.SaveGame();
    //    }
    //}

    //private IEnumerator LoadGameAfterSceneLoad()
    //{
    //    yield return new WaitForSeconds(1f); // Wait for the scene to load
    //    if (GameSaveManager.Instance != null)
    //    {
    //        GameSaveManager.Instance.LoadGame();
    //    }
    //}



    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}