using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyTypeRanged : EnemyBase
{
    public float attackTime = 2.0f;
    public float attackCooldown = 2.0f;
    public float attackRange = 25.0f; // max shooting range
    public float minRange = 10.0f; // what range it will try to shoot at
    public float moveSpeed = 5.0f;
    public float slowSpeed = 2.0f;

    [SerializeField] Transform playerTransform;
    [SerializeField] GameObject bulletPrefab;
    float remainingAttackTime;
    float remainingAttackCooldown;

    new void Start()
    {
        base.Start();
        SetState(EnemyState.idle);
    }

    new void Update()
    {
        base.Update();
        UpdateState();
    }

    void SetState(EnemyState state)
    {
        ExitState();
        this.state = state;
        EnterState();
    }

    void EnterState()
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

            case EnemyState.attacking:
                {
                    shouldPath = false;
                    remainingAttackTime = attackTime;

                    // do attack here
                    Projectile projectile = Instantiate(bulletPrefab, transform.position, transform.rotation).GetComponent<Projectile>();
                    projectile.direction = Vector3.Normalize(playerTransform.position - transform.position);
                }
                break;
        }
    }

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

                    // if lost sight of the player, go back to idle
                    if (!hasLineOfSight) SetState(EnemyState.idle);

                    // i'll restructure this later
                    if (Vector3.Distance(transform.position, moveTarget) >= attackRange)
                    {
                        shouldPath = true;
                        pathAgent.speed = moveSpeed;
                    }
                    else
                    {
                        shouldPath = false;

                        if (remainingAttackCooldown <= 0)
                        {
                            SetState(EnemyState.attacking);
                        }
                        else if (Vector3.Distance(transform.position, moveTarget) >= minRange)
                        {
                            shouldPath = true;
                            pathAgent.speed = slowSpeed;
                        }
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
