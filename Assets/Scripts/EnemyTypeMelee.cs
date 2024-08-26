using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;
using static UnityEngine.GraphicsBuffer;

public class EnemyTypeMelee : EnemyBase
{
    public float chaseTime = 2.0f;
    public float attackTime = 1.0f;
    public float attackCooldown = 1.0f;
    public float attackRange = 2.0f;
    public GameObject hitboxPrefab;
    public Transform playerTransform;

    Vector3 initialPosition;
    Vector3 lastSeenPosition;
    float remainingChaseTime = 0;
    float remainingAttackTime = 0;
    float remainingAttackCooldown = 0;

    new void Start()
    {
        base.Start();
        initialPosition = transform.position;
        SetState(EnemyState.idle);
    }

    new void Update()
    {
        base.Update();
        UpdateState();
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
                    shouldPath = true;
                    moveTarget = initialPosition;
                }
                break;

            case EnemyState.movingToTarget:
                {
                    shouldPath = true;
                }
                break;

            case EnemyState.lostSightOfTarget:
                {
                    lastSeenPosition = playerTransform.position;
                    moveTarget = lastSeenPosition;
                    remainingChaseTime = chaseTime;
                }
                break;

            case EnemyState.attacking:
                {
                    shouldPath = false;
                    remainingAttackTime = attackTime;

                    if (hitboxPrefab != null)
                    {
                        GameObject hitbox = Instantiate(hitboxPrefab, transform);
                        hitbox.transform.position = hitbox.transform.position + hitbox.transform.forward * 1.0f;
                    }
                }
                break;
        }
    }

    // frame by frame update for the current method
    void UpdateState()
    {
        // do this regardless of state 
        lookTarget = playerTransform.position;
        if (remainingAttackCooldown > 0) remainingAttackCooldown -= Time.deltaTime;

        switch (state)
        {
            case EnemyState.idle:
                {
                    // if the player gets withing range and line of sight, switch to chasing them
                    if (hasLineOfSight) SetState(EnemyState.movingToTarget);
                }
                break;

            case EnemyState.movingToTarget:
                {
                    moveTarget = playerTransform.position;

                    // if lost sight of the player, switch to the lost sight state
                    if (!hasLineOfSight) SetState(EnemyState.lostSightOfTarget);

                    // if within range and not on cooldown, attack
                    if (Vector3.Distance(transform.position, moveTarget) <= attackRange && remainingAttackCooldown <= 0)
                    {
                        SetState(EnemyState.attacking);
                    }
                }
                break;

            case EnemyState.lostSightOfTarget:
                {
                    // keep following the player until the timer runs out, then go back to idle
                    remainingChaseTime -= Time.deltaTime;
                    if (remainingChaseTime <= 0)
                    {
                        SetState(EnemyState.idle);
                    }
                }
                break;

            case EnemyState.attacking:
                {
                    // do attack stuff, then switch back to chasing
                    remainingAttackTime -= Time.deltaTime;
                    if (remainingAttackTime <= 0)
                    {
                        SetState(EnemyState.movingToTarget);
                    }
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

                }
                break;

            case EnemyState.movingToTarget:
                {

                }
                break;

            case EnemyState.lostSightOfTarget:
                {
                    shouldPath = false;
                }
                break;

            case EnemyState.attacking:
                {
                    remainingAttackCooldown = attackCooldown;
                }
                break;
        }
    }
}
