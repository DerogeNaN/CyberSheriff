using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    public Transform target;

    public float sightRange = 100.0f;
    [HideInInspector] public bool targetLineOfSight = false;
    [HideInInspector] public bool playerLineOfSight = false;
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
            RaycastHit hit;
            Debug.DrawRay(transform.position, (target.position - transform.position).normalized * sightRange, new(1.0f, 0.0f, 0.0f));
            if (Physics.Raycast(transform.position, (target.position - transform.position).normalized, out hit, sightRange))
            {
                targetLineOfSight = hit.transform.gameObject.tag != "Wall";
            }
            else targetLineOfSight = true;
        } else targetLineOfSight = false;

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
