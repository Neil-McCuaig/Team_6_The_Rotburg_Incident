using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
}

public class EnemySpawnerManager : MonoBehaviour
{
    public List<EnemySpawnData> enemySpawnPoints = new List<EnemySpawnData>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public void SpawnEnemies()
    {
        ClearEnemies(); 

        foreach (var data in enemySpawnPoints)
        {
            GameObject enemy = Instantiate(data.enemyPrefab, data.spawnPoint.position, Quaternion.identity);
            spawnedEnemies.Add(enemy);
        }
    }

    public void ClearEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();
    }
}
