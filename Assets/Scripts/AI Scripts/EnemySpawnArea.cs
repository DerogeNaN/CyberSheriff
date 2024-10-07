using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnArea : MonoBehaviour
{
    [Tooltip("incase playerTransform isn't set for each enemy")]
    public Transform playerTransform;
    [Tooltip("checking this will automatically despawn enemies in the trigger area, and spawn them when the trigger activates")]
    public bool auto;
    public List<EnemyBase> enemies;

    bool triggered = false;
    BoxCollider colliderr;

    void Start()
    {
        colliderr = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        foreach (EnemyBase e in enemies)
        {
            e.playerTransform = playerTransform;
            if (!e.active) e.Spawn();
            // ^ assuming the gameobject has EnemyBase, otherwise it shouldnt have been added to the list
        }
        triggered = true;
    }
}
