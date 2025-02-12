using UnityEngine;
using System.IO;

public class ResourcesExtension
{
    public static string ResourcesPath = Application.dataPath + "/Resources";

    public static UnityEngine.Object Load(string resourceName, System.Type systemTypeInstance)
    {
        Debug.Log($"Searching for '{resourceName}' in Resources...");

        string[] directories = Directory.GetDirectories(ResourcesPath, "*", SearchOption.AllDirectories);

        foreach (var dir in directories)
        {
            string relativePath = dir.Substring(ResourcesPath.Length + 1).Replace("\\", "/");

            UnityEngine.Object result = Resources.Load(relativePath + "/" + resourceName, systemTypeInstance);

            if (result != null)
            {
                Debug.Log($"Found and loaded '{resourceName}' from '{relativePath}'");
                return result;
            }
        }

        Debug.LogWarning($"Failed to find '{resourceName}' in Resources subfolders!");
        return null;
    }
}