using System.Collections;
using System.Collections.Generic;
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
    public Transform moveTarget;
    public float sightRange = 25.0f;

    [HideInInspector] public EnemyState state;
    protected bool hasLineOfSight;
    protected bool shouldPath;
    protected NavMeshAgent pathAgent;

    [SerializeField] TMP_Text debugStateText;

    public void Start()
    {
        pathAgent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if (debugStateText != null) debugStateText.text = state.ToString();

        // if the target is out of range, don't raycast
        if ((moveTarget.position - transform.position).magnitude <= sightRange)
        {
            // check for line of sight with target
            if (Physics.Raycast(transform.position, (moveTarget.position - transform.position).normalized, out RaycastHit hit, sightRange))
            {
                // colliders tagged as "Wall" will block the line of sight
                hasLineOfSight = !hit.transform.gameObject.CompareTag("Wall");
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
    }

    public virtual void Hit(int damage)
    {
        // probably add more parameters to this, like what type of hit
        health -= damage;

        Debug.Log("hit " + name);
    }
}
