using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] public GameObject[] wave1Enemies;
    [SerializeField] public GameObject[] wave2Enemies;
    [SerializeField] public GameObject[] wave3Enemies;
    private GameObject[][] waves;

    [SerializeField] private BoxCollider spawnZone;
    private Vector3 center;
    private Vector3 size;

    void Start()
    {
        // Always reassign spawnZone reference in case the reference is lost
        spawnZone = GetComponent<BoxCollider>();

        if (spawnZone == null)
        {
            Debug.LogError("Failed to retrieve BoxCollider component on spawnZone!");
            return;  // Exit the Start method if spawnZone is missing
        }

        // Proceed with setup if spawnZone exists
        center = transform.position;
        size = spawnZone.size;

        WaveManager.StartNewWave += PrepareNextWave;
        waves = new GameObject[][] { wave1Enemies, wave2Enemies, wave3Enemies };

        //spawnZone = GetComponent<BoxCollider>();
        //center = transform.position;
        //size = spawnZone.size;
    }

    void SpawnWave(int waveNumber)
    {
        foreach (GameObject enemy in waves[waveNumber]) 
        {
            Debug.Log("Attempting to spawn wave " + (WaveManager.waveManagerInstance.waveNumber + 1).ToString() + " enemies");
            Vector3 spawnPos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 0, Random.Range(- size.z / 2, size.z / 2));
            Instantiate(enemy, spawnPos, spawnZone.transform.rotation);
            WaveManager.waveManagerInstance.enemiesRemaining++;
        }
    }

    void PrepareNextWave()
    {
        if (spawnZone == null)
        {
            Debug.LogError("SpawnZone is null in PrepareNextWave!");
        }
        else
        {
            Debug.Log("SpawnZone is valid in PrepareNextWave.");
        }
        SpawnWave(WaveManager.waveManagerInstance.waveNumber);
    }
}
