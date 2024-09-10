using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 20;
    public bool active = false;
    [SerializeField] float startUp = 0.0f;
    [SerializeField] float activeTime = 0.1f;

    float time;

    void Start()
    {
        time = startUp + activeTime;
    }

    void Update()
    {
        time -= Time.deltaTime;

        if (time - activeTime <= 0) active = true;

        if (time <= 0) Destroy(gameObject);
    }
}
