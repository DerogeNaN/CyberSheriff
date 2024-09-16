using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : EnemyBase
{
    [Header("Melee Movement Settings")]
    public float runSpeed = 5.0f;
    public float walkSpeed = 2.0f;
    public float chaseTime = 2.0f;

    [Header("Melee Attack Settings")]
    public GameObject attackPrefab;
    public float attackRange = 2.0f;
    public float attackTime = 1.0f;
    public float attackCooldown = 1.0f;

    Vector3 initialPosition;
    Vector3 lastSeenPosition;
    float remainingChaseTime = 0;
    float remainingAttackTime = 0;
    float remainingAttackCooldown = 0;

    protected override void OnStart()
    {
        SetState(EnemyState.idle);

        initialPosition = transform.position;
        moveTarget = initialPosition;
        speed = walkSpeed;
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
        speed = walkSpeed;
        moveTarget = initialPosition;
    }
    protected override void MovingToTargetEnter()
    {
        speed = runSpeed;
        shouldPath = true;
    }
    protected override void LostSightOfTargetEnter()
    {
        remainingChaseTime = chaseTime;
        shouldPath = true;
        moveTarget = lastSeenPosition;
    }
    protected override void AttackingEnter()
    {
        shouldPath = false;
        remainingAttackTime = attackTime;

        if (attackPrefab != null)
        {
            GameObject hitbox = Instantiate(attackPrefab, mesh.transform);
            hitbox.transform.position = hitbox.transform.position + hitbox.transform.forward * 1.0f;
        }
    }
    #endregion

    #region update state
    protected override void IdleUpdate()
    {
        // if has line of sight, chase player
        if (hasLineOfSight)
        {
            SetState(EnemyState.movingToTarget);
        }
    }
    protected override void MovingToTargetUpdate()
    {
        moveTarget = playerTransform.position;

        // if line of sight is lost, change to lost sight state
        if (!hasLineOfSight) SetState(EnemyState.lostSightOfTarget);
        else lastSeenPosition = playerTransform.position; // only update last seen pos if we didnt lose sight this frame

        // if has line of sight and within attack range, switch to attack state
        if (hasLineOfSight && remainingAttackCooldown <= 0 && Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            SetState(EnemyState.attacking);
        }
    }
    protected override void LostSightOfTargetUpdate()
    {
        remainingChaseTime -= Time.deltaTime;
        // give up chasing
        if (remainingChaseTime <= 0)
        {
            SetState(EnemyState.idle);
        }

        // chase player
        if (hasLineOfSight)
        {
            SetState(EnemyState.movingToTarget);
        }
    }
    protected override void AttackingUpdate()
    {
        remainingAttackTime -= Time.deltaTime;
        // if attack time ends, go back to idle and set attack cooldown
        if (remainingAttackTime <= 0)
        {
            SetState(EnemyState.movingToTarget);
        }
    }
    #endregion

    #region exit state
    protected override void IdleExit()
    {
        //shouldPath = true;
        //moveTarget = initialPosition;
    }
    protected override void MovingToTargetExit()
    {

    }
    protected override void LostSightOfTargetExit()
    {

    }
    protected override void AttackingExit()
    {
        remainingAttackCooldown = attackCooldown;
    }
    #endregion
}
