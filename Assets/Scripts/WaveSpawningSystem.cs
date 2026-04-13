using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class WaveSpawningSystem : MonoBehaviour
{
    [SerializeField] private EnemyDetector enemyDetector;

    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<Wave> waves;

    private List<EnemyController> currentWaveEnemies = new();
    private EnemyDetector EnemyDetector;
    
    public int CurrentWaveIndex { get; private set; } = -1;

    private void Start()
    {
        EnemyDetector = FindFirstObjectByType<EnemyDetector>();

        BeginNextWave();
    }

    public void SpawnWave(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= waves.Count)
        {
            Debug.LogError("Invalid wave index: " + waveIndex);
            return;
        }

        Wave waveToSpawn = waves[waveIndex];
        foreach (WaveEnemy waveEnemy in waveToSpawn.EnemyTypes)
        {
            for (int i = 0; i < waveEnemy.Count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                GameObject enemyInstance = Instantiate(waveEnemy.EnemyPrefab, spawnPoint.position, Quaternion.identity);

                if(enemyInstance.TryGetComponent(out EnemyController enemyController))
                {
                    currentWaveEnemies.Add(enemyController);
                }
                else Debug.LogError("Spawned enemy does not have an EnemyController component: " + enemyInstance.name);
            }
        }
    }

    [Button("Spawn Next Wave")]
    public void BeginNextWave()
    {
        ++CurrentWaveIndex;
        SpawnWave(CurrentWaveIndex);
        enemyDetector.UpdateEnemyDisplayText(currentWaveEnemies.Count);
    }

    public void WaveEnemyDied(EnemyController enemyController)
    {
        currentWaveEnemies.Remove(enemyController);
        enemyDetector.UpdateEnemyDisplayText(currentWaveEnemies.Count);

        if (currentWaveEnemies.Count <= 0)
        {
            Debug.Log("Wave " + CurrentWaveIndex + " cleared!");

            if (CurrentWaveIndex >= waves.Count - 1) //if current wave is the last wave
            {
                Debug.Log("All waves cleared.");
                Invoke(nameof(AllWavesCleared), 2f); //load end scene after 2 seconds
            }
            else
            {
                Invoke(nameof(BeginNextWave), 4f); //start next wave after 4 seconds
            }
        }
    }
    private void AllWavesCleared()
    {
        SceneManager.LoadScene("EndScene");
    }

}

[Serializable]
public struct Wave
{
    public List<WaveEnemy> EnemyTypes;
}

[Serializable]
public struct WaveEnemy
{
    [RequiredType(typeof(EnemyController))] [AllowNesting]
    public GameObject EnemyPrefab;
    public int Count;
}