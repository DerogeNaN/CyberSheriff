using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEnemy : Enemy
{
    public float minDistance = 5.0f;

    private new void Start()
    {
        base.Start();
    }

    private new void Update()
    {
        base.Update();

        // moves when it has line of sight, and is at least the min distance from the target
        if (targetLineOfSight && (target.position - transform.position).magnitude >= minDistance)
        {
            shouldPath = true;
        }
        else shouldPath = false;
    }
}
