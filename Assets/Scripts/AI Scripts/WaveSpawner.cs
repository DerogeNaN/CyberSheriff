using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] wave1Enemies;
    public GameObject[] wave2Enemies;
    public GameObject[] wave3Enemies;
    public GameObject[] wave4Enemies;
    public GameObject[] wave5Enemies;
    public GameObject[] wave6Enemies;
    public GameObject[] wave7Enemies;
    public GameObject[] wave8Enemies;
    public GameObject[] wave9Enemies;
    public GameObject[] wave10Enemies;
    private GameObject[][] waves;

    [SerializeField] private BoxCollider spawnZone;
    [SerializeField] private BoxCollider landingZone;
    private Vector3 centreSpawn;
    private Vector3 sizeSpawn;
    private Vector3 centreLanding;
    private Vector3 sizeLanding;

    void Start()
    {
        // Always reassign spawnZone reference in case the reference is lost
        spawnZone = GetComponent<BoxCollider>();

        if (spawnZone == null)
        {
            Debug.LogError("Failed to retrieve BoxCollider component on spawnZone!");
            return;  // Exit the Start method if spawnZone is missing
        }

        if (landingZone == null)
        {
            Debug.LogError("Spawning missing a LANDING ZONE!", this);
            return;
        }

        // Proceed with setup if a spawnZone & landingZone exists
        centreSpawn = transform.position;
        sizeSpawn = spawnZone.size;
        centreLanding = landingZone.transform.position;
        sizeLanding = landingZone.size;

        WaveManager.StartNewWave += PrepareNextWave;
        waves = new GameObject[][] {    wave1Enemies, wave2Enemies, wave3Enemies, 
                                        wave4Enemies, wave5Enemies, wave6Enemies, 
                                        wave7Enemies, wave8Enemies, wave9Enemies, wave10Enemies };
    }

    void SpawnWave(int waveNumber)
    {
        if (waves[waveNumber][0] == null) return;

        for (int i = 0; i < waves[waveNumber].Length; i++) 
        {
            Vector3 destination = Vector3.zero;

            Vector3 spawnPos = centreSpawn;
            Vector3 offset = new Vector3(Random.Range(-sizeSpawn.x / 2.0f, sizeSpawn.x / 2.0f), 0.0f, Random.Range(-sizeSpawn.z / 2.0f, sizeSpawn.z / 2.0f));

            Instantiate(waves[waveNumber][i], transform.localToWorldMatrix.MultiplyPoint3x4(offset), spawnZone.transform.rotation);
            WaveManager.waveManagerInstance.enemiesRemaining++;

            destination = landingZone.transform.position;
            destination += new Vector3(Random.Range(-sizeLanding.x / 2, sizeLanding.x / 2), 0, Random.Range(-sizeLanding.z / 2, sizeLanding.z / 2));
            //destination = landingZone.transform.localToWorldMatrix.MultiplyPoint3x4(destination);

            //destination = landingZone.transform.position;
            EnemyCommon currentEnemy = waves[waveNumber][i].gameObject.GetComponent<EnemyCommon>();
            currentEnemy.initialPosition = destination;
            //Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), destination, Quaternion.identity);
        }

        //NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        //spawnPos = hit.position;
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
