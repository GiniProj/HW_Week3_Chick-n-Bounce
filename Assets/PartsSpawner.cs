using UnityEngine;

public class PartsSpawner : MonoBehaviour
{
    [Header("Main Parts Spawner Settings")]
    [Tooltip("The object we want to spawn")]
    [SerializeField] private GameObject mainObjectToSpawn;
    [Tooltip("The range of height we want to spawn the object in")]
    [SerializeField] private float mainHeightRangeOffset = 22f;
    [Tooltip("Delay time between spawning objects of the main")]
    [SerializeField] private float delayTimeMain = 2f;

    [Header("Secondary Parts Spawner Settings")]
    [Tooltip("The object we want to spawn")]
    [SerializeField] private GameObject secondaryObjectToSpawn;
    [Tooltip("Range of the height from the main object - according to its position on the Y axis from center main object")]
    [SerializeField] private float secondaryHeightRangeOffset = 6f;
    [Tooltip("The ratio of main to secondary objects - e.x 2 means 1 main objects to 2 secondary object")]
    [SerializeField] private int spawnRatioSecondaryToMain = 5; // The ratio of main to secondary objects
    [Tooltip("Delay time between spawning objects of the second")]
    [SerializeField] private float delayTimeSecond = 1f;

    private Vector3 lastMainSpawn = Vector3.zero;  // The last position we spawned the object in
    private int maxSpawnCount = 100; // Limit the number of spawns
    private int currentSpawnCount = 0;

    void Start()
    {
        Debug.Log("start to spawn objects...");
        spawner();
    }

    async void spawner()
    {
        while (currentSpawnCount < maxSpawnCount)
        {
            spawnMain();
            currentSpawnCount++;

            int counter = 0;
            do
            {
                await Awaitable.WaitForSecondsAsync(delayTimeSecond);
                spawnSecondary();
                currentSpawnCount++;
                counter++;
            } while (counter < spawnRatioSecondaryToMain);
            await Awaitable.WaitForSecondsAsync(delayTimeMain);
        }
    }

    void spawnMain()
    {
        float randomHeight = -1;
        randomHeight = Random.Range(-mainHeightRangeOffset, mainHeightRangeOffset);
        Vector3 spawnPosition = new Vector3(mainObjectToSpawn.transform.position.x, randomHeight, 0);
        Instantiate(mainObjectToSpawn, spawnPosition, Quaternion.identity);
        Debug.Log($"Spawned main object at position: {spawnPosition}");
        lastMainSpawn = spawnPosition;
    }

    void spawnSecondary()
    {
        float randomHeight = -1;
        int retryCount = 0;
        int maxRetries = 10;

        do
        {
            randomHeight = Random.Range(-secondaryHeightRangeOffset, secondaryObjectToSpawn.transform.position.y);
            retryCount++;
        } while (lastMainSpawn.y < randomHeight && retryCount < maxRetries);

        if (retryCount == maxRetries)
        {
            Debug.LogWarning("Max retries reached for secondary object height calculation.");
        }

        Vector3 secondarySpawnPosition = new Vector3(secondaryObjectToSpawn.transform.position.x, randomHeight, 0);
        Instantiate(secondaryObjectToSpawn, secondarySpawnPosition, Quaternion.identity);
        Debug.Log($"Spawned secondary object at position: {secondarySpawnPosition}");
    }
}
