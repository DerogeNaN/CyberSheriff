using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEditor;
using UnityEngine;

enum LaserState
{
    none,
    charging,
    firing,
    disappearing
}

public class EnemySniper : EnemyBase
{
    public float attackRange;
    public float attackCooldown;
    public float attackStartup;
    public float shotDelay;
    public float shotDuration;
    public float attackEndlag;
    public GameObject laser;
    public Transform gunPos;

    LaserState laserState = LaserState.none;
    float timer;
    LineRenderer currentLaser;

    protected override void OnStart()
    {
        timer = attackCooldown;
        enemy.animator.SetBool("Run", false);

        currentLaser = Instantiate(laser, gunPos).GetComponentInChildren<LineRenderer>();
        Debug.Log(currentLaser);
    }

    protected override void OnUpdate()
    {
        Vector3[] positions = { transform.position, enemy.playerTransform.position };
        currentLaser.SetPositions(positions);

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            switch (laserState)
            {
                case LaserState.none:
                    laserState = LaserState.charging;
                    timer = attackStartup;
                    break;

                case LaserState.charging:
                    laserState = LaserState.firing;
                    timer = shotDuration;
                    break;

                case LaserState.firing:
                    laserState = LaserState.disappearing;
                    timer = attackEndlag;
                    break;

                case LaserState.disappearing:
                    laserState = LaserState.none;
                    timer = attackCooldown;
                    break;
            }
        }

        switch (laserState)
        {
            case LaserState.none:
                break;

            case LaserState.charging:
                break;

            case LaserState.firing:
                break;

            case LaserState.disappearing:
                break;
        }
    }
}
