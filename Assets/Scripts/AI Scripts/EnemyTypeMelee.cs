using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyTypeMelee : EnemyBase
{
    [Header("Melee Movement Settings")]
    public float runSpeed = 5.0f;
    public float walkSpeed = 2.0f;
    public float chaseTime = 2.0f;

    [Header("Melee Attack Settings")]
    public GameObject hitboxPrefab;
    public float attackRange = 2.0f;
    public float attackTime = 1.0f;
    public float attackCooldown = 1.0f;

    Vector3 initialPosition;
    Vector3 lastSeenPosition;
    float remainingChaseTime = 0;
    float remainingAttackTime = 0;
    float remainingAttackCooldown = 0;
    float remainingStun = 0;

    new void Start()
    {
        base.Start();
        SetState(EnemyState.idle);

        initialPosition = transform.position;
        speed = walkSpeed;
    }

    new void Update()
    {
        base.Update();
        UpdateState();
    }

    public override void Hit(int damage)
    {
        base.Hit(damage);
        SetState(EnemyState.stunned);
        remainingStun = 0.5f;
    }

    // use this to change states in UpdateState
    void SetState(EnemyState state)
    {
        ExitState();
        this.state = state;
        EnterState();
    }

    // runs once when the state is changed
    void EnterState()
    {
        switch (state)
        {
            case EnemyState.idle:
                {
                    speed = walkSpeed;
                    moveTarget = initialPosition;
                }
                break;

            case EnemyState.movingToTarget:
                {
                    speed = runSpeed;
                }
                break;

            case EnemyState.lostSightOfTarget:
                {
                    remainingChaseTime = chaseTime;
                    shouldPath = true;
                    moveTarget = lastSeenPosition;
                }
                break;

            case EnemyState.attacking:
                {
                    speed = walkSpeed;
                    remainingAttackTime = attackTime;

                    if (hitboxPrefab != null)
                    {
                        GameObject hitbox = Instantiate(hitboxPrefab, mesh.transform);
                        hitbox.transform.position = hitbox.transform.position + hitbox.transform.forward * 1.0f;
                    }
                }
                break;

            case EnemyState.stunned:
                {
                    shouldPath = false;
                }
                break;

            case EnemyState.downed:
                {
                    shouldPath = false;
                }
                break;
        }
    }

    // frame by frame update for the current state
    void UpdateState()
    {
        // do this regardless of state 
        lookTarget = playerTransform.position;
        if (remainingAttackCooldown > 0) remainingAttackCooldown -= Time.deltaTime;

        switch (state)
        {
            case EnemyState.idle:
                {
                    // if has line of sight, chase player
                    if (hasLineOfSight)
                    {
                        SetState(EnemyState.movingToTarget);
                    }
                }
                break;

            case EnemyState.movingToTarget:
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
                break;

            case EnemyState.lostSightOfTarget:
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
                break;

            case EnemyState.attacking:
                {
                    remainingAttackTime -= Time.deltaTime;
                    // if attack time ends, go back to idle and set attack cooldown
                    if (remainingAttackTime <= 0)
                    {
                        SetState(EnemyState.movingToTarget);
                    }
                }
                break;

            case EnemyState.stunned:
                {
                    remainingStun -= Time.deltaTime;

                    if (remainingStun <= 0)
                        SetState(EnemyState.idle);
                }
                break;

            case EnemyState.downed:
                {

                }
                break;
        }
    }

    // runs once when the state is changed
    void ExitState()
    {
        switch (state)
        {
            case EnemyState.idle:
                {
                    shouldPath = true;
                    moveTarget = initialPosition;
                }
                break;

            case EnemyState.movingToTarget:
                {
                    
                }
                break;

            case EnemyState.lostSightOfTarget:
                {
                    
                }
                break;

            case EnemyState.attacking:
                {
                    remainingAttackCooldown = attackCooldown;
                }
                break;

            case EnemyState.stunned:
                {

                }
                break;

            case EnemyState.downed:
                {

                }
                break;
        }
    }
}
