using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;
    public Transform player;
    public float minSpawnRadius = 5f;
    public float maxSpawnRadius = 15f;
    public int maxEnemies = 10;
    public float spawnInterval = 3f;

    private float timer;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Update()
    {
        if (EnergyMeter.Instance.SanityCheck())
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval && CountEnemies() < maxEnemies)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
            if (CountEnemies() > 0)
                clearEnemies();
        }

    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Vector3 directionToPlayer = (player.position - spawnPos).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(directionToPlayer);

        GameObject enemy = Instantiate(prefab, spawnPos, spawnRotation);
        spawnedEnemies.Add(enemy);
    }

    Vector3 GetRandomSpawnPosition()
    {
        float theta = Random.Range(0f, Mathf.PI * 2f);
        float phi = Mathf.Acos(Random.Range(-1f, 1f));
        float radius = Mathf.Pow(
            Random.Range(
                Mathf.Pow(minSpawnRadius, 3f),
                Mathf.Pow(maxSpawnRadius, 3f)
            ), 1f / 3f
        );

        float x = player.position.x + radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = player.position.y + radius * Mathf.Cos(phi);
        float z = player.position.z + radius * Mathf.Sin(phi) * Mathf.Sin(theta);

        return new Vector3(x, y, z);
    }

    int CountEnemies()
    {
        spawnedEnemies.RemoveAll(e => e == null);
        return spawnedEnemies.Count;
    }

    void clearEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null) 
            {
                if (enemy.TryGetComponent<HitCircle>(out HitCircle hitCircle))
                    hitCircle.DestroyEnemy();
                else
                    Destroy(enemy); 
            }
        }
        spawnedEnemies.Clear();
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, maxSpawnRadius);
    }
}
