using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawntest : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
}
