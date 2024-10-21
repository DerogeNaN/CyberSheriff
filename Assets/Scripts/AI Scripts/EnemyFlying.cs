using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : EnemyBase
{
    [Header("Flying Movement Settings")]
    public float maxSpeed;
    public float turnSpeed;
    public float acceleration;
    public float deceleration;

    [Header("Flying Offset Settings")]
    public float yOffset;
    public float closeDistance;

    [Header("Flying Advanced Settings")]
    public float avoidanceStrength;
    public float avoidDistance;
    public float avoidanceDistanceWeight;
    public float avoidanceSpeedWeight;

    Vector3 dir;
    Vector3 toPlayer;
    float currentSpeed;
    bool close;

    protected override void OnStart()
    {
        SetState(EnemyState.idle);
        enemy.shouldPath = false;
        dir = Vector3.zero;
    }

    protected override void OnUpdate()
    {
        enemy.lookTarget = enemy.playerTransform.position;

        toPlayer = enemy.playerTransform.position - transform.position + new Vector3(0, yOffset, 0);
        close = toPlayer.magnitude <= closeDistance;

        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void FixedUpdate()
    {
        // look at player
        dir = Vector3.Lerp(dir, toPlayer.normalized, turnSpeed);

        if (enemy.hasLineOfSight && toPlayer.magnitude > closeDistance)
        {
            // increase speed
            if (currentSpeed < maxSpeed) currentSpeed += acceleration;
            else currentSpeed = maxSpeed;
        }
        else
        {
            // decrease speed
            if (currentSpeed > 0.0f) currentSpeed -= deceleration;
            else currentSpeed = 0.0f;
        }

        Vector3 towardsPlayer = currentSpeed * dir;

        // apply movement
        transform.position += towardsPlayer + GetAvoidance();
    }

    private Vector3 GetAvoidance()
    {
        Vector3 avoidance = new();
        Vector3[] rays =
        {
             Quaternion.Euler(0, -45, 0) * transform.forward, // left
             Quaternion.Euler(0, 45, 0) * transform.forward, // right
        };

        // loop through rays for each direction and add to avoidance amount
        foreach (Vector3 v in rays)
        {
            Debug.DrawRay(transform.position, v * avoidDistance);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, v, out hit, avoidDistance))
            {
                avoidance += hit.normal * (hit.distance * avoidanceDistanceWeight);
            }
        }

        // scale avoidance based on the percentage of current speed to max speed (i think)
        avoidance *= ((currentSpeed / maxSpeed) * avoidanceSpeedWeight);

        return avoidance * avoidanceStrength;
    }
}
