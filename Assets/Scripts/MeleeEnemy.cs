using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

enum State
{
    idle,
    chasing,
    lostSight,
    attacking
}

public class MeleeEnemy : Enemy
{
    public float playerChaseTime;
    public float attackTime;
    [SerializeField] State state;
    [SerializeField] TMP_Text debugStateText;

    float remainingChaseTime;
    float remainingAttackTime;

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
                    // if the player is withing range, start an attack
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
        }

        // moves when it has clear line of sight to the target
    }
}
