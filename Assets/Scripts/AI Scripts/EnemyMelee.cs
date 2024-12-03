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
    [Tooltip("if checked, enemies will never stop chasing the player once they see them for the first time")]
    public bool neverLoseSight;
    [Tooltip("whether or not line of sight to the player is required to start chasing them")]
    public bool needsLineOfSight;

    public float walkingSoundInterval = 2.0f;
    private float lastWalkingSoundTime = 0.0f;

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
    [SerializeField] GameObject ragdoll;

    protected override void OnStart()
    {
        SetState(EnemyState.idle);
        SoundManager2.Instance.PlaySound("RobotSpawn", transform);
        //initialPosition = transform.position;
        enemy.moveTarget = enemy.initialPosition;
        enemy.speed = runSpeed;
    }

    protected override void OnUpdate()
    {
        // do this regardless of state 
        enemy.lookTarget = enemy.playerTransform.position;
        if (remainingAttackCooldown > 0) remainingAttackCooldown -= Time.deltaTime;
    }

    public override void OnHit(int damage, int damageType)
    {
        stun = 0.5f;
        SetState(EnemyState.stunned);
        enemy.CreateHitEffect();

        SoundManager2.Instance.PlaySound("RobotHit", transform);

        if (damage > 50 && damageType == 1) SoundManager2.Instance.PlaySound("RobotWeakPointHit", transform);
    }

    public override void OnDestroyed(int damage, int damageType)
    {
        //create ragdoll
        EnemyRagdoll rd = Instantiate(ragdoll, transform.position, transform.rotation).GetComponent<EnemyRagdoll>();
        if (damageType == 3) // if the damage was from explosion
        {
            Vector3 normal = (transform.position - enemy.playerTransform.position).normalized;
            rd.ApplyForce(new Vector3(normal.x, Mathf.Abs(normal.y), normal.y).normalized, 300.0f);
        } // else do knockback based on damage
        else rd.ApplyForce((transform.position - enemy.playerTransform.position).normalized, damage > 50 ? 300.0f : 50.0f);

        Destroy(gameObject);
    }

    #region enter state
    protected override void IdleEnter()
    {
        enemy.speed = walkSpeed;
        enemy.moveTarget = enemy.initialPosition;
        enemy.shouldPath = true;
    }
    protected override void MovingToTargetEnter()
    {
        enemy.speed = runSpeed;
        enemy.shouldPath = true;
        enemy.moveTarget = enemy.playerTransform.position;
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

        SoundManager2.Instance.PlaySound("RobotMelee", transform);

        // set directions to lerp to quickly when attacking
        //attackStartRotation = transform.rotation.eulerAngles;
        //attackRotate = 0;

        //Vector3 dir = (enemy.playerTransform.position - transform.position).normalized;
        //attackTargetRotation = new(dir.x, 0, dir.z);
    }

    protected override void StunnedEnter()
    {
        enemy.shouldPath = false;
        enemy.animator.SetBool("Run", false);
        enemy.animator.SetBool("Attack", false);
        enemy.animator.SetBool("Stagger", true);

        SoundManager2.Instance.PlaySound("RobotStun", transform);
    }
    #endregion

    #region update state
    protected override void IdleUpdate()
    {
        // if has line of sight, chase player
        if (enemy.hasLineOfSight || !needsLineOfSight)
        {
            SetState(EnemyState.movingToTarget);
        }

        if (lastWalkingSoundTime + walkingSoundInterval + Random.Range(0.0f, 2.0f) < Time.time)
        {
            SoundManager2.Instance.PlaySound("RobotWalking", enemy.transform);
            lastWalkingSoundTime = Time.time;
        }

        enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);


        /*if (Physics.Raycast(transform.position, -Vector3.up, 1.0f))
        {
            enemy.animator.SetBool("Jump", false);
            enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);

        }
        else
        {
            enemy.animator.SetBool("Jump", true);
            enemy.animator.SetBool("Run", false);
        }*/

        // force target
        if (WaveManager.waveManagerInstance.timerScript.timeLeft < WaveManager.waveManagerInstance.forceTargetTime)
        {
            SetState(EnemyState.movingToTarget);
        }
    }
    protected override void MovingToTargetUpdate()
    {
        enemy.moveTarget = enemy.playerTransform.position;

        enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);

        /*if (Physics.Raycast(transform.position, -Vector3.up, 1.0f))
        {
            enemy.animator.SetBool("Jump", false);
            enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);
        }
        else
        {
            enemy.animator.SetBool("Jump", true);
            enemy.animator.SetBool("Run", false);
        }*/


        if (lastWalkingSoundTime + walkingSoundInterval + Random.Range(0.0f, 2.0f) < Time.time)
        {
            SoundManager2.Instance.PlaySound("RobotWalking", enemy.transform);
            lastWalkingSoundTime = Time.time;
        }

        // if line of sight is lost, change to lost sight state
        if (!neverLoseSight && needsLineOfSight)
        {
            if (!enemy.hasLineOfSight) SetState(EnemyState.lostSightOfTarget);
            else lastSeenPosition = enemy.playerTransform.position; // only update last seen pos if we didnt lose sight this frame
        }

        // if has line of sight and within attack range, switch to attack state
        if (enemy.hasLineOfSight && remainingAttackCooldown <= 0 && Vector3.Distance(transform.position, enemy.playerTransform.position) <= attackRange)
        {
            if (Physics.Raycast(transform.position, -Vector3.up, 1.0f)) SetState(EnemyState.attacking);
        }
    }
    protected override void LostSightOfTargetUpdate() // UNUSED
    {
        enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);

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

    protected override void StunnedExit()
    {
        enemy.animator.SetBool("Stagger", false);
    }
    #endregion
}
