using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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
    protected Vector3 moveTarget; // the object it follows
    protected Vector3 lookTarget; // the object to check line of sight with (usually will be the same as moveTarget, but doesn't have to be)
    public float sightRange = 25.0f;

    [HideInInspector] public EnemyState state;
    protected bool hasLineOfSight;
    protected bool shouldPath;
    protected NavMeshAgent pathAgent;

    [SerializeField] TMP_Text debugStateText;

    public void Start()
    {
        pathAgent = GetComponent<NavMeshAgent>();
        pathAgent.avoidancePriority = (int)Random.Range(0, 99.0f);
    }

    public void Update()
    {
        if (debugStateText != null) debugStateText.text = state.ToString();

        // if the target is out of range, don't raycast
        if ((lookTarget - transform.position).magnitude <= sightRange)
        {
            // check for line of sight with target
            if (Physics.Raycast(transform.position, (lookTarget - transform.position).normalized, out RaycastHit hit, sightRange))
            {
                // colliders tagged as "Wall" will block the line of sight
                hasLineOfSight = !hit.transform.gameObject.CompareTag("Wall");
            }
            else hasLineOfSight = true;
        }
        else hasLineOfSight = false;

        // draw ray for debugging
        if (hasLineOfSight) Debug.DrawRay(transform.position, (lookTarget - transform.position).normalized * sightRange, new(1.0f, 0.0f, 0.0f));
        else Debug.DrawRay(transform.position, (lookTarget - transform.position).normalized * sightRange, new(0.0f, 0.0f, 1.0f));

        Debug.DrawRay(transform.position, (moveTarget - transform.position).normalized * sightRange, new(0.5f, 0.0f, 0.5f));

        // enemy types that inherit from this decide when to set shouldPath to true or false
        if (shouldPath)
        {
            pathAgent.enabled = true;
            pathAgent.destination = moveTarget;
        }
        else pathAgent.enabled = false;
    }

    public virtual void Hit(int damage)
    {
        // probably add more parameters to this, like what type of hit
        health -= damage;

        Debug.Log("hit " + name);
    }
}
