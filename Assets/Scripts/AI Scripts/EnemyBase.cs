using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// NEW BASE ENEMY SCRIPT

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
    public float stopDistance = 1.0f;
    protected Vector3 moveTarget; // the object it follows
    protected Vector3 lookTarget; // the object to check line of sight with (usually will be the same as moveTarget, but doesn't have to be)

    [Header("Pathing Settings")]
    // navigation
    [SerializeField] NavMeshSurface navMesh;
    public float repathFrequency = 1.0f;
    protected NavMeshPath path;
    float untilRepath;

    [HideInInspector] public EnemyState state;
    protected bool hasLineOfSight;
    protected bool shouldPath;

    [Header("General Movement Settings")]
    // movement
    protected float speed = 5.0f;
    int nextCorner = 0;

    [Header("Advanced")]
    [SerializeField] TMP_Text debugStateText;
    Collider colliderr;
    [SerializeField] GameObject mesh;
    [SerializeField] protected Vector3 lineOfSightOffset;
    [SerializeField] protected Vector3 floorRaycastPos;
    [SerializeField] protected float floorRaycastLength;


    public void Start()
    {
        // initialise pathing values
        shouldPath = false;
        untilRepath = repathFrequency;
        path = new();

        // get components
        colliderr = GetComponentInChildren<Collider>();

        // start spawned or despawned
        if (active) Spawn();
        else Despawn();
    }

    public void Update()
    {
        if (!active) return;

        if (debugStateText != null) debugStateText.text = state.ToString();

        // if the target is out of range, don't raycast
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
        if (shouldPath && Vector3.Distance(transform.position, moveTarget) > stopDistance)
        {
            // recalculate path
            untilRepath -= Time.deltaTime;
            if (untilRepath <= 0)
            {
                CalculatePath();
            }

            // move
            if (path.corners.Length > 0 && nextCorner < path.corners.Length)
            {
                Move();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CalculatePath();
        }
    }

    void Hit(int damage)
    {
        // probably add more parameters to this, like what type of hit
        health -= damage;
        OnHit(damage);
    }

    public virtual void OnHit(int damage)
    {

    }

    void CalculatePath()
    {
        NavMesh.CalculatePath(transform.position, moveTarget, NavMesh.AllAreas, path);
        untilRepath = repathFrequency;
        nextCorner = 1;
    }

    void Move()
    {
        Vector3 next = path.corners[nextCorner];
        Vector3 dirToNextCorner = (next - transform.position).normalized;

        transform.position += dirToNextCorner * speed * Time.deltaTime;
        if ((next - transform.position).magnitude <= 1.0f) // if less than 1 unit away from target corner
        {
            if (nextCorner < path.corners.Length - 1) nextCorner++;
        }
    }

    void StayAboveFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + floorRaycastPos, -transform.up, out hit, floorRaycastLength))
        {
            float distanceIntoFloor = hit.distance;
            transform.position += new Vector3(0, distanceIntoFloor);

            Debug.Log(distanceIntoFloor);
        }

        Debug.DrawRay(transform.position + floorRaycastPos, -transform.up, Color.green);
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

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < path.corners.Length; i++)
            {
                Gizmos.DrawWireSphere(path.corners[i], 0.5f);
                if (i < path.corners.Length - 1) Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            }
        }
    }


}
