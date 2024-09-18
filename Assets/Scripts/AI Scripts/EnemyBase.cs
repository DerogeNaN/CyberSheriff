using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

// BASE ENEMY SCRIPT

// TODO: DISABLE COLLIDERS WHEN NOT SPAWNED

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
    [SerializeField] protected GameObject mesh;
    [SerializeField] protected Vector3 lineOfSightOffset;
    [SerializeField] protected Vector3 floorRaycastPos;
    [SerializeField] protected float floorRaycastLength;

    virtual protected void OnStart() { }
    virtual protected void OnUpdate() { }


    public void Start()
    {
        // initialise pathing values
        shouldPath = false;
        navAgent = GetComponent<NavMeshAgent>();

        // start spawned or despawned
        if (active) Spawn();
        else Despawn();

        SetPlayerTransform();
        OnStart();
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
            // teleport back to navmesh
            if (!navAgent.isOnNavMesh)
            {
                //Debug.Log("aaaaa");
                Debug.Log(moveTarget);
                //NavMeshHit hit;
                //if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
                //{
                //    Debug.Log(hit.position);
                //    //navAgent.Warp(hit.position);
                //}
            }

            navAgent.enabled = true;
            navAgent.destination = moveTarget;
            navAgent.speed = speed;
        }
        else navAgent.enabled = false;

        UpdateState();
        OnUpdate();
    }

    public void SetState(EnemyState state)
    {
        ExitState();
        this.state = state;
        EnterState();
    }

    private void EnterState()
    {
        switch (state)
        {
            case EnemyState.idle:
                IdleEnter();
                break;
            case EnemyState.movingToTarget:
                MovingToTargetEnter();
                break;
            case EnemyState.lostSightOfTarget:
                LostSightOfTargetEnter();
                break;
            case EnemyState.attacking:
                AttackingEnter();
                break;
        }
    }

    private void UpdateState()
    {
        switch (state)
        {
            case EnemyState.idle:
                IdleUpdate();
                break;
            case EnemyState.movingToTarget:
                MovingToTargetUpdate();
                break;
            case EnemyState.lostSightOfTarget:
                LostSightOfTargetUpdate();
                break;
            case EnemyState.attacking:
                AttackingUpdate();
                break;
        }
    }

    private void ExitState()
    {
        switch (state)
        {
            case EnemyState.idle:
                IdleExit();
                break;
            case EnemyState.movingToTarget:
                MovingToTargetExit();
                break;
            case EnemyState.lostSightOfTarget:
                LostSightOfTargetExit();
                break;
            case EnemyState.attacking:
                AttackingExit();
                break;
        }
    }

    virtual protected void IdleEnter() { }
    virtual protected void IdleUpdate() { }
    virtual protected void IdleExit() { }

    virtual protected void MovingToTargetEnter() { }
    virtual protected void MovingToTargetUpdate() { }
    virtual protected void MovingToTargetExit() { }

    virtual protected void LostSightOfTargetEnter() { }
    virtual protected void LostSightOfTargetUpdate() { }
    virtual protected void LostSightOfTargetExit() { }

    virtual protected void AttackingEnter() { }
    virtual protected void AttackingUpdate() { }
    virtual protected void AttackingExit() { }

    public void OnDrawGizmos()
    {
        if (navAgent)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(moveTarget, 0.5f);
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