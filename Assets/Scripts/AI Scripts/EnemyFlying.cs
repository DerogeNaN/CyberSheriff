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
    public float yOffset;

    [Header("Flying Advanced Settings")]
    public float closeDistance;
    public float zeroVelocityBuffer;

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
    }

    private void FixedUpdate()
    {
        if (toPlayer.magnitude > closeDistance)
        {
            // look at player
            dir = Vector3.Lerp(dir, toPlayer.normalized, turnSpeed);

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

        // apply movement
        transform.position += currentSpeed * dir * Time.deltaTime;
    }
}
