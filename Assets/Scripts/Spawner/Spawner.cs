using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    // events
    public delegate void ProgressMultiplierChange(float newSpeed);
    public event ProgressMultiplierChange OnProgressMultiplierChange;


    [Header("Spawner Settings")]
    [SerializeField] Transform spawnZoneXMinPos;
    [SerializeField] Transform spawnZoneXMaxPos;
    [SerializeField] Transform spawnZoneYPos;
    [SerializeField] float initialSpawnInterval = 1.7f;
    [SerializeField] float initialFallingSpeed = 2f;
    [SerializeField] float speedIncrementValue = 0.1f;
    [SerializeField] float spawnIntervalIncrementValue = -0.1f;
    [SerializeField] float maxProgressMultiplier = 200f;

    [Header("Spawner Input")]
    [SerializeField] GameObject prefabToSpawn;
    [SerializeField] int minItemsPerRow = 3;
    [SerializeField] int maxItemsPerRow = 6;
    [SerializeField] float verticalVariance = 1f;

    List<FallingObject> fallingObjects = new List<FallingObject>();

    private float spawnInterval;
    private float fallingSpeed;
    private float progressMultiplier = 0f;

    private const float INCREMENT_MULTIPLIER_BASE = 1f;

    private float minSpawnX;
    private float maxSpawnX;
    private float spawnLineDistance;

    private Coroutine spawnerCR;

    // cached vars
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // set min & max X pos
        minSpawnX = spawnZoneXMinPos.transform.position.x;
        maxSpawnX = spawnZoneXMaxPos.transform.position.x;
                
        // calculate distance to place objects same distance from each other
        spawnLineDistance = Vector2.Distance(spawnZoneXMinPos.transform.position, spawnZoneXMaxPos.transform.position);

        // trigger player move speed update
        OnProgressMultiplierChange?.Invoke(initialFallingSpeed);

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

    private void SpawnRow(int nbrOfItemsToSpawn)
    {
        FallingObject fallingObj = prefabToSpawn.GetComponent<FallingObject>();
        fallingObj.GetPadding(out float objPaddingLeft, out float objPaddingRight);

        // TODO: add variance to available spawnLineDistance (have items more concentrated in middle)

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

            float spawnPosX = minSpawnX + (spawnDistance / 2) + i * spawnDistance;

            float spawnPosY = UnityEngine.Random.Range(
                spawnZoneYPos.transform.position.y - (verticalVariance / 2), 
                spawnZoneYPos.transform.position.y + (verticalVariance / 2));

            Vector2 spawnPos = new Vector2(
                spawnPosX,
                spawnPosY);

            FallingObject newFallingObject = Spawn(spawnPos, isReal);

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

    public float GetProgressMultiplier() => progressMultiplier;

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

    public void IncreaseProgressMultiplier(float incrementValue = INCREMENT_MULTIPLIER_BASE)
    {
        if (progressMultiplier == maxProgressMultiplier) return;

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
