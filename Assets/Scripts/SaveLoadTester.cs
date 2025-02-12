using UnityEngine;

public class SaveLoadTester : MonoBehaviour
{
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F5)) // Press F5 to Save
        //{
        //    Debug.Log("Saving game...");
        //    GameSaveManager.Instance.SaveGame();
        //}

        if (Input.GetKeyDown(KeyCode.F9)) // Press F9 to Load
        {
            Debug.Log("Loading game...");
            GameSaveManager.Instance.LoadGame();
        }
    }
}
