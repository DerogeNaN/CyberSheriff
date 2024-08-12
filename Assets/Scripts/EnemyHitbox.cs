using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 20;
    public float activeTime = 0.1f;

    float time;

    void Start()
    {
        time = activeTime;
    }

    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0) Destroy(gameObject);
    }
}
