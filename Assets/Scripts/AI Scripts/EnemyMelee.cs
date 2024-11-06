using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMelee : EnemyBase
{
    [Header("Melee Movement Settings")]
    [Tooltip("movement speed when chasing the player")]
    public float runSpeed = 5.0f;
    float walkSpeed = 2.0f; // unused, enemy doesn't return to spawn point
    float chaseTime = 2.0f; // unused, enemy doesn't leave the chase state

    [Header("Melee Attack Settings")]
    [Tooltip("the GameObject spawned when this attacks. should be the melee hitbox prefab")]
    public GameObject attackPrefab;
    [Tooltip("distance at which to start attacking.")]
    public float attackRange = 2.0f;
    [Tooltip("time between the attack animation starting, and the attack hitbox coming out")]
    public float attackStartUp = 0.5f;
    [Tooltip("remaining time in the attack animation after the hitbox comes out")]
    public float attackEndlag = 1.0f;
    [Tooltip("time before the enemy can attempt an attack again, even if the player is still in range")]
    public float attackCooldown = 1.0f;

    TMP_Text debugStunText;
    Vector3 initialPosition;
    Vector3 lastSeenPosition;
    float remainingChaseTime = 0;
    float remainingAttackTime = 0;
    float remainingAttackCooldown = 0;
    float stun = 0;
    Vector3 attackStartRotation;
    Vector3 attackTargetRotation;
    float attackRotate = 0;
    bool createdHitbox = false;
    float untilDestroy = 2.0f;

    protected override void OnStart()
    {
        SetState(EnemyState.idle);
        //SoundManager2.Instance.PlaySound("RobotSpawnSFX", enemy.transform);
        initialPosition = transform.position;
        enemy.moveTarget = initialPosition;
        enemy.speed = runSpeed;

        if (debugStunText) debugStunText.text = "";
    }

    protected override void OnUpdate()
    {
        // do this regardless of state 
        enemy.lookTarget = enemy.playerTransform.position;
        if (remainingAttackCooldown > 0) remainingAttackCooldown -= Time.deltaTime;

        if (enemy.health.health <= 0)
        {
            untilDestroy -= Time.deltaTime;
            if (untilDestroy <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public override void OnHit(int damage, int damageType)
    {
        stun = 0.5f;
        SetState(EnemyState.stunned);
    }

    public override void OnDestroyed(int damageType)
    {
        SetState(EnemyState.downed);
        enemy.animator.SetTrigger("Death");
    }

    #region enter state
    protected override void IdleEnter()
    {
        enemy.speed = walkSpeed;
        enemy.moveTarget = initialPosition;
    }
    protected override void MovingToTargetEnter()
    {
        enemy.speed = runSpeed;
        enemy.shouldPath = true;
        enemy.moveTarget = enemy.playerTransform.position;
        //SoundManager2.Instance.PlaySound("RobotSoundSFX", enemy.transform);
    }
    protected override void LostSightOfTargetEnter()
    {
        remainingChaseTime = chaseTime;
        enemy.shouldPath = true;
        enemy.moveTarget = lastSeenPosition;
    }
    protected override void AttackingEnter()
    {
        enemy.shouldPath = false;
        enemy.navAgent.avoidancePriority = 99;
        remainingAttackTime = attackEndlag + attackStartUp;

        // look at player
        Vector3 dir = (enemy.playerTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new(dir.x, 0, dir.z));

        enemy.animator.SetBool("Attack", true);
        enemy.animator.SetBool("Run", false);

        // set directions to lerp to quickly when attacking
        //attackStartRotation = transform.rotation.eulerAngles;
        //attackRotate = 0;

        //Vector3 dir = (enemy.playerTransform.position - transform.position).normalized;
        //attackTargetRotation = new(dir.x, 0, dir.z);
    }

    protected override void StunnedEnter()
    {
        enemy.shouldPath = false;
    }
    #endregion

    #region update state
    protected override void IdleUpdate()
    {
        // if has line of sight, chase player
        if (enemy.hasLineOfSight)
        {
            SetState(EnemyState.movingToTarget);
        }
    }
    protected override void MovingToTargetUpdate()
    {
        enemy.moveTarget = enemy.playerTransform.position;

        // if line of sight is lost, change to lost sight state
        //if (!enemy.hasLineOfSight) SetState(EnemyState.lostSightOfTarget);
        //else lastSeenPosition = enemy.playerTransform.position; // only update last seen pos if we didnt lose sight this frame

        if (enemy.navAgent.velocity.magnitude > 0) enemy.animator.SetBool("Run", true);
        else enemy.animator.SetBool("Run", false);

        // if has line of sight and within attack range, switch to attack state
        if (enemy.hasLineOfSight && remainingAttackCooldown <= 0 && Vector3.Distance(transform.position, enemy.playerTransform.position) <= attackRange)
        {
            SetState(EnemyState.attacking);
        }
    }
    protected override void LostSightOfTargetUpdate() // UNUSED
    {
        remainingChaseTime -= Time.deltaTime;
        // give up chasing
        if (remainingChaseTime <= 0)
        {
            SetState(EnemyState.idle);
        }

        // chase player
        if (enemy.hasLineOfSight)
        {
            SetState(EnemyState.movingToTarget);
        }
    }
    protected override void AttackingUpdate()
    {
        remainingAttackTime -= Time.deltaTime;

        // if startup time has passed, spawn the hitbox
        if (!createdHitbox && remainingAttackTime < attackEndlag)
        {
            if (attackPrefab != null)
            {
                GameObject hitbox = Instantiate(attackPrefab, enemy.mesh.transform);
                hitbox.transform.position = hitbox.transform.position + hitbox.transform.forward * 1.0f;
                createdHitbox = true; // reset when leaving attack state
            }
        }

        // if attack time ends, go back to idle and set attack cooldown
        if (remainingAttackTime <= 0)
        {
            SetState(EnemyState.movingToTarget);
        }

        // rotate to look at player
        //if (attackRotate < 1.0f) attackRotate += Time.deltaTime * 2.0f;
        //transform.rotation = Quaternion.Euler(Vector3.Lerp(attackStartRotation, attackTargetRotation, attackRotate));
    }

    protected override void StunnedUpdate()
    {
        if (debugStunText) debugStunText.text = "stun";

        stun -= Time.deltaTime;
        if (stun <= 0)
        {
            SetState(EnemyState.movingToTarget);
            if (debugStunText) debugStunText.text = "";
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
        enemy.navAgent.avoidancePriority = 50;
        createdHitbox = false;
    }
    #endregion
}
