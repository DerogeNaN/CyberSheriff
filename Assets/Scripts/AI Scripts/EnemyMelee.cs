using TMPro;
using UnityEngine;

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

    [SerializeField] TMP_Text debugStunText;
    Vector3 initialPosition;
    Vector3 lastSeenPosition;
    float remainingChaseTime = 0;
    float remainingAttackTime = 0;
    float remainingAttackCooldown = 0;
    float stun = 0;
    Vector3 attackStartRotation;
    Vector3 attackTargetRotation;
    float attackRotate = 0;

    protected override void OnStart()
    {
        SetState(EnemyState.idle);
        initialPosition = transform.position;
        enemy.moveTarget = initialPosition;
        enemy.speed = walkSpeed;

        if (debugStunText) debugStunText.text = "";
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
        remainingAttackTime = attackTime;

        if (attackPrefab != null)
        {
            GameObject hitbox = Instantiate(attackPrefab, enemy.mesh.transform);
            hitbox.transform.position = hitbox.transform.position + hitbox.transform.forward * 1.0f;
        }

        // look at player
        Vector3 dir = (enemy.playerTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new(dir.x, 0, dir.z));

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
        if (!enemy.hasLineOfSight) SetState(EnemyState.lostSightOfTarget);
        else lastSeenPosition = enemy.playerTransform.position; // only update last seen pos if we didnt lose sight this frame

        // if has line of sight and within attack range, switch to attack state
        if (enemy.hasLineOfSight && remainingAttackCooldown <= 0 && Vector3.Distance(transform.position, enemy.playerTransform.position) <= attackRange)
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
        if (enemy.hasLineOfSight)
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
            SetState(EnemyState.idle);
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
    }
    #endregion
}
