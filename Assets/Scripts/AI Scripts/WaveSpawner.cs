using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] public GameObject[] wave1Enemies;
    [SerializeField] public GameObject[] wave2Enemies;
    [SerializeField] public GameObject[] wave3Enemies;
    [SerializeField] public GameObject[] wave4Enemies;
    [SerializeField] public GameObject[] wave5Enemies;
    [SerializeField] public GameObject[] wave6Enemies;
    [SerializeField] public GameObject[] wave7Enemies;
    [SerializeField] public GameObject[] wave8Enemies;
    [SerializeField] public GameObject[] wave9Enemies;
    [SerializeField] public GameObject[] wave10Enemies;
    private GameObject[][] waves;

    [SerializeField] private BoxCollider spawnZone;
    [SerializeField] private BoxCollider landingZone;
    [SerializeField] private Vector3 centerSpawn;
    [SerializeField] private Vector3 sizeSpawn;

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
        centerSpawn = transform.position;
        sizeSpawn = spawnZone.size;

        WaveManager.StartNewWave += PrepareNextWave;
        waves = new GameObject[][] {    wave1Enemies, wave2Enemies, wave3Enemies, 
                                        wave4Enemies, wave5Enemies, wave6Enemies, 
                                        wave7Enemies, wave8Enemies, wave9Enemies, wave10Enemies };
    }

    void SpawnWave(int waveNumber)
    {
        //for (int i = 0; i < waves[waveNumber].Length; i++) 
        //{
        //    Vector3 spawnPos = centerSpawn + new Vector3(Random.Range(-sizeSpawn.x / 2, sizeSpawn.x / 2), 0, Random.Range(-sizeSpawn.z / 2, sizeSpawn.z / 2));
        //}

        foreach (GameObject enemy in waves[waveNumber]) 
        {
            //Debug.Log("Attempting to spawn wave " + (WaveManager.waveManagerInstance.waveNumber + 1).ToString() + " enemies");
            Vector3 spawnPos = centerSpawn + new Vector3(Random.Range(-sizeSpawn.x / 2, sizeSpawn.x / 2), 0, Random.Range(- sizeSpawn.z / 2, sizeSpawn.z / 2));
            Vector3 offset = new Vector3(Random.Range(-sizeSpawn.x / 2, sizeSpawn.x / 2), 0, Random.Range(-sizeSpawn.z / 2, sizeSpawn.z / 2));


            Instantiate(enemy, spawnPos, spawnZone.transform.rotation);
            WaveManager.waveManagerInstance.enemiesRemaining++;
        }

        

        //NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        //spawnPos = hit.position;
    }

    Vector3[] FindLandingPoints()
    {
        return new Vector3[] { };
    }

    void PrepareNextWave()
    {
        if (spawnZone == null)
        {
            Debug.LogError("SpawnZone is null in PrepareNextWave!");
        }
        else SpawnWave(WaveManager.waveManagerInstance.waveNumber);
    }
}
