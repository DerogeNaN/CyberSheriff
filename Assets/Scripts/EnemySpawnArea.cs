using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnArea : MonoBehaviour
{
    public Transform playerTransform;

    List<GameObject> enemies;
    bool triggered = false;
    BoxCollider colliderr;

    void Start()
    {
        enemies = new();
        colliderr = GetComponent<BoxCollider>();
        SetEnemiesInactive();
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
        Collider[] colliders = Physics.OverlapBox(transform.position, colliderr.size / 2);

        foreach (Collider c in colliders)
        {
            if (c.gameObject.CompareTag("Player")) SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        foreach (GameObject go in enemies)
        {
            go.SetActive(true);
            go.GetComponent<EnemyBase>().playerTransform = playerTransform;
            // ^ assuming the gameobject has EnemyBase, otherwise it shouldnt have been added to the list
        }
        triggered = true;
    }

    void SetEnemiesInactive()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, colliderr.size / 2);
        
        foreach (Collider c in colliders)
        {
            if (c.gameObject.GetComponent<EnemyBase>())
            {
                enemies.Add(c.gameObject);
                c.gameObject.SetActive(false);
            }
        }
    }
}
