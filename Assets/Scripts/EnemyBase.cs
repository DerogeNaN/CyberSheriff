using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

// NEW BASE ENEMY SCRIPT

public enum EnemyState
{
    idle,
    movingToTarget,
    lostSightOfTarget,
    attacking,
}

public class EnemyBase : MonoBehaviour
{
    public int health = 100;
    public Transform moveTarget;
    public float sightRange = 25.0f;
    [SerializeField] EnemyState state;

    [HideInInspector] public bool hasLineOfSight;
    [HideInInspector] public bool shouldPath;
    NavMeshAgent pathAgent;

    public void Start()
    {
        pathAgent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        // if the target is out of range, don't raycast
        if ((moveTarget.position - transform.position).magnitude <= sightRange)
        {
            // check for line of sight with target
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (moveTarget.position - transform.position).normalized, out hit, sightRange))
            {
                // colliders tagged as "Wall" will block the line of sight
                hasLineOfSight = hit.transform.gameObject.tag != "Wall";
            }
            else hasLineOfSight = true;
        }
        else hasLineOfSight = false;

        // draw ray for debugging
        if (hasLineOfSight) Debug.DrawRay(transform.position, (moveTarget.position - transform.position).normalized * sightRange, new(1.0f, 0.0f, 0.0f));
        else Debug.DrawRay(transform.position, (moveTarget.position - transform.position).normalized * sightRange, new(0.0f, 0.0f, 1.0f));

        // enemy types that inherit from this decide when to set shouldPath to true or false
        if (shouldPath)
        {
            pathAgent.enabled = true;
            pathAgent.destination = moveTarget.position;
        }
        else pathAgent.enabled = false;

        // call the method for whichever state we're in
        // each state's mathod can be overriden by the inheriting class
        UpdateState(state);
    }

    public virtual void Hit(int damage)
    {
        // probably add more parameters to this, like what type of hit
        health -= damage;

        Debug.Log("hit " + name);
    }

    public void SetState(EnemyState newState)
    {
        // change the current state
        // this calles the method for entering whichever state we're changing to,
        // which can be overriden by the inheriting class
    }

    void UpdateState(EnemyState state)
    {
        // call the method for whichever state we're in
        // each state's mathod can be overriden by the inheriting class
    }


}
