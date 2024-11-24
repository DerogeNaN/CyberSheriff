using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{

    public float despawnTime;

    public Rigidbody torso;

    void Start()
    {
        Destroy(gameObject, despawnTime);
    }

    public void ApplyForce(Vector3 hitNormal, float hitStrength)
    {
        torso.AddForce(hitNormal * hitStrength, ForceMode.Impulse);
    }
}
