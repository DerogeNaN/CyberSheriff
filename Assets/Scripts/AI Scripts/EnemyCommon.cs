using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

// this is a class containing common functions used my enemies, so include this on enemy GameObjects along with a class the inherits from EnemyBase

public class EnemyCommon : MonoBehaviour
{
    [Header("Enemy Settings")]
    [HideInInspector] public bool active;
    [Tooltip("maximum distance at which the player can be considered in line of sight")]
    public float sightRange = 25.0f;
    [Tooltip("offset from where the enemy's eyes are, for checking line of sight")]
    public Vector3 lineOfSightOffset;
    [HideInInspector] public float speed = 5.0f;

    [Header("Advanced")]
    [Tooltip("reference to the enemy's mesh GameObject, used for animation")]
    public GameObject mesh;
    public SkinnedMeshRenderer meshRenderer;
    [Tooltip("the animator to use. should be a component of the mesh GameObject")]
    public Animator animator;

    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public Vector3 moveTarget; // the object it follows
    [HideInInspector] public Vector3 lookTarget; // the object to check line of sight with (usually will be the same as moveTarget, but doesn't have to be)
    [HideInInspector] public bool hasLineOfSight;
    [HideInInspector] public bool shouldPath;

    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public Health health;
    [SerializeField] GameObject hitEffectVFX;

    private void Start()
    {
        // initialise pathing values
        shouldPath = false;
        TryGetComponent<NavMeshAgent>(out navAgent);

        // start spawned or despawned
        if (active) Spawn();
        else Despawn();

        SetPlayerTransform();

        health = GetComponent<Health>();
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
            if (Physics.Raycast(raycastPos, (lookTarget - raycastPos).normalized, out RaycastHit hit, sightRange, ~LayerMask.GetMask(new[] { "Enemy" })))
            {
                hasLineOfSight = hit.transform.gameObject.CompareTag("Player");
            }
            else hasLineOfSight = true;
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
        GameObject vfx = Instantiate(hitEffectVFX, transform);
        ParticleSystem[] particles = vfx.GetComponentsInChildren<ParticleSystem>();
        
        foreach(ParticleSystem p in particles)
        {
            
        }
    }
}

