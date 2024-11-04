using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// this is the base enemy state machine
// this should not be put on gameobjects, use one of the inheriting classes

// TODO: DISABLE COLLIDERS WHEN NOT SPAWNED

public enum EnemyState
{
    idle,
    movingToTarget,
    lostSightOfTarget,
    attacking,
    stunned,
    downed,
    chargeAttack
}

public class EnemyBase : MonoBehaviour
{
    [HideInInspector] public EnemyState state;

    protected EnemyCommon enemy;

    private void Start()
    {
        enemy = GetComponent<EnemyCommon>();
        OnStart();
    }

    private void Update()
    {
        UpdateState();
        OnUpdate();
    }

    virtual protected void OnStart() { }
    virtual protected void OnUpdate() { }
    virtual public void OnHit(int damage, int damageType) { }
    virtual public void OnDestroyed(int damageType) { }

    public void SetState(EnemyState state)
    {
        ExitState();
        this.state = state;
        EnterState();
    }

    private void EnterState()
    {
        switch (state)
        {
            case EnemyState.idle:
                IdleEnter();
                break;
            case EnemyState.movingToTarget:
                MovingToTargetEnter();
                break;
            case EnemyState.lostSightOfTarget:
                LostSightOfTargetEnter();
                break;
            case EnemyState.attacking:
                AttackingEnter();
                break;
            case EnemyState.stunned:
                StunnedEnter();
                break;
            case EnemyState.chargeAttack:
                ChargeAttackEnter();
                break;
            case EnemyState.downed:
                DownedEnter();
                break;
        }
    }

    private void UpdateState()
    {
        switch (state)
        {
            case EnemyState.idle:
                IdleUpdate();
                break;
            case EnemyState.movingToTarget:
                MovingToTargetUpdate();
                break;
            case EnemyState.lostSightOfTarget:
                LostSightOfTargetUpdate();
                break;
            case EnemyState.attacking:
                AttackingUpdate();
                break;
            case EnemyState.stunned:
                StunnedUpdate();
                break;
            case EnemyState.chargeAttack:
                ChargeAttackUpdate();
                break;
            case EnemyState.downed:
                DownedUpdate();
                break;
        }
    }

    private void ExitState()
    {
        switch (state)
        {
            case EnemyState.idle:
                IdleExit();
                break;
            case EnemyState.movingToTarget:
                MovingToTargetExit();
                break;
            case EnemyState.lostSightOfTarget:
                LostSightOfTargetExit();
                break;
            case EnemyState.attacking:
                AttackingExit();
                break;
            case EnemyState.stunned:
                StunnedExit();
                break;
            case EnemyState.downed:
                DownedExit();
                break;
        }
    }

    virtual protected void IdleEnter() { }
    virtual protected void IdleUpdate() { }
    virtual protected void IdleExit() { }

    virtual protected void MovingToTargetEnter() { }
    virtual protected void MovingToTargetUpdate() { }
    virtual protected void MovingToTargetExit() { }

    virtual protected void LostSightOfTargetEnter() { }
    virtual protected void LostSightOfTargetUpdate() { }
    virtual protected void LostSightOfTargetExit() { }

    virtual protected void AttackingEnter() { }
    virtual protected void AttackingUpdate() { }
    virtual protected void AttackingExit() { }

    virtual protected void StunnedEnter() { }
    virtual protected void StunnedUpdate() { }
    virtual protected void StunnedExit() { }

    virtual protected void ChargeAttackEnter() { }
    virtual protected void ChargeAttackUpdate() { }
    virtual protected void ChargeAttackExit() { }

    virtual protected void DownedEnter() { }
    virtual protected void DownedUpdate() { }
    virtual protected void DownedExit() { }
}