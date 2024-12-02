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
    [Tooltip("maximum distance from the player to shoot")]
    public float attackRange = 25.0f;
    [Tooltip("remaining time in the animation after firing")]
    public float attackEndlag = 1.0f;
    [Tooltip("time before the enemy can shoot again. attackCooldown is randomised between the min and max so enemies don't keep shooting in sync")]
    public float attackCooldownMin = 1.0f;
    [Tooltip("time before the enemy can shoot again. attackCooldown is randomised between the min and max so enemies don't keeo shooting in sync")]
    public float attackCooldownMax = 3.0f;
    [Tooltip("the gameObject to spawn when shooting. should be the bullet prefab")]
    [SerializeField] GameObject bulletPrefab;
    //[Tooltip("whether or not to use the sniper's aiming effect before shooting")]
    bool doSniperAimEffect = false;
    //[Tooltip("if enabled, the amount of time the sniper's aiming effect lasts before the enemy shoots")]
    float sniperAimEffectLength;
    [Tooltip("if checked, enemies will never stop chasing the player once they see them for the first time")]
    public bool neverLoseSight;
    [Tooltip("whether or not line of sight to the player is required to start chasing them")]
    public bool needsLineOfSight;

    public float walkingSoundInterval = 2.0f;
    private float lastWalkingSoundTime = 0.0f;

    float remainingAttackTime;
    float remainingAttackCooldown;
    float remainingSniperAimTime;
    float stun = 0;
    float untilDestroy = 2.0f;
    [SerializeField] GameObject ragdoll;

    protected override void OnStart()
    {
        // randomly change stopDistance between -25% and +25% per enemy
        stopDistance += Random.Range(-stopDistance * 0.25f, stopDistance * 0.25f);

        enemy.moveTarget = enemy.initialPosition;
        enemy.speed = runSpeed;
        SetState(EnemyState.idle);
        enemy.animator.SetBool("Run", false);
    }

    protected override void OnUpdate()
    {
        // do this regardless of state 
        enemy.playerTransform = Movement.playerMovement.transform;
        enemy.lookTarget = enemy.playerTransform.position;
        if (remainingAttackCooldown > 0) remainingAttackCooldown -= Time.deltaTime;
    }

    public override void OnHit(int damage, int damageType)
    {
        stun = 0.5f;
        SetState(EnemyState.stunned);
        enemy.CreateHitEffect();
    }

    public override void OnDestroyed(int damage, int damageType)
    {
        //create ragdoll
        EnemyRagdoll rd = Instantiate(ragdoll, transform.position, transform.rotation).GetComponent<EnemyRagdoll>();
        if (damageType == 3) // if the damage was from explosion
        {
            Vector3 normal = (transform.position - enemy.playerTransform.position).normalized;
            rd.ApplyForce(new Vector3(normal.x, 0, normal.y).normalized, 300.0f);
        } // else do knockback based on damage
        else rd.ApplyForce((transform.position - enemy.playerTransform.position).normalized, damage > 50 ? 300.0f : 50.0f);


        SoundManager2.Instance.PlaySound("RobotDeath", transform);
        Destroy(gameObject);
    }

    #region enter state
    protected override void IdleEnter()
    {
        enemy.shouldPath = true;
        enemy.moveTarget = enemy.initialPosition;
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
        SoundManager2.Instance.PlaySound("RobotRangedAttack", transform);
        // snap to point at player when firing
        transform.LookAt(enemy.playerTransform);

        Vector3 rot = transform.rotation.eulerAngles;
        rot.x = 0;
        transform.rotation = Quaternion.Euler(rot);

        enemy.animator.SetTrigger("Attack");
    }

    protected override void ChargeAttackEnter()
    {
        remainingSniperAimTime = sniperAimEffectLength;
    }

    protected override void StunnedEnter()
    {
        enemy.shouldPath = false;
        enemy.animator.SetBool("Run", false);
        enemy.animator.SetBool("Stagger", true);

        SoundManager2.Instance.PlaySound("RobotStun", transform);
    }
    #endregion

    #region update state
    protected override void IdleUpdate()
    {
        enemy.speed = runSpeed;
        enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);

        if (lastWalkingSoundTime + walkingSoundInterval + Random.Range(0.0f, 2.0f) < Time.time)
        {
            SoundManager2.Instance.PlaySound("RobotWalking", enemy.transform);
            lastWalkingSoundTime = Time.time;
        }

        // if the player gets withing range and line of sight, switch to chasing them
        if (enemy.hasLineOfSight || !needsLineOfSight) SetState(EnemyState.movingToTarget);
    }
    protected override void MovingToTargetUpdate()
    {
        enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);

        enemy.moveTarget = enemy.playerTransform.position;

        if (lastWalkingSoundTime + walkingSoundInterval + Random.Range(0.0f, 2.0f) < Time.time)
        {
            SoundManager2.Instance.PlaySound("RobotWalking", enemy.transform);
            lastWalkingSoundTime = Time.time;
        }

        // if lost sight of the player, go back to idle
        if (!neverLoseSight && needsLineOfSight)
        {
            if (!enemy.hasLineOfSight) SetState(EnemyState.idle);
        }

        Vector3 toPlayer = enemy.playerTransform.position - transform.position;

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

    protected override void StunnedUpdate()
    {

        stun -= Time.deltaTime;
        if (stun <= 0)
        {
            SetState(EnemyState.movingToTarget);
        }
    }
    #endregion

    #region exit state
    protected override void IdleExit()
    {
        Debug.Log("among us");
    }
    protected override void MovingToTargetExit()
    {
        enemy.animator.SetBool("Run", false);
    }
    protected override void AttackingExit()
    {
        remainingAttackCooldown = Random.Range(attackCooldownMin, attackCooldownMax);
    }

    protected override void StunnedExit()
    {
        enemy.animator.SetBool("Stagger", false);
    }
    #endregion
}
