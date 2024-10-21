using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing;
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
    public float avoidFloorDistance;

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
        transform.position += towardsPlayer + GetAvoidance2();
    }

    private Vector3 GetAvoidance()
    {
        Vector3 avoidance = new();
        Vector3[] rays =
        {
             Quaternion.Euler(0, -45, 0) * transform.forward * avoidDistance, // left
             Quaternion.Euler(0, 45, 0) * transform.forward * avoidDistance,  // right
             new Vector3(0.0f, -1.0f, 0.0f) * avoidFloorDistance,             // down
             //transform.forward * avoidFloorDistance,                        // forward
        };

        // loop through rays for each direction and add to avoidance amount
        foreach (Vector3 v in rays)
        {
            Debug.DrawRay(transform.position, v);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, v, out hit, v.magnitude))
            {
                avoidance += hit.normal * (hit.distance * avoidanceDistanceWeight);
            }
        }

        // scale avoidance based on the percentage of current speed to max speed (i think)
        //avoidance *= ((currentSpeed / maxSpeed) * avoidanceSpeedWeight);

        return avoidance * avoidanceStrength;
    }

    private Vector3 GetAvoidance2()
    {
        Vector3 avoidance = new();

        Collider[] colliders = Physics.OverlapSphere(transform.position, avoidDistance, LayerMask.GetMask("Enemy"));

        foreach (Collider c in colliders)
        {
            if (!IsDescendantOf(transform, c.gameObject.transform) && c.gameObject.transform != transform)
            {
                Debug.Log(c.gameObject.name);
                Vector3 fromCollision = transform.position - c.transform.position;
                avoidance += fromCollision.normalized * (fromCollision.magnitude * avoidanceDistanceWeight);
            }
        }

        return avoidance * avoidanceStrength;
    }

    static bool IsDescendantOf(Transform maybeParent, Transform maybeChild)
    {
        if (maybeParent == maybeChild.parent) return true;
        if (maybeChild.parent == null) return false;
        return IsDescendantOf(maybeParent, maybeChild.parent);
    }
    static bool IsDescendantOf_NotRecursive(Transform maybeParent, Transform maybeChild)
    {
        Transform currentChild = maybeChild;

        while (true)
        {
            if (currentChild.parent == null) return false;
            if (currentChild.parent == maybeParent) return true;
            currentChild = currentChild.parent;
        }
    }
}
