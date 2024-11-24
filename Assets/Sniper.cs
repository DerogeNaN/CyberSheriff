using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : EnemyBase
{
    public float moveSpeed;

    public float chargeTime;
    public float timeBeforeDamage;
    public float cooldown;

    public List<LineRenderer> lineRenderers;
    LaserState laserState = LaserState.none;
    float laserIntensity = 0;

    float fireDuration;
    float endDuration;

    public Transform targetPos;
    bool reachedTargetPos = false;

    float timer = 0;

    protected override void OnStart()
    {
        enemy.shouldPath = false;
        enemy.speed = moveSpeed;
    }

    protected override void OnUpdate()
    {
        enemy.lookTarget = enemy.playerTransform.position;

        if (!reachedTargetPos) // if hasnt moved to sniper pos yet
        {
            if (enemy.hasLineOfSight)
            {
                // start heading to target pos
                enemy.shouldPath = true;
                enemy.moveTarget = targetPos.position;
            }
            
            if ((targetPos.position - transform.position).magnitude < 1.0f)
            {
                // reached target pos
                reachedTargetPos = true;
                enemy.shouldPath = false;
            }
        }
        else // if has reached sniper pos
        {

        }
    }
}