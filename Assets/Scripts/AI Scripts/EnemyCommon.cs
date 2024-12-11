using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.Shapes;

// this is a class containing common functions used my enemies, so include this on enemy GameObjects along with a class the inherits from EnemyBase

public class EnemyCommon : MonoBehaviour
{
    [Header("Enemy Settings")]
    public bool active = true;
    [Tooltip("maximum distance at which the player can be considered in line of sight")]
    public float sightRange = 25.0f;
    [Tooltip("offset from where the enemy's eyes are, for checking line of sight")]
    public Vector3 lineOfSightOffset;
    [HideInInspector] public float speed = 5.0f;

    [Header("dont change")]
    public Vector3 initialPosition;
    public GameObject mesh;
    public Animator animator;
    public Transform playerTransform;

    public Vector3 moveTarget; // the object it follows
    public Vector3 lookTarget; // the object to check line of sight with (usually will be the same as moveTarget, but doesn't have to be)
    public bool hasLineOfSight;
    [SerializeField] GameObject hitEffectVFX;
    public bool shouldPath;

    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public Health health;

    private void Start()
    {
        Debug.Log(hitEffectVFX);
        // initialise pathing values
        TryGetComponent<NavMeshAgent>(out navAgent);
        //if (navAgent) navAgent.autoTraverseOffMeshLink = false;

        health = GetComponent<Health>();
        mesh = transform.Find("Mesh").gameObject;
        animator = transform.GetComponentInChildren<Animator>();

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
            if (Physics.Raycast(raycastPos, (lookTarget - raycastPos).normalized, out RaycastHit hit, sightRange, ~LayerMask.GetMask(new[] { "Enemy", "Ignore Raycast" })))
            {
                hasLineOfSight = hit.transform.gameObject.CompareTag("Player");
            }
            else hasLineOfSight = true;
            //Debug.Log(hit.collider.gameObject.name);
        }
        else hasLineOfSight = false;

        // draw ray for debugging
        if (hasLineOfSight) Debug.DrawRay(raycastPos, (lookTarget - raycastPos).normalized * sightRange, new(1.0f, 0.0f, 0.0f));
        else Debug.DrawRay(raycastPos, (lookTarget - raycastPos).normalized * sightRange, new(0.0f, 0.0f, 1.0f));
        Debug.DrawRay(raycastPos, (moveTarget - raycastPos).normalized * sightRange, new(0.5f, 0.0f, 0.5f));
    }

    private void UpdateNavAgent()
    {
        if (navAgent != null)
        {
            // enemy types that inherit from this decide when to set shouldPath to true or false
            if (shouldPath)
            {
                navAgent.speed = speed;
                navAgent.SetDestination(moveTarget);
            }
            else
            {
                navAgent.speed = 0;
                navAgent.velocity = Vector3.zero;
            }
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
        // get the GameObject that the movement script is on
        playerTransform = Movement.playerMovement.gameObject.transform;
    }

    public void CreateHitEffect()
    {
        if (hitEffectVFX)
            Instantiate(hitEffectVFX, transform);
    }
}

