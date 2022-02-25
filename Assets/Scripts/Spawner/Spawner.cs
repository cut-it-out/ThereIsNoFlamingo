using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    // events
    public delegate void ProgressMultiplierChange(float newSpeed);
    public event ProgressMultiplierChange OnProgressMultiplierChange;


    [Header("Spawner Settings")]
    [SerializeField] Transform spawnZonePos;
    [SerializeField] float initialSpawnInterval = 1.7f;
    [SerializeField] float initialFallingSpeed = 2f;
    [SerializeField] float speedIncrementValue = 0.1f;
    [SerializeField] float spawnIntervalIncrementValue = -0.1f;
    [SerializeField] float maxProgressMultiplier = 5f;

    [Header("Spawner Input")]
    [SerializeField] GameObject prefabToSpawn;
    [SerializeField] int minItemsPerRow = 3;
    [SerializeField] int maxItemsPerRow = 6;

    List<FallingObject> fallingObjects = new List<FallingObject>();

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private float spawnInterval;
    private float fallingSpeed;
    private float progressMultiplier = 0f;

    private float minSpawnX;
    private float maxSpawnX;

    private Coroutine spawnerCR;

    // cached vars
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        InitBounds();
        UpdateFallingSpeedAndSpawnInterval();

    }

    public void InitSpawner()
    {
        StopSpawner(); //double check to be sure not to run several times :D
        spawnerCR = StartCoroutine(SpawnerTask());
    }

    public void StopSpawner()
    {
        if (spawnerCR != null) StopCoroutine(spawnerCR); 
        spawnerCR = null;
    }
    
    IEnumerator SpawnerTask()
    {
        while (true)
        {
            SpawnRow(UnityEngine.Random.Range(minItemsPerRow, maxItemsPerRow));
            yield return new WaitForSeconds(spawnInterval );
        }
    }

    private void InitBounds()
    {
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 0)); // stay on bottom line
    }

    private void SpawnRow(int nbrOfItemsToSpawn)
    {
        FallingObject fallingObj = prefabToSpawn.GetComponent<FallingObject>();
        fallingObj.GetPadding(out float objPaddingLeft, out float objPaddingRight);

        // set min & max X pos
        minSpawnX = minBounds.x ;
        maxSpawnX = maxBounds.x ;

        // TODO: add variance to available spawnLineDistance (have items more concentrated in middle)

        // calculate distance to place objects same distance from each other
        float spawnLineDistance = Vector2.Distance(minBounds, maxBounds);
        float spawnDistance = spawnLineDistance / nbrOfItemsToSpawn;

        if (spawnLineDistance < nbrOfItemsToSpawn * (objPaddingLeft + objPaddingRight))
        {
            Debug.LogWarning("Spawned object might collide as required padding is not available!");
        }

        // randomly select one real item
        int isRealIndex = UnityEngine.Random.Range(0, nbrOfItemsToSpawn);

        // loop through positions
        for (int i = 0; i < nbrOfItemsToSpawn; i++)
        {
            bool isReal = false;
            if (i == isRealIndex)
            {
                isReal = true;
            }

            FallingObject newFallingObject = 
                Spawn(new Vector2(minSpawnX + (spawnDistance / 2) + i * spawnDistance, spawnZonePos.transform.position.y), isReal);

            fallingObjects.Add(newFallingObject);
            
        }

    }

    private FallingObject Spawn(Vector2 spawnPos, bool isReal)
    {
        return FallingObject.Create(
            prefabToSpawn.transform, 
            spawnPos, 
            fallingSpeed, 
            isReal);
    }

    public void RemoveFallingObject(FallingObject fallingObj)
    {
        fallingObjects.Remove(fallingObj);
    }

    public void RemoveAllRemainingFallingObjects()
    {  
        while (fallingObjects.Count > 0)
        {
            fallingObjects[fallingObjects.Count - 1].DestroySelf();
        }

        fallingObjects.Clear();
    }

    public void IncreaseProgressMultiplier(float incrementValue = 1f)
    {
        progressMultiplier += incrementValue;
        UpdateFallingSpeedAndSpawnInterval();
        Debug.Log($"The Game just got harder, setting multiplier to {progressMultiplier}");

        // trigger change event
        OnProgressMultiplierChange?.Invoke(fallingSpeed);
    }

    private void UpdateFallingSpeedAndSpawnInterval()
    {
        spawnInterval = initialSpawnInterval + (spawnIntervalIncrementValue * progressMultiplier);
        fallingSpeed = initialFallingSpeed + (speedIncrementValue * progressMultiplier);
    }
}
