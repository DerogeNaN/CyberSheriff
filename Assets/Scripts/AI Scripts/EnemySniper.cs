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
        // face player
        transform.LookAt(enemy.playerTransform);
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x = 0;
        transform.rotation = Quaternion.Euler(rot);

        enemy.animator.SetFloat("Aim", (enemy.playerTransform.position.y - gunPos.position.y));

        UpdateLaser();

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

    void UpdateLaser()
    {
        RaycastHit hit;
        Vector3[] positions = new Vector3[2];

        if (Physics.Raycast(gunPos.position, (enemy.playerTransform.position - gunPos.position).normalized, out hit))
        {
            positions[0] = gunPos.position;
            positions[1] = hit.point;
        }
        else
        {
            positions[0] = gunPos.position;
            positions[1] = enemy.playerTransform.position;
        }

        currentLaser.SetPositions(positions);
    }
}
