using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

enum State
{
    idle,
    chasing,
    lostSight,
    attacking,
    stunned
}

public class MeleeEnemy : Enemy
{
    public float attackRange;
    public float playerChaseTime;
    public float attackTime;
    public float attackCooldown;
    [SerializeField] State state;
    [SerializeField] float stun = 0;
    [SerializeField] TMP_Text debugStateText;

    float remainingChaseTime = 0;
    float remainingAttackTime = 0;
    float remainingAttackCooldown = 0;

    private new void Start()
    {
        base.Start();

        state = State.idle;
    }

    private new void Update()
    {
        base.Update();
        debugStateText.text = state.ToString();

        switch (state)
        {
            case State.idle:
                {
                    // when this has light of sight with the player, start chasing
                    if (targetLineOfSight)
                    {
                        state = State.chasing;
                        shouldPath = true;
                    }
                }
                break;

            case State.chasing:
                {
                    // when lost line of sight, keep following the player for a moment
                    if (!targetLineOfSight)
                    {
                        state = State.lostSight;
                        remainingChaseTime = playerChaseTime;
                    }
                    // if the player is withing range and attack cooldown is up, start an attack
                    if (Vector3.Distance(transform.position, target.position) <= attackRange && remainingAttackCooldown <= 0)
                    {
                        state = State.attacking;
                        remainingAttackTime = attackTime;
                        shouldPath = false;
                        remainingAttackCooldown = attackCooldown;
                    }
                    remainingAttackCooldown -= Time.deltaTime;
                }
                break;

            case State.lostSight:
                {
                    // if line of sight is regained, start chasing the player again
                    if (targetLineOfSight)
                    {
                        state = State.chasing;
                    }
                    // if the timer runs out with still no line of sight. go back to idling
                    if (remainingChaseTime <= 0)
                    {
                        state = State.idle;
                        shouldPath = false;
                    }
                    remainingChaseTime -= Time.deltaTime;
                }
                break;

            case State.attacking:
                {
                    // when the attack timer ends, go back to idling
                    if (remainingAttackTime <= 0)
                    {
                        state = State.idle;
                    }
                    remainingAttackTime -= Time.deltaTime;
                }
                break;

            case State.stunned:
                {
                    // if the stun timer is up, return to idling
                    if (stun <= 0)
                    {
                        state = State.idle;
                    }
                    stun -= Time.deltaTime;
                }
                break;
        }
    }

    public void Stun(int stunTime)
    {
        state = State.stunned;
        stun = stunTime;
        shouldPath = false;
    }
}
