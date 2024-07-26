using System.Collections;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] cloudPrefabs; // Array of cloud prefabs
    [SerializeField] private float spawnInterval = 5f; // Time between each spawn
    [SerializeField] private Vector2 spawnRange = new Vector2(-10f, 10f); // Range within which clouds will spawn
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float originalScale = 0.001f;
    [SerializeField] private float waitBeforeDespawn = 10f;


    void Start()
    {
        StartCoroutine(SpawnClouds());
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            SpawnCloud(waitBeforeDespawn);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnCloud(float deathTime)
    {
        int index = Random.Range(0, cloudPrefabs.Length);
        GameObject cloud = Instantiate(cloudPrefabs[index], GetRandomSpawnPosition(), Quaternion.identity);

        // Randomize cloud properties
        float randomScale = Random.Range(0.5f, 2f);
        cloud.transform.localScale = new Vector3(randomScale * originalScale, randomScale * originalScale, 1f);
        
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        cloud.GetComponent<Cloud>().SetSpeed(randomSpeed);

        // Destroy cloud after it moves out of view
        Destroy(cloud, deathTime);
    }

    Vector2 GetRandomSpawnPosition()
    {
        float x = this.transform.position.x;
        float y = this.transform.position.y + Random.Range(spawnRange.y, spawnRange.y + 5f); // Adjust based on your scene
        return new Vector2(x, y);
    }
}
