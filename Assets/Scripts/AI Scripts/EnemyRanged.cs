using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Ranged Movement Settings")]
    [Tooltip("movement speed when chasing the player")]
    public float runSpeed = 5.0f;
    [Tooltip("distance to stop moving towards the player. ignores this if no line of sight")]
    public float stopDistance = 7.0f;

    [Header("Ranged Attack Settings")]
    [Tooltip("minimum distance to the player to shoot")]
    public float attackRange = 25.0f;
    [Tooltip("remaining time in the animation after firing")]
    public float attackEndlag = 1.0f;
    [Tooltip("time before the enemy can shoot again. attackCooldown is randomised between the min and max so enemies don't keep shooting in sync")]
    public float attackCooldownMin = 1.0f;
    [Tooltip("time before the enemy can shoot again. attackCooldown is randomised between the min and max so enemies don't keeo shooting in sync")]
    public float attackCooldownMax = 3.0f;
    [Tooltip("the gameObject to spawn when shooting. should be the bullet prefab")]
    [SerializeField] GameObject bulletPrefab;
    [Tooltip("whether or not to use the sniper's aiming effect before shooting")]
    public bool doSniperAimEffect;
    [Tooltip("if enabled, the amount of time the sniper's aiming effect lasts before the enemy shoots")]
    public float sniperAimEffectLength;

    Vector3 initialPosition;
    float remainingAttackTime;
    float remainingAttackCooldown;
    float remainingSniperAimTime;

    protected override void OnStart()
    {
        // randomly change stopDistance between -25% and +25% per enemy
        stopDistance += Random.Range(-stopDistance * 0.25f, stopDistance * 0.25f);

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
        enemy.shouldPath = true;
        enemy.moveTarget = initialPosition;
    }
    protected override void MovingToTargetEnter()
    {
        enemy.shouldPath = true;
    }
    protected override void AttackingEnter()
    {
        enemy.shouldPath = false;
        remainingAttackTime = attackEndlag;

        // do attack here
        // change spawn pos to gun pos
        Projectile projectile = Instantiate(bulletPrefab, transform.position + enemy.lineOfSightOffset, transform.rotation).GetComponent<Projectile>();
        projectile.Shoot(enemy.playerTransform.position);
        //SoundManager2.Instance.PlaySound("RobotLazerSFX", enemy.transform);
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
        //if (!enemy.hasLineOfSight) SetState(EnemyState.idle);

        Vector3 toPlayer = enemy.playerTransform.position - transform.position;

        enemy.shouldPath = toPlayer.magnitude > stopDistance || !enemy.hasLineOfSight;

        if (enemy.hasLineOfSight && remainingAttackCooldown <= 0)
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
    }
    #endregion
}
