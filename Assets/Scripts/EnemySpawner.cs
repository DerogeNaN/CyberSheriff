using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    melee,
    ranged
}

public class EnemySpawner : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject enemy;
    public int count;
    public float radius;
    public float interval;

    float untilNextSpawn;
    int toSpawn;

    void Start()
    {
        untilNextSpawn = interval;
        toSpawn = count;
    }

    void Update()
    {
        if (toSpawn > 0)
        {
            untilNextSpawn -= Time.deltaTime;

            if (untilNextSpawn <= 0)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        toSpawn--;
        untilNextSpawn = interval;

        GameObject go = Instantiate(enemy, transform.position, transform.rotation);
        go.GetComponent<EnemyBase>().playerTransform = playerTransform;
    }
}
