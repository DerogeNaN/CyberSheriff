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
    public float avoidFloorDistance;
    public float closeBuffer;

    [Header("FlyingAttack Settings")]
    public float attackRange = 25.0f;
    public float attackCooldownMin = 1.0f;
    public float attackCooldownMax = 3.0f;
    [SerializeField] GameObject bulletPrefab;

    [Header("Flying Randomise Settings")]
    public Vector2 randomiseTimerRange;
    public Vector2 closeDistanceRange;
    public Vector2 yOffsetRange;
    public Vector2 maxSpeedRange;

    Vector3 dir;
    Vector3 toPlayer;
    float currentSpeed;
    bool close;
    float timer;
    float attackCooldown;

    protected override void OnStart()
    {
        SetState(EnemyState.idle);
        enemy.shouldPath = false;
        dir = Vector3.zero;

        maxSpeed = Random.Range(maxSpeedRange.x, maxSpeedRange.y);
        yOffset = Random.Range(yOffsetRange.x, yOffsetRange.y);
        closeDistance = Random.Range(closeDistanceRange.x, closeDistanceRange.y);
        attackCooldown = Random.Range(attackCooldownMin, attackCooldownMax);
    }

    protected override void OnUpdate()
    {
        //timer -= Time.deltaTime;
        //if (timer <= 0) RandomiseValues();

        enemy.lookTarget = enemy.playerTransform.position;

        toPlayer = enemy.playerTransform.position - transform.position + new Vector3(0, yOffset, 0);
        close = toPlayer.magnitude <= closeDistance;

        // apply movement and rotation
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);
        //transform.rotation = Quaternion.LookRotation(new(dir.x, 0, dir.z));
        transform.position += ((currentSpeed * dir) + GetAvoidance2()) * Time.deltaTime;

        //transform.position += transform.right * 1.0f * Time.deltaTime;
    }

    private void FixedUpdate()
    {

    }

    protected override void IdleUpdate()
    {
        // if has line of sight, start chasing
        if (enemy.hasLineOfSight)
        {
            SetState(EnemyState.movingToTarget);
        }

        Decelerate();
    }

    protected override void MovingToTargetUpdate()
    {
        // if lost line of sight, go back to idle
        if (!enemy.hasLineOfSight)
        {
            SetState(EnemyState.idle);
        }

        // look at player
        dir = Vector3.Lerp(dir, toPlayer.normalized, turnSpeed);

        if (toPlayer.magnitude > closeDistance + closeBuffer)
        {
            // if far from player, move towards them
            if (currentSpeed < maxSpeed) currentSpeed += acceleration * Time.deltaTime;
        }
        else if (toPlayer.magnitude < closeDistance - closeBuffer)
        {
            // if close to player, move away from them
            if (currentSpeed > -maxSpeed) currentSpeed -= acceleration * Time.deltaTime;
        }
        else
        {
            Decelerate();
        }

        // attack
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0)
        {
            attackCooldown = Random.Range(attackCooldownMin, attackCooldownMax);

            // do attack here
            Projectile projectile = Instantiate(bulletPrefab, transform.position, transform.rotation).GetComponent<Projectile>();
            projectile.Shoot(enemy.playerTransform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("aaaa");
        if (!collision.gameObject.CompareTag("Player"))
        {

            ContactPoint contact = collision.GetContact(0);
            transform.position += contact.normal * contact.separation;
        }
    }

    private void Decelerate()
    {
        // decelerate
        float sign = Mathf.Sign(currentSpeed);
        currentSpeed -= deceleration * sign * Time.deltaTime;

        if (sign > 0) if (currentSpeed < 0) currentSpeed = 0;
            else if (sign < 0) if (currentSpeed > 0) currentSpeed = 0;
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
                avoidance += hit.normal;
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
                //Debug.Log(c.gameObject.name);
                Vector3 fromCollision = transform.position - c.transform.position;
                avoidance += fromCollision.normalized * (fromCollision.magnitude * avoidanceDistanceWeight);
            }
        }

        // floor avoidance
        if (Physics.Raycast(transform.position, -transform.up))
        {
            avoidance += transform.up * avoidanceStrength;
        }

        return avoidance * avoidanceStrength;
    }

    void RandomiseValues()
    {
        timer = Random.Range(randomiseTimerRange.x, randomiseTimerRange.y);
        yOffset = Random.Range(yOffsetRange.x, yOffsetRange.y);
        closeDistance = Random.Range(closeDistanceRange.x, closeDistanceRange.y);
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
