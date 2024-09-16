using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 20;
    [HideInInspector] public bool active = true;
    [SerializeField] float activeTime = 0.2f;

    float time;

    void Start()
    {
        time = activeTime;
        active = true;
    }

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0) active = false;
    }
}
