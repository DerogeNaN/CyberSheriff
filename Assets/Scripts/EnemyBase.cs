using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
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

    [SerializeField] TMP_Text debugStateText;

    // navigation
    [SerializeField] NavMeshSurface navMesh;
    public float repathFrequency = 1.0f;
    protected NavMeshPath path;
    float untilRepath;
    Vector3 lastPos;

    // movement
    public float speed = 5.0f;
    float lerpAmount = 0.0f;
    int nextCorner = 0;

    public void Start()
    {
        shouldPath = false;
        untilRepath = repathFrequency;
        path = new();
        lastPos = transform.position;
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
            // recalculate path
            untilRepath -= Time.deltaTime;
            if (untilRepath <= 0)
            {
                //CalculatePath();
            }

            // move
            if (path.corners.Length > 0)
            {
                if (lerpAmount < 1.0f)
                {
                    lerpAmount += Time.deltaTime * speed;
                    transform.position = Vector3.Lerp(lastPos, path.corners[nextCorner], lerpAmount);
                }
                else if (nextCorner < path.corners.Length - 1)
                {
                    lerpAmount = 0;
                    lastPos = transform.position;
                    nextCorner++;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CalculatePath();
        }
    }

    public virtual void Hit(int damage)
    {
        // probably add more parameters to this, like what type of hit
        health -= damage;

        Debug.Log("hit " + name);
    }

    void CalculatePath()
    {
        NavMesh.CalculatePath(transform.position, moveTarget, NavMesh.AllAreas, path);
        untilRepath = repathFrequency;
        lerpAmount = 0.0f;
        nextCorner = 0;
        lastPos = transform.position;
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
