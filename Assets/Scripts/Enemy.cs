using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.AI;

// OUTDATED SCRIPT
// EnemyBase is the new one

public class Enemy : MonoBehaviour
{
    public Transform target;

    public float sightRange = 100.0f;
    [HideInInspector] public bool targetLineOfSight = false;
    [HideInInspector] public bool shouldPath = false;

    NavMeshAgent agent;


    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        // if within range
        if ((target.position - transform.position).magnitude <= sightRange)
        {
            // if has line of sight
            if (Physics.Raycast(transform.position, (target.position - transform.position).normalized, out RaycastHit hit, sightRange))
            {
                targetLineOfSight = !hit.transform.gameObject.CompareTag("Wall");
            }
            else targetLineOfSight = true;
        } else targetLineOfSight = false;

        // debug ray
        if (targetLineOfSight) Debug.DrawRay(transform.position, (target.position - transform.position).normalized * sightRange, new(1.0f, 0.0f, 0.0f));
        else Debug.DrawRay(transform.position, (target.position - transform.position).normalized * sightRange, new(0.0f, 0.0f, 1.0f));

        // set logic for shouldPath in inherited class
        // enable nav agent if should path
        if (shouldPath)
        {
            agent.enabled = true;
            agent.destination = target.position;
        }
        else agent.enabled = false;
    }
}
