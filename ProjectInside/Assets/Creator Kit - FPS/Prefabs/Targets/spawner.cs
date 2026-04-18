using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnInterval = 2f;
    public int maxSpawnCount = 10;

    private float timer;
    private int currentCount;

    void Update()
    {
        if (prefabToSpawn == null || currentCount >= maxSpawnCount)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        Instantiate(prefabToSpawn, transform.position, transform.rotation);
        currentCount++;
    }
}
