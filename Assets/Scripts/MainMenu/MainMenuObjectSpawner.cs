using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        public float spawnInterval = 2f; // Time between spawns for this object
        public int spawnCount = 1; // Number to spawn at once
    }

    [Header("Spawn Settings")]
    [SerializeField] private List<SpawnableObject> objectsToSpawn = new List<SpawnableObject>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private bool randomizeSpawnPoints = true;
    [SerializeField] private bool startSpawningOnAwake = true;
    
    [Header("Spawn Timing")]
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private bool useRandomIntervals = false;
    [SerializeField] private Vector2 randomIntervalRange = new Vector2(1f, 3f);

    private List<float> spawnTimers;
    private bool isSpawning = false;

    void Start()
    {
        if (startSpawningOnAwake)
        {
            StartSpawning();
        }
    }

    void Update()
    {
        if (!isSpawning || objectsToSpawn.Count == 0 || spawnPoints.Count == 0) return;

        // Update spawn timers
        for (int i = 0; i < objectsToSpawn.Count; i++)
        {
            spawnTimers[i] -= Time.deltaTime;
            
            if (spawnTimers[i] <= 0)
            {
                SpawnObject(i);
                
                // Reset timer
                if (useRandomIntervals)
                {
                    spawnTimers[i] = Random.Range(randomIntervalRange.x, randomIntervalRange.y);
                }
                else
                {
                    spawnTimers[i] = objectsToSpawn[i].spawnInterval;
                }
            }
        }
    }

    void SpawnObject(int objectIndex)
    {
        SpawnableObject spawnable = objectsToSpawn[objectIndex];
        
        for (int i = 0; i < spawnable.spawnCount; i++)
        {
            // Select spawn point
            Transform spawnPoint = GetSpawnPoint();
            
            if (spawnPoint != null && spawnable.prefab != null)
            {
                // Instantiate the object
                GameObject spawnedObject = Instantiate(spawnable.prefab, spawnPoint.position, spawnPoint.rotation);
                spawnedObject.tag = "MenuFloatingObject";
            }
        }
    }

    Transform GetSpawnPoint()
    {
        if (spawnPoints.Count == 0) return null;
        
        if (randomizeSpawnPoints)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        }
        else
        {
            // Cycle through spawn points in order
            currentSpawnPointIndex = (currentSpawnPointIndex + 1) % spawnPoints.Count;
            return spawnPoints[currentSpawnPointIndex];
        }
    }

    private int currentSpawnPointIndex = -1;

    public void StartSpawning()
    {
        isSpawning = true;
        
        // Initialize timers
        spawnTimers = new List<float>();
        for (int i = 0; i < objectsToSpawn.Count; i++)
        {
            spawnTimers.Add(initialDelay);
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void ClearAllSpawnedObjects()
    {
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("FloatingObject");
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
    }
}
