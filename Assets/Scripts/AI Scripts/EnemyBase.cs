using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// BASE ENEMY SCRIPT

public enum EnemyState
{
    idle,
    movingToTarget,
    lostSightOfTarget,
    attacking,
    stunned,
    downed,
}

// this should not be put on gameobjects, use one of the inheriting classes

public class EnemyBase : MonoBehaviour
{
    [Header("Basic Settings")]
    public bool active;
    public int health = 100;

    [Header("Detection Settings")]
    public Transform playerTransform;
    public float sightRange = 25.0f;
    protected Vector3 moveTarget; // the object it follows
    protected Vector3 lookTarget; // the object to check line of sight with (usually will be the same as moveTarget, but doesn't have to be)
    NavMeshAgent navAgent;

    [HideInInspector] public EnemyState state;
    protected bool hasLineOfSight;
    protected bool shouldPath;

    [Header("General Movement Settings")]
    protected float speed = 5.0f;

    [Header("Advanced")]
    [SerializeField] TMP_Text debugStateText;
    Collider colliderr;
    [SerializeField] protected GameObject mesh;
    [SerializeField] protected Vector3 lineOfSightOffset;
    [SerializeField] protected Vector3 floorRaycastPos;
    [SerializeField] protected float floorRaycastLength;


    public void Start()
    {
        // initialise pathing values
        shouldPath = false;
        colliderr = GetComponentInChildren<Collider>();
        navAgent = GetComponent<NavMeshAgent>();

        // start spawned or despawned
        if (active) Spawn();
        else Despawn();
    }

    public void Update()
    {
        if (!active) return;

        if (debugStateText != null) debugStateText.text = state.ToString();


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
        if (hasLineOfSight) Debug.DrawRay(raycastPos, (lookTarget - raycastPos).normalized * sightRange, new(1.0f, 0.0f, 0.0f));
        else Debug.DrawRay(raycastPos, (lookTarget - raycastPos).normalized * sightRange, new(0.0f, 0.0f, 1.0f));
        Debug.DrawRay(raycastPos, (moveTarget - raycastPos).normalized * sightRange, new(0.5f, 0.0f, 0.5f));


        // enemy types that inherit from this decide when to set shouldPath to true or false
        if (shouldPath)
        {
            navAgent.destination = moveTarget;
            navAgent.enabled = true;
            navAgent.speed = speed;
        }
        else navAgent.enabled = false;
    }

    public void OnDrawGizmos()
    {
        if (navAgent)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(moveTarget, 0.5f);
        }
    }

    public virtual void Hit(int damage)
    {
        // probably add more parameters to this, like what type of hit
        health -= damage;
    }

    public void Spawn()
    {
        active = true;
        mesh.SetActive(true);
        colliderr.enabled = true;
    }

    public void Despawn()
    {
        active = false ;
        mesh.SetActive(false);
        colliderr.enabled = false;
    }
}
