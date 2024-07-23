using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    private new void Start()
    {
        base.Start();
    }

    private new void Update()
    {
        base.Update();

        // moves when it has clear line of sight to the target
        shouldPath = targetLineOfSight;
    }
}
