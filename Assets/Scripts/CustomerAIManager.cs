using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAIManager : MonoBehaviour
{
    public static CustomerAIManager Instance;

    [SerializeField] private List<Transform> spawns;
    [SerializeField] private List<GameObject> npcPrefabs;
    private float spawnTimer = 0f;
    [SerializeField]
    private float baseSpawnTime = 25f;
    private float spawnTime = 0f;

    private List<CustomerAI> activeCustomers = new List<CustomerAI>();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateSpawnTime();
    }

    // Update is called once per frame
    void Update()
    {
        switch (TimeManager.Instance.GetCurrentTime())
        {
            case TimeManager.TimeOfDay.Day:
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnTime)
                {
                    //Spawn a NPC
                    spawnTimer = 0f;
                    SpawnNPC();
                }
                break;
            case TimeManager.TimeOfDay.Evening:
                AllCustomersLeave();
                break;
        }
    }

    private void SpawnNPC()
    {
        // Ensure we have valid spawns and NPC prefabs
        if (spawns.Count == 0 || npcPrefabs.Count == 0)
        {
            Debug.LogWarning("No spawns or NPC prefabs assigned in CustomerAIManager!");
            return;
        }

        // Randomly choose an NPC prefab and spawn point
        int npcIndex = Random.Range(0, npcPrefabs.Count);
        int spawnIndex = Random.Range(0, spawns.Count);

        GameObject npcPrefab = npcPrefabs[npcIndex];
        Transform spawnPoint = spawns[spawnIndex];

        // Instantiate the NPC at the chosen spawn point
        GameObject newNPC = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
        CustomerAI customer = newNPC.GetComponent<CustomerAI>();
        customer.enabled = true;
        customer.SetLeavingPoint(spawnPoint);
        if (customer != null)
        {
            activeCustomers.Add(customer);
            
        }
    }

    public void UpdateSpawnTime()
    {
        float reputation = ReputationManager.Instance.GetReputationFloat();
        spawnTime = baseSpawnTime - 2.5f * (reputation - 1);
    }

    private void AllCustomersLeave()
    {
        for (int i = activeCustomers.Count - 1; i >= 0; i--)
        {
            CustomerAI customer = activeCustomers[i];
            customer.Leave();
            activeCustomers.RemoveAt(i);
        }
    }
}
