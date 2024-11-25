using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEditor;
using UnityEngine;

enum LaserState
{
    none,
    charging,
    beforeFire,
    firing,
    disappearing
}

public class EnemySniper : EnemyBase
{
    [Tooltip("whether or not the charging progress resets when line of sight is broken")]
    public bool resetOnLoseSight;
    [Tooltip("time for the beam to charge up while aiming at the player, before the attack fires")]
    public float chargeTime;
    [Tooltip("the moment of time that the enemy stops tracking the player before firing, allowing them to dodge the beam before it fires")]
    public float timeBeforeShot;
    [Tooltip("duration of the firing animation. the actual damaging shot still only happens once")]
    public float shotDuration;
    [Tooltip("time for the animation of the beam dissipating after firing")]
    public float disappearTime;
    [Tooltip("time between shots")]
    public float shotCooldown;
    [Tooltip("the laser LineRenderer prefab")]
    public GameObject laser;
    [Tooltip("the transform at which to fire the laser from")]
    public Transform gunPos;

    LaserState laserState = LaserState.none;
    float attackRange;
    float timer;
    //LineRenderer currentLaser;
    LineRenderer[] lasers;
    float laserIntensity = 0.0f;
    bool fired = false;
    bool targetPlayer = true;

    protected override void OnStart()
    {
        attackRange = enemy.sightRange;
        timer = shotCooldown;
        enemy.animator.SetBool("Run", false);

        lasers = laser.GetComponentsInChildren<LineRenderer>();

        foreach (var l in lasers)
            l.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);

        //currentLaser = Instantiate(laser, gunPos).GetComponentInChildren<LineRenderer>();
        //currentLaser.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);
    }

    protected override void OnUpdate()
    {
        if (state == EnemyState.downed) return;

        if (targetPlayer)
        {
            // target player
            enemy.lookTarget = enemy.playerTransform.position;
            UpdateLaser();

            // face target
            transform.LookAt(enemy.playerTransform);
            Vector3 rot = transform.rotation.eulerAngles;
            rot.x = 0;
            transform.rotation = Quaternion.Euler(rot);

            enemy.animator.SetFloat("Aim", enemy.playerTransform.position.y - gunPos.position.y);
        }


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
                    fired = false;
                    targetPlayer = false;
                    timer = shotDuration + timeBeforeShot;
                    break;

                case LaserState.firing:
                    laserState = LaserState.disappearing;
                    timer = disappearTime;
                    targetPlayer = false;
                    break;

                case LaserState.disappearing:
                    laserState = LaserState.none;
                    timer = shotCooldown;
                    laserIntensity = 0.0f;
                    targetPlayer = true;
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
                if (!fired && timer < shotDuration)
                {
                    Fire();
                    laserIntensity = 1.0f;
                    fired = true;
                }
                break;

            case LaserState.disappearing:
                if (laserIntensity > 0.0f) laserIntensity -= Time.deltaTime * 2.0f;
                break;
        }

        foreach (var l in lasers)
            l.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, laserIntensity);
    }

    void UpdateLaser()
    {
        RaycastHit hit;
        Vector3[] positions = new Vector3[2];

        if (Physics.Raycast(gunPos.position, (enemy.lookTarget - gunPos.position).normalized, out hit, 1000.0f, ~LayerMask.GetMask(new[] { "Enemy" })))
        {
            if (hit.collider.CompareTag("Player"))
            {
                positions[0] = gunPos.position;
                positions[1] = enemy.lookTarget;
            }
            else
            {
                positions[0] = gunPos.position;
                positions[1] = hit.point;
                if (resetOnLoseSight)
                {
                    // reset progress
                    if (laserState == LaserState.charging) timer = 0.0f; // if was already charging, start again immedietly

                    laserState = LaserState.none;
                    laserIntensity = 0.0f;
                }
            }
        }
        else
        {
            // shouldn't happen?
            positions[0] = gunPos.position;
            positions[1] = enemy.lookTarget;
        }

        // get the direction of the beam, then extend it in that direction
        positions[1] += (positions[1] - positions[0]).normalized * 100.0f;

        foreach (var l in lasers)
            l.SetPositions(positions);
    }

    void Fire()
    {
        RaycastHit hit;

        // double check that the player is within sight
        if (Physics.Raycast(gunPos.position, (enemy.lookTarget - gunPos.position).normalized, out hit, 1000.0f, ~LayerMask.GetMask(new[] { "Enemy" })))
        {
            if (hit.collider.CompareTag("Player"))
            {
                enemy.playerTransform.gameObject.GetComponent<PlayerHealth>().TakeDamage(25, 0);
            }
        }
    }

    public override void OnDestroyed(int damageType)
    {
        SetState(EnemyState.downed);
        enemy.animator.SetTrigger("Death");
    }
}
