using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class EnemyTypeMelee : EnemyBase
{
    public float chaseTime = 2.0f;
    float remainingChaseTime = 0;

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

    // use this tp change states in UpdateState
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

                }
                break;

            case EnemyState.movingToTarget:
                {
                    shouldPath = true;
                }
                break;

            case EnemyState.lostSightOfTarget:
                {
                    remainingChaseTime = chaseTime;
                }
                break;
        }
    }

    // frame by frame update for the current method
    void UpdateState()
    {
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
                    // if lost sight of the player, switch to the lost sight state
                    if (!hasLineOfSight) SetState(EnemyState.lostSightOfTarget);
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
        }
    }
}
