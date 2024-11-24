using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Sniper : EnemyBase
{
    public float moveSpeed;

    public float chargeTime;
    public float timeBeforeDamage;
    public float cooldown;

    public List<LineRenderer> lineRenderers;
    LaserState laserState = LaserState.none;
    float laserIntensity = 0;
    public Transform gunPos;

    float fireDuration = 0.2f;
    float endDuration;

    public Transform targetPos;
    bool reachedTargetPos = false;
    bool trackPlayer = false;

    float timer = 0;

    protected override void OnStart()
    {
        enemy.shouldPath = false;
        enemy.speed = moveSpeed;
    }

    protected override void OnUpdate()
    {
        enemy.lookTarget = enemy.playerTransform.position;
        enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);

        if (!reachedTargetPos) // if hasnt moved to sniper pos yet
        {
            if (enemy.hasLineOfSight)
            {
                // start heading to target pos
                enemy.shouldPath = true;
                enemy.moveTarget = targetPos.position;
            }
            
            if ((targetPos.position - transform.position).magnitude < 1.0f)
            {
                // reached target pos
                reachedTargetPos = true;
                enemy.shouldPath = false;
            }
        }
        else if (enemy.hasLineOfSight) // if has reached sniper pos
        {
            timer -= Time.deltaTime;

            // update laser intensity
            foreach (LineRenderer line in lineRenderers)
            {
                line.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, laserIntensity);
            }

            if (trackPlayer) UpdateLaserPos();

            // when timer runs out, switch states
            if (timer <= 0)
            {
                switch (laserState)
                {
                    case LaserState.none:
                        laserState = LaserState.charging;
                        timer = chargeTime;
                        laserIntensity = 0.2f;
                        trackPlayer = true;
                        break;

                    case LaserState.charging:
                        laserState = LaserState.beforeFire;
                        timer = timeBeforeDamage;
                        laserIntensity = 0.2f;
                        trackPlayer = false;
                        break;

                    case LaserState.beforeFire:
                        laserState = LaserState.firing;
                        timer = fireDuration;
                        laserIntensity = 1.0f;
                        trackPlayer = false;
                        Fire();
                        break;

                    case LaserState.firing:
                        laserState = LaserState.none;
                        timer = cooldown;
                        laserIntensity = 0.0f;
                        trackPlayer = false;
                        break;
                }
            }
        }
        else // no line of sight
        {
            laserIntensity = 0.0f;
            timer = 0;
            laserState = LaserState.none;
            
            // update laser intensity
            foreach (LineRenderer line in lineRenderers)
            {
                line.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, laserIntensity);
            }
        }
    }

    void UpdateLaserPos()
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
                // reset progress
                if (laserState == LaserState.charging) timer = 0.0f; // if was already charging, start again immedietly

                laserState = LaserState.none;
                laserIntensity = 0.0f;
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

        foreach (var l in lineRenderers)
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
}