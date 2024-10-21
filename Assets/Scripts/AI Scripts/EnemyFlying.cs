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

        Vector3 left = Quaternion.Euler(0, -45, 0) * transform.position;
        Debug.DrawRay(transform.position, left);
    }

    private void FixedUpdate()
    {
        // look at player
        dir = Vector3.Lerp(dir, toPlayer.normalized, turnSpeed);

        if (toPlayer.magnitude > closeDistance)
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

        Vector3 left = Quaternion.Euler(0, -45, 0) * transform.position;
        Debug.DrawRay(transform.position, left);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, left, out hit, avoidDistance))
        {
            avoidance -= hit.normal * avoidanceStrength;
        }

        return avoidance;
    }
}
