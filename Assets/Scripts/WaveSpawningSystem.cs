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

    private List<OilRigController> oilRigControllers;

    private bool skipOilRigPhase = false;

    private void Start()
    {
        EnemyDetector = FindFirstObjectByType<EnemyDetector>();
        oilRigControllers = new List<OilRigController>(FindObjectsByType<OilRigController>(FindObjectsSortMode.None));

        if (oilRigControllers.Count == 0)
        {
            Debug.LogWarning("No oil rigs found in scene. Wave Spawner will skip oil rig phase.");
            skipOilRigPhase = true;
        }
        else if (oilRigControllers.Count < waves.Count)
        {
            Debug.LogError("Not enough oil rigs in scene. Number of oil rigs should match number of waves.");
        }
        else if (oilRigControllers.Count != waves.Count)
        {
            Debug.LogWarning("Number of oil rigs in scene does not match number of waves");
        }

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

                if (enemyInstance.TryGetComponent(out EnemyController enemyController))
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

        //disable oil rig destructible
        foreach (OilRigController oilRig in oilRigControllers)
        {
            oilRig.SetDestructible(false);
        }
    }

    public void WaveEnemyDied(EnemyController enemyController)
    {
        currentWaveEnemies.Remove(enemyController);
        enemyDetector.UpdateEnemyDisplayText(currentWaveEnemies.Count);

        if (currentWaveEnemies.Count <= 0)
        {
            Debug.Log("Wave " + CurrentWaveIndex + " cleared!");

            //NOTE: an "end of a wave" is now logistically after an oil rig gets destroyed -- so it gets handled there unless oil rig phase is skipped

            if (!skipOilRigPhase)
            {
                foreach (OilRigController oilRig in oilRigControllers)
                {
                    oilRig.SetDestructible(true);
                }

                //continue in OilRigDestroyed() once an oil rig gets destroyed
            }
            else
            {
                if (CurrentWaveIndex >= waves.Count - 1) //current wave was last wave
                {
                    Invoke(nameof(AllWavesCleared), 2.0f);
                }
                else //go next wave
                {
                    Invoke(nameof(BeginNextWave), 2.0f);
                }
            }
        }
    }

    public void OilRigDestroyed(OilRigController oilRig)
    {
        oilRigControllers.Remove(oilRig);

        if (CurrentWaveIndex >= waves.Count - 1) //current wave was last wave
        {
            Invoke(nameof(AllWavesCleared), 2.0f);
        }
        else //go next wave
        {
            Invoke(nameof(BeginNextWave), 2.0f);
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
    [RequiredType(typeof(EnemyController))]
    [AllowNesting]
    public GameObject EnemyPrefab;
    public int Count;
}