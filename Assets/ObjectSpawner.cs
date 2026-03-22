using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] objectPrefabs;
    public Transform player;
    public float minSpawnRadius = 5f;
    public int maxObjects = 10;
    public float spawnInterval = 3f;

    [Header("Box Bounds")]
    public Vector3 boxCenter = Vector3.zero;
    public Vector3 boxSize = new Vector3(50f, 50f, 50f);

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && CountObjects() < maxObjects)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    void SpawnObject()
    {
        Vector3 spawnPos;
        if (!TryGetSpawnPosition(out spawnPos)) return;

        GameObject prefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
        Vector3 directionToPlayer = (player.position - spawnPos).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(directionToPlayer);
        Instantiate(prefab, spawnPos, spawnRotation);
    }

    bool TryGetSpawnPosition(out Vector3 result)
    {
        int maxAttempts = 30;
        Bounds bounds = new Bounds(boxCenter, boxSize);

        for (int i = 0; i < maxAttempts; i++)
        {
            // Pick a random point inside the box
            Vector3 candidate = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // Reject if inside the min radius around the player
            if (Vector3.Distance(candidate, player.position) >= minSpawnRadius)
            {
                result = candidate;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    int CountObjects()
    {
        return GameObject.FindGameObjectsWithTag("Breakable").Length;
    }

    void OnDrawGizmosSelected()
    {
        // Draw static box
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCenter, boxSize);

        // Draw inner exclusion sphere around player
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minSpawnRadius);
        }
    }
}
