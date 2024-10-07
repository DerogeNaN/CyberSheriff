using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRanged : EnemyBase
{
    [Header("Ranged Movement Settings")]
    public bool stationary = false;
    public float runSpeed = 5.0f;
    public float walkSpeed = 2.0f;
    public float stopDistance = 7.0f;

    [Header("Ranged Attack Settings")]
    public float attackRange = 25.0f;
    public float attackTime = 1.0f;
    public float attackCooldownMin = 1.0f;
    public float attackCooldownMax = 3.0f; 
    [SerializeField] GameObject bulletPrefab;

    Vector3 initialPosition;
    float remainingAttackTime;
    float remainingAttackCooldown;

    protected override void OnStart()
    {
        initialPosition = transform.position;
        speed = runSpeed;
        SetState(EnemyState.idle);
    }

    protected override void OnUpdate()
    {
        // do this regardless of state 
        lookTarget = playerTransform.position;
        if (remainingAttackCooldown > 0) remainingAttackCooldown -= Time.deltaTime;
    }

    #region enter state
    protected override void IdleEnter()
    {
        if (!stationary) shouldPath = true;
        moveTarget = initialPosition;
    }
    protected override void MovingToTargetEnter()
    {
        if (!stationary) shouldPath = true;
    }
    protected override void AttackingEnter()
    {
        shouldPath = false;
        remainingAttackTime = attackTime;
        speed = walkSpeed;

        // do attack here
        // change spawn pos to gun pos
        Projectile projectile = Instantiate(bulletPrefab, transform.position + lineOfSightOffset, transform.rotation).GetComponent<Projectile>();
        projectile.direction = Vector3.Normalize(playerTransform.position - transform.position - lineOfSightOffset);
    }
    #endregion

    #region update state
    protected override void IdleUpdate()
    {
        // if the player gets withing range and line of sight, switch to chasing them
        if (hasLineOfSight) SetState(EnemyState.movingToTarget);
    }
    protected override void MovingToTargetUpdate()
    {

        moveTarget = playerTransform.position;

        // if lost sight of the player, go back to idle
        if (!hasLineOfSight) SetState(EnemyState.idle);

        float distance = Vector3.Distance(transform.position, moveTarget);
        if (distance >= stopDistance)
        {
            if (!stationary) shouldPath = true;
        }
        else
        {
            shouldPath = false;
        }

        // can attack even before reached stopping distance
        if (remainingAttackCooldown <= 0)
        {
            SetState(EnemyState.attacking);
        }
    }
    protected override void AttackingUpdate()
    {
        // do attack stuff, then switch back to chasing
        remainingAttackTime -= Time.deltaTime;
        if (remainingAttackTime <= 0)
        {
            SetState(EnemyState.movingToTarget);
        }
    }

    #endregion

    #region exit state
    protected override void IdleExit()
    {

    }
    protected override void MovingToTargetExit()
    {

    }
    protected override void AttackingExit()
    {
        remainingAttackCooldown = Random.Range(attackCooldownMin, attackCooldownMax);
        speed = runSpeed;
    }
    #endregion
}
