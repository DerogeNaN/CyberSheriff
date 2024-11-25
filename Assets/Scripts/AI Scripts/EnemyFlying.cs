using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : EnemyBase
{
    [Header("Flying Movement Settings")]
    [Tooltip("maximuim speed the enemy can be moving")]
    public float maxSpeed;
    [Tooltip("speed at which the enemy turns towards the player")]
    public float turnSpeed;
    [Tooltip("rate at which the enemy increases in speed while chasing the player")]
    public float acceleration;
    [Tooltip("rate at which the enemy decreases in speed while not chasing the player")]
    public float deceleration;

    [Header("Flying Offset Settings")]
    [Tooltip("height offset from the player's y position to move towards")]
    public float yOffset;
    [Tooltip("the distance the enemy should try to keep from the player")]
    public float closeDistance;
    [Tooltip("the distance to the floor the enemy can be before trying to move upwards")]
    public float avoidFloorDistance;

    [Header("Flying Attack Settings")]
    [Tooltip("the maximum range at which the enemy can shoot at the player")]
    public float attackRange = 25.0f;
    [Tooltip("time before the enemy can shoot again. attackCooldown is randomised between the min and max so enemies don't keep shooting in sync")]
    public float attackCooldownMin = 1.0f;
    [Tooltip("time before the enemy can shoot again. attackCooldown is randomised between the min and max so enemies don't keep shooting in sync")]
    public float attackCooldownMax = 3.0f;
    [Tooltip("the gameObject to spawn when shooting. should be the bullet prefab")]
    [SerializeField] GameObject bulletPrefab;

    [Header("Flying Randomise Settings")]
    [Tooltip("whether or not to randomise movement values every so often")]
    public bool randomiseValues;
    [Tooltip("if enabled, this is the time to wait between randomising the values. the timer itself is a random range")]
    public Vector2 randomiseTimerRange;
    [Tooltip("see \"closeDistance\"")]
    public Vector2 closeDistanceRange;
    [Tooltip("see \"yOffset\"")]
    public Vector2 yOffsetRange;
    [Tooltip("see \"maxSpeed\"")]
    public Vector2 maxSpeedRange;

    [Header("Flying Advanced Settings")]
    [Tooltip("strength multiplier for avoiding other flying enemies")]
    public float avoidanceStrength;
    [Tooltip("max distance to detect nearby enemies")]
    public float avoidDistance;
    [Tooltip("weight multiplier for the influence of distance to enemies")]
    public float avoidanceDistanceWeight;
    [Tooltip("weight multiplier for the influence of the current speed")]
    public float avoidanceSpeedWeight;
    [Tooltip("the distance range in which the enemy is considered both near enough and far enough from the player to stop moving. stops jittering between forwards and backwards")]
    public float closeBuffer;
    [Tooltip("multiplier for the acceleration when moving away from the player")]
    public float backwardsAccelerationMultiplier = 1.0f;

    Vector3 dir;
    Vector3 toPlayer;
    float currentSpeed;
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
        if (randomiseValues)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) RandomiseValues();
        }

        enemy.lookTarget = enemy.playerTransform.position;

        toPlayer = enemy.playerTransform.position - transform.position + new Vector3(0, yOffset, 0);

        // apply movement and rotation
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(new(dir.x, 0, dir.z));
        transform.position += ((currentSpeed * dir) + GetAvoidance2()) * Time.deltaTime;

        //transform.position += transform.right * 1.0f * Time.deltaTime;
    }

    public override void OnDestroyed(int damage, int damageType)
    {
        Destroy(gameObject);
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
        //if (!enemy.hasLineOfSight) SetState(EnemyState.idle);

        // look at player
        dir = Vector3.Lerp(dir, toPlayer.normalized, turnSpeed * Time.deltaTime);

        if (toPlayer.magnitude > closeDistance + closeBuffer)
        {
            // if far from player, move towards them
            if (currentSpeed < maxSpeed) currentSpeed += acceleration * Time.deltaTime;
        }
        else if (toPlayer.magnitude < closeDistance - closeBuffer)
        {
            // if close to player, move away from them
            if (currentSpeed > -maxSpeed) currentSpeed -= acceleration * backwardsAccelerationMultiplier * Time.deltaTime;
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
        //Debug.Log("aaaa");
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
                avoidance += fromCollision.normalized * ((avoidDistance - fromCollision.magnitude) * avoidanceDistanceWeight);
            }
        }

        // floor avoidance
        RaycastHit floorHit;
        if (Physics.Raycast(transform.position, -transform.up, out floorHit, avoidFloorDistance))
        {
            avoidance += transform.up * (avoidFloorDistance - floorHit.distance) * 10.0f;
        }

        return avoidance * avoidanceStrength;
    }

    void RandomiseValues()
    {
        timer = Random.Range(randomiseTimerRange.x, randomiseTimerRange.y);
        yOffset = Random.Range(yOffsetRange.x, yOffsetRange.y);
        closeDistance = Random.Range(closeDistanceRange.x, closeDistanceRange.y);
    }

    bool IsDescendantOf(Transform maybeParent, Transform maybeChild)
    {
        if (maybeParent == maybeChild.parent) return true;
        if (maybeChild.parent == null) return false;
        return IsDescendantOf(maybeParent, maybeChild.parent);
    }
}
