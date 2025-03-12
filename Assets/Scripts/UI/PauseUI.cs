using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetDayButton;
    [SerializeField] private Button mainMenuButton;
    public static PauseUI Instance;
    private bool isOpened = false;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        GameInput.Instance.OnPause += GameInput_OnPause;

        continueButton.onClick.AddListener(HidePauseScreen);
        resetDayButton.onClick.AddListener(ResetDay);
        mainMenuButton.onClick.AddListener(BackToMenu);
    }

    private void GameInput_OnPause(object sender, System.EventArgs e)
    {
        TogglePauseScreen();
    }

    private void ResetDay()
    {
        GameSaveManager.Instance.LoadGame();
    }
    private void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void TogglePauseScreen()
    {
        if(!isOpened)
        {
            ShowPauseScreen();
            

        }
        else
        {
            HidePauseScreen();
            
        }
    }

    private void ShowPauseScreen()
    {
        TimeManager.Pause();
        gameObject.SetActive(true);
        isOpened = true;
    }

    private void HidePauseScreen()
    {
        TimeManager.StopPause();
        gameObject.SetActive(false);
        isOpened = false;
    }
}

