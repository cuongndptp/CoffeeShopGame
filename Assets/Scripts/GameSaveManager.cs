using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.KitchenObject;

public class GameSaveManager : MonoBehaviour
{
    private static string saveFilePath;
    public static GameSaveManager Instance;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        saveFilePath = Application.persistentDataPath + "/savegame.json";
    }

    public void SaveGame()
    {
        List<ObjectData> objectDataList = new List<ObjectData>();

        Debug.Log("Starting SaveGame process...");

        // Save KitchenObjects
        KitchenObject[] kitchenObjects = FindObjectsOfType<KitchenObject>();
        Debug.Log($"Found {kitchenObjects.Length} KitchenObjects to save.");

        foreach (var obj in kitchenObjects)
        {
            ObjectData data = obj.SaveData();
            objectDataList.Add(data);
            Debug.Log($"Saved KitchenObject: {data.objectName} at position {data.position[0]}, {data.position[1]}, {data.position[2]}");
        }

        // Save Player (if exists)
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            ObjectData playerData = player.SaveData();
            objectDataList.Add(playerData);
            Debug.Log($"Saved Player at position {playerData.position[0]}, {playerData.position[1]}, {playerData.position[2]}");
        }
        else
        {
            Debug.LogWarning("No Player found to save.");
        }

        // Save Player Money
        float playerMoney = MoneyManager.Instance?.GetPlayerMoney() ?? 0f;
        Debug.Log($"Saved Player Money: {playerMoney}");

        // Save Reputation & Day Count
        float totalRating = ReputationManager.Instance?.GetTotalRating() ?? 0f;
        int dayCount = TimeManager.Instance?.GetDayCount() ?? 0;

        // Create the save data object
        GameSaveData saveData = new GameSaveData
        {
            objects = objectDataList,
            playerMoney = playerMoney, // Save the player's money
            totalRating = totalRating, 
            dayCount = dayCount
        };

        string json = JsonUtility.ToJson(saveData, true);
        Debug.Log($"Generated Save JSON:\n{json}");

        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved Successfully!");
    }

    public void LoadGame()
    {
        Debug.Log("Starting LoadGame process...");

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        Debug.Log($"Loaded JSON from file:\n{json}");

        GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(json);

        if (loadedData == null || loadedData.objects == null || loadedData.objects.Count == 0)
        {
            Debug.LogError("Failed to deserialize save data! The file might be empty or corrupted.");
            return;
        }

        Debug.Log($"Found {loadedData.objects.Count} objects in save file.");

        // Load Player Money
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.SetPlayerMoney(loadedData.playerMoney);
            Debug.Log($"Loaded Player Money: {loadedData.playerMoney}");
        }
        else
        {
            Debug.LogError("MoneyManager instance not found! Unable to load money.");
        }

        // Load Reputation & Day Count
        ReputationManager.Instance?.SetTotalRating(loadedData.totalRating);
        TimeManager.Instance?.SetDayCount(loadedData.dayCount);

        // Destroy existing KitchenObjects
        foreach (var obj in FindObjectsOfType<KitchenObject>())
        {
            Debug.Log($"Destroying existing KitchenObject: {obj.name}");
            Destroy(obj.gameObject);
        }

        // Destroy existing Player if a saved player exists
        Player existingPlayer = FindObjectOfType<Player>();
        bool playerExistsInSave = loadedData.objects.Exists(data => data.objectType == "Player");

        if (existingPlayer != null && playerExistsInSave)
        {
            existingPlayer.UnsubscribeFromGameInput();
            Debug.Log("Destroying existing Player before loading new one.");
            DestroyImmediate(existingPlayer.gameObject);
        }

        // Load saved objects
        foreach (var data in loadedData.objects)
        {
            if (data.objectType == "KitchenObject")
            {
                Debug.Log($"Loading KitchenObject: {data.objectName}");

                KitchenObjectSO kitchenObjectSO = (KitchenObjectSO)ResourcesExtension.Load(data.objectName, typeof(KitchenObjectSO));

                if (kitchenObjectSO == null)
                {
                    Debug.LogWarning($"KitchenObjectSO '{data.objectName}' not found in Resources!");
                    continue;
                }

                GameObject prefab = kitchenObjectSO.prefab?.gameObject;
                if (prefab == null)
                {
                    Debug.LogWarning($"Prefab for '{data.objectName}' is missing!");
                    continue;
                }

                GameObject newObj = Instantiate(prefab,
                    new Vector3(data.position[0], data.position[1], data.position[2]),
                    Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]));

                if (newObj.TryGetComponent(out KitchenObject kitchenObject))
                {
                    kitchenObject.LoadData(data);
                    Debug.Log($"Successfully loaded KitchenObject: {data.objectName}");
                }
                else
                {
                    Debug.LogWarning($"Spawned object '{data.objectName}' does not have a KitchenObject component!");
                }
            }
            else if (data.objectType == "Player")
            {
                Debug.Log("Loading Player...");

                Player player = FindObjectOfType<Player>();

                if (player == null)
                {
                    Debug.Log("Player not found in scene. Trying to load Player prefab...");

                    GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
                    if (playerPrefab == null)
                    {
                        Debug.LogError("❌ Player prefab NOT FOUND in Resources/Prefabs!");
                        return;
                    }

                    Debug.Log("✅ Player prefab found! Instantiating...");
                    GameObject newPlayerObj = Instantiate(playerPrefab,
                        new Vector3(data.position[0], data.position[1], data.position[2]),
                        Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]));

                    if (newPlayerObj.TryGetComponent(out player))
                    {
                        Debug.Log("🎉 Successfully loaded Player!");
                        player.LoadData(data);
                    }
                    else
                    {
                        Debug.LogError("❌ ERROR: Spawned Player object does NOT have a Player component!");
                    }
                }
                else
                {
                    Debug.Log("✅ Existing Player found! Updating position...");
                    player.LoadData(data);
                }
            }
        }

        Debug.Log("Game Loaded Successfully!");
    }
}

[System.Serializable]
public class GameSaveData
{
    public List<ObjectData> objects;
    public float playerMoney;
    public float totalRating; // Save total rating
    public int dayCount; // Save day count
}
