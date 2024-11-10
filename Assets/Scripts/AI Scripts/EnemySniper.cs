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
    public bool resetOnLoseSight;
    public float attackRange;
    public float chargeTime;
    public float timeBeforeShot;
    public float shotDuration;
    public float disappearTime;
    public float shotCooldown;
    public GameObject laser;
    public Transform gunPos;

    LaserState laserState = LaserState.none;
    float timer;
    LineRenderer currentLaser;
    float laserIntensity = 0.0f;
    float laserDistance;

    protected override void OnStart()
    {
        timer = shotCooldown;
        enemy.animator.SetBool("Run", false);

        currentLaser = Instantiate(laser, gunPos).GetComponentInChildren<LineRenderer>();
        currentLaser.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);
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
                    timer = chargeTime;
                    laserIntensity = 0.0f;
                    break;

                case LaserState.charging:
                    laserState = LaserState.firing;
                    timer = shotDuration;
                    laserIntensity = 1.0f;
                    break;

                case LaserState.firing:
                    laserState = LaserState.disappearing;
                    timer = disappearTime;
                    laserIntensity = 1.0f;
                    break;

                case LaserState.disappearing:
                    laserState = LaserState.none;
                    timer = shotCooldown;
                    laserIntensity = 0.0f;
                    break;
            }
        }

        switch (laserState)
        {
            case LaserState.none:
                break;

            case LaserState.charging:
                if (laserIntensity < 0.5f) laserIntensity += Time.deltaTime * 0.2f;
                break;

            case LaserState.firing:
                // count down delay before actual shot then fire
                Fire();
                break;

            case LaserState.disappearing:
                if (laserIntensity > 0.0f) laserIntensity -= Time.deltaTime * 2.0f;
                break;
        }

        currentLaser.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, laserIntensity);
    }

    void UpdateLaser()
    {
        RaycastHit hit;
        Vector3[] positions = new Vector3[2];

        if (Physics.Raycast(gunPos.position, (enemy.playerTransform.position - gunPos.position).normalized, out hit, 1000.0f, LayerMask.GetMask("Wall") | LayerMask.GetMask("Player")))
        {
            if (hit.collider.CompareTag("Player"))
            {
                positions[0] = gunPos.position;
                positions[1] = enemy.playerTransform.position;
            }
            else
            {
                positions[0] = gunPos.position;
                positions[1] = hit.point;
            }
        }
        else
        {
            // shouldn't happen?
            positions[0] = gunPos.position;
            positions[1] = enemy.playerTransform.position;
        }

        currentLaser.SetPositions(positions);
    }

    void Fire()
    {
        RaycastHit hit;

        // double check that the player is within sight
        if (Physics.Raycast(gunPos.position, (enemy.playerTransform.position - gunPos.position).normalized, out hit, 1000.0f, LayerMask.GetMask("Wall") | LayerMask.GetMask("Player")))
        {
            if (hit.collider.CompareTag("Player"))
            {
                enemy.playerTransform.gameObject.GetComponent<PlayerHealth>().TakeDamage(25, 0);
            }
        }
    }
}
