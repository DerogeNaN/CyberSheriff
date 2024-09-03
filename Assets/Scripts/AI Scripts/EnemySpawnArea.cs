using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnArea : MonoBehaviour
{
    public Transform playerTransform;
    public bool auto;
    public List<EnemyBase> enemies;

    bool triggered = false;
    BoxCollider colliderr;

    void Start()
    {
        //enemies = new();
        colliderr = GetComponent<BoxCollider>();
       // SetEnemiesInactive();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            SpawnEnemies();
        }
    }

    void Update()
    {
        //if (!triggered)
        //{
        //    Collider[] colliders = Physics.OverlapBox(transform.position, colliderr.size / 2);

        //    foreach (Collider c in colliders)
        //    {
        //        if (c.gameObject.CompareTag("Player"))
        //        {
        //            SpawnEnemies();
        //        }
        //    }
        //}
    }

    void SpawnEnemies()
    {
        foreach (EnemyBase e in enemies)
        {
            e.playerTransform = playerTransform;
            e.Spawn();
            // ^ assuming the gameobject has EnemyBase, otherwise it shouldnt have been added to the list
        }
        triggered = true;
    }

    void SetEnemiesInactive()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, colliderr.size / 2);
        
        foreach (Collider c in colliders)
        {
            EnemyBase e = c.gameObject.GetComponent<EnemyBase>();
            if (e)
            {
                enemies.Add(e);
                e.Despawn();
            }
        }
    }
}
