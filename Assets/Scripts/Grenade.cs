using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[ExecuteInEditMode]
public class Grenade : MonoBehaviour
{
    [SerializeField]
    int damage = 50;

    [SerializeField]
    Collider collider;

    [SerializeField]
    float colliderRadius;

    [SerializeField]
    float blastRadius;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        colliderRadius = ((SphereCollider)collider).radius;
        //blastRadius = colliderRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }


    // Update is called once per frame
    void Update()
    {
        //((SphereCollider)collider).radius = colliderRadius;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("exploding");
        Explosion();
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("exploding");
    //    Explosion();
    //}

    void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (var hitCollider in hitColliders)
        {
            Health hpscript = null;

            if (hitCollider.transform.parent)
            {
                if (hitCollider.transform.parent.GetComponentInChildren<Health>())
                {
                    hpscript = hitCollider.transform.parent.GetComponentInChildren<Health>();
                    Debug.Log(hpscript);
                    Debug.Log(hitCollider.name + "Was Caught in Blast");
                    hpscript.TakeDamage(damage, 0);
                    continue;
                }
                else if (hitCollider.transform.parent.TryGetComponent<Health>(out hpscript))
                {
                    Debug.Log(hpscript);
                    Debug.Log(hitCollider.name + "Was Caught in Blast");
                    hpscript.TakeDamage(damage, 0);
                    continue;
                }
                else
                    Debug.Log("Not allowed to take Damage");
            }
            else if (hitCollider.TryGetComponent<Health>(out hpscript))
            {
                Debug.Log(hpscript);
                Debug.Log(hitCollider.name + "Was Caught in Blast");
                hpscript.TakeDamage(damage, 0);
            }
        }

        Destroy(gameObject);
    }
}
