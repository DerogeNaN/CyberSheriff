using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

// this is a class containing common functions used my enemies, so include this on enemy GameObjects along with a class the inherits from EnemyBase

public class EnemyCommon : MonoBehaviour
{
    [Header("Basic Settings")]
    public bool active;

    [Header("Detection Settings")]
    public float sightRange = 25.0f;

    [Header("General Movement Settings")]
    [HideInInspector] public float speed = 5.0f;

    [Header("Advanced")]
    public GameObject mesh;
    public Vector3 lineOfSightOffset;
    public Vector3 floorRaycastPos;
    public float floorRaycastLength;

    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public Vector3 moveTarget; // the object it follows
    [HideInInspector] public Vector3 lookTarget; // the object to check line of sight with (usually will be the same as moveTarget, but doesn't have to be)
    public bool hasLineOfSight;
    [HideInInspector] public bool shouldPath;

    private NavMeshAgent navAgent;

    private void Start()
    {
        // initialise pathing values
        shouldPath = false;
        TryGetComponent<NavMeshAgent>(out navAgent);

        // start spawned or despawned
        if (active) Spawn();
        else Despawn();

        SetPlayerTransform();
    }

    private void Update()
    {
        if (!active) return;

        UpdateRaycast();
        UpdateNavAgent();
    }

    public void OnDrawGizmos()
    {
        if (navAgent)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(moveTarget, 0.5f);
        }
    }

    private void UpdateRaycast()
    {
        // raycasting
        Vector3 raycastPos = transform.position + lineOfSightOffset;

        if ((lookTarget - raycastPos).magnitude <= sightRange)
        {
            // check for line of sight with target
            if (Physics.Raycast(raycastPos, (lookTarget - raycastPos).normalized, out RaycastHit hit, sightRange))
            {
                // colliders tagged as "Wall" will block the line of sight
                hasLineOfSight = !hit.transform.gameObject.CompareTag("Wall");
            }
            else hasLineOfSight = true;
        }
        else hasLineOfSight = false;


        // draw ray for debugging
        //if (hasLineOfSight) Debug.DrawRay(raycastPos, (lookTarget - raycastPos).normalized * sightRange, new(1.0f, 0.0f, 0.0f));
        //else Debug.DrawRay(raycastPos, (lookTarget - raycastPos).normalized * sightRange, new(0.0f, 0.0f, 1.0f));
        //Debug.DrawRay(raycastPos, (moveTarget - raycastPos).normalized * sightRange, new(0.5f, 0.0f, 0.5f));
    }

    private void UpdateNavAgent()
    {
        if (navAgent != null)
        {
            // enemy types that inherit from this decide when to set shouldPath to true or false
            if (shouldPath)
            {
                navAgent.enabled = true;
                navAgent.speed = speed;
                navAgent.SetDestination(moveTarget);
            }
            else navAgent.enabled = false;
        }
    }

    public void Spawn()
    {
        active = true;
        mesh.SetActive(true);
    }

    public void Despawn()
    {
        active = false;
        mesh.SetActive(false);
    }

    void SetPlayerTransform()
    {
        // get the transform of whatever has the main camera
        playerTransform = Camera.main.transform.parent;
    }
}

