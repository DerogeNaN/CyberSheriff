using System.Collections.Generic;
using UnityEngine;

public class Sniper : EnemyBase
{
    public float moveSpeed;

    public float chargeTime;
    public float timeBeforeDamage;
    public float cooldown;

    public List<LineRenderer> lineRenderers;
    LaserState laserState = LaserState.none;
    float laserIntensity = 0;
    float targetIntensity = 0;
    public Transform gunPos;

    float fireDuration = 0.2f;
    float endDuration;

    public Transform targetPos;
    bool reachedTargetPos = false;
    bool trackPlayer = false;

    float timer = 0;
    public GameObject ragdoll;

    public override void OnHit(int damage, int damageType)
    {
        SetState(EnemyState.stunned);
        enemy.CreateHitEffect();
        SoundManager2.Instance.PlaySound("RobotHit", transform);
    }

    public override void OnDestroyed(int damage, int damageType)
    {
        EnemyRagdoll rd = Instantiate(ragdoll, transform.position, transform.rotation).GetComponent<EnemyRagdoll>();
        rd.ApplyForce((transform.position - enemy.playerTransform.position).normalized, damage > 50 ? 300.0f : 50.0f);
        SoundManager2.Instance.PlaySound("RobotDeath", transform);
        Destroy(gameObject);
    }

    protected override void OnStart()
    {
        enemy.shouldPath = false;
        enemy.speed = moveSpeed;
        SoundManager2.Instance.PlaySound("RobotSpawn", transform);
    }

    protected override void OnUpdate()
    {
        enemy.lookTarget = enemy.playerTransform.position;
        enemy.animator.SetBool("Run", enemy.navAgent.velocity.magnitude > 0.1f);

        if (targetPos && !reachedTargetPos) // if hasnt moved to sniper pos yet
        {
            if (enemy.hasLineOfSight)
            {
                // start heading to target pos
                enemy.shouldPath = true;
                if (targetPos) enemy.moveTarget = targetPos.position;
                else enemy.moveTarget = transform.position;
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

            laserIntensity += (targetIntensity - laserIntensity) * 1.5f * Time.deltaTime;

            // update laser intensity
            foreach (LineRenderer line in lineRenderers)
            {
                line.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, laserIntensity);
            }

            if (trackPlayer)
            {
                UpdateLaserPos();

                // look at player
                transform.LookAt(enemy.playerTransform);
                Vector3 rot = transform.rotation.eulerAngles;
                rot.x = 0;
                transform.rotation = Quaternion.Euler(rot);

                enemy.animator.SetFloat("Aim", enemy.playerTransform.position.y - gunPos.position.y);
            }

            // when timer runs out, switch states
            if (timer <= 0)
            {
                switch (laserState)
                {
                    case LaserState.none:
                        laserState = LaserState.charging;
                        timer = chargeTime;
                        targetIntensity = 0.1f;
                        trackPlayer = true;
                        break;

                    case LaserState.charging:
                        laserState = LaserState.beforeFire;
                        SoundManager2.Instance.PlaySound("SniperLaserCharge", transform);
                        timer = timeBeforeDamage;
                        targetIntensity = 0.1f;
                        trackPlayer = false;
                        break;

                    case LaserState.beforeFire:
                        laserState = LaserState.firing;
                        timer = fireDuration;
                        targetIntensity = 0.5f;
                        laserIntensity = 0.5f; // set laser intensity instantly
                        trackPlayer = false;
                        Fire();
                        break;

                    case LaserState.firing:
                        laserState = LaserState.none;
                        timer = cooldown;
                        targetIntensity = 0.0f;
                        trackPlayer = false;
                        break;
                }
            }
        }
        else // no line of sight
        {
            targetIntensity = 0.0f;
            timer = 0;
            laserState = LaserState.none;

            if (laserIntensity > 0) laserIntensity -= 0.5f * Time.deltaTime;

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

        SoundManager2.Instance.PlaySound("SniperLaserShot", transform);

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