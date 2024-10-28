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
    public bool doSniperAimEffect;
    public float sniperAimEffectLength;

    Vector3 initialPosition;
    float remainingAttackTime;
    float remainingAttackCooldown;
    float remainingSniperAimTime;

    protected override void OnStart()
    {
        initialPosition = transform.position;
        enemy.speed = runSpeed;
        SetState(EnemyState.idle);
    }

    protected override void OnUpdate()
    {
        // do this regardless of state 
        enemy.lookTarget = enemy.playerTransform.position;
        if (remainingAttackCooldown > 0) remainingAttackCooldown -= Time.deltaTime;
    }

    #region enter state
    protected override void IdleEnter()
    {
        if (!stationary) enemy.shouldPath = true;
        enemy.moveTarget = initialPosition;
    }
    protected override void MovingToTargetEnter()
    {
        if (!stationary) enemy.shouldPath = true;
    }
    protected override void AttackingEnter()
    {
        enemy.shouldPath = false;
        remainingAttackTime = attackTime;
        enemy.speed = walkSpeed;

        // do attack here
        // change spawn pos to gun pos
        Projectile projectile = Instantiate(bulletPrefab, transform.position + enemy.lineOfSightOffset, transform.rotation).GetComponent<Projectile>();
        projectile.Shoot(enemy.playerTransform.position);

        // snap to point at player when firing
        transform.LookAt(enemy.playerTransform);

        Vector3 rot = transform.rotation.eulerAngles;
        rot.x = 0;
        transform.rotation = Quaternion.Euler(rot);
    }

    protected override void ChargeAttackEnter()
    {
        remainingSniperAimTime = sniperAimEffectLength;
    }
    #endregion

    #region update state
    protected override void IdleUpdate()
    {
        // if the player gets withing range and line of sight, switch to chasing them
        if (enemy.hasLineOfSight) SetState(EnemyState.movingToTarget);
    }
    protected override void MovingToTargetUpdate()
    {

        enemy.moveTarget = enemy.playerTransform.position;

        // if lost sight of the player, go back to idle
        if (!enemy.hasLineOfSight) SetState(EnemyState.idle);

        float distance = Vector3.Distance(transform.position, enemy.moveTarget);
        if (distance >= stopDistance)
        {
            if (!stationary) enemy.shouldPath = true;
        }
        else
        {
            enemy.shouldPath = false;
        }

        // can attack even before reached stopping distance
        if (remainingAttackCooldown <= 0)
        {
            if (doSniperAimEffect) SetState(EnemyState.chargeAttack);
            else SetState(EnemyState.attacking);
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

    protected override void ChargeAttackUpdate()
    {
        Debug.DrawRay(transform.position + enemy.lineOfSightOffset, enemy.playerTransform.position - transform.position + enemy.lineOfSightOffset, new(1, 0, 0));

        remainingSniperAimTime -= Time.deltaTime;
        if (remainingSniperAimTime <= 0) SetState(EnemyState.attacking);
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
        enemy.speed = runSpeed;
    }
    #endregion
}
