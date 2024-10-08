using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyFlying : EnemyBase
{
    [Header("Flying Settings")]
    public float targetHeight = 1.0f;
    public float targetDistance = 3.0f;
    public float maxSpeed = 5.0f;
    public float accelerationSpeed = 1.0f;
    public float decelerationSpeed = 2.0f;

    private Vector3 velcoity;

    protected override void OnStart()
    {
        SetState(EnemyState.idle);
        shouldPath = false;
    }

    protected override void OnUpdate()
    {
        lookTarget = playerTransform.position;

        transform.position += velcoity * Time.deltaTime;

        //if (velcoity.magnitude < 0.1f) velcoity = Vector3.zero;
    }

    protected override void IdleUpdate()
    {
        if (hasLineOfSight)
        {
            SetState(EnemyState.movingToTarget);
        }
        else if (velcoity.magnitude > 0)
        {
            velcoity -= velcoity.normalized * decelerationSpeed * Time.deltaTime;
        }
    }

    protected override void MovingToTargetUpdate()
    {
        if (!hasLineOfSight) SetState(EnemyState.idle);

        Vector3 toPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;
        
        if (distanceToPlayer >= targetDistance && velcoity.magnitude <= maxSpeed)
        {
            velcoity += toPlayer.normalized * accelerationSpeed * Time.deltaTime;
        }
        else
        {
            velcoity -= velcoity.normalized * decelerationSpeed * Time.deltaTime;
        }
    }
}
