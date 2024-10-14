using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[ExecuteInEditMode]
public class AoeDamager : MonoBehaviour
{
    [Tooltip("Damage on initial Blast")]
    [SerializeField]
    int damage = 50;

    [Tooltip("This will be pulled automatically from the object hierchy")]
    [SerializeField]
    Collider collider;

    [Tooltip("area of effect range")]
    [SerializeField]
    float blastRadius;

    [SerializeField]
    GameObject explosionVFX;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }


    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.parent)
        {
            if (collision.gameObject.transform.parent.gameObject.layer != 3)
            {
                Debug.Log("exploding");
                Explosion();
            }
        }
        else if (collision.gameObject.transform.gameObject.layer != 3)
        {
            Debug.Log("exploding");
            Explosion();
        }
    }

    void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
        GameObject vfx = Instantiate(explosionVFX);
        vfx.transform.position = transform.position;

        foreach (var hitCollider in hitColliders)
        {
            Health hpscript = null;

            if (hitCollider.transform.parent)
            {
                if (hitCollider.transform.parent.GetComponentInChildren<Health>())
                {
                    hpscript = hitCollider.transform.parent.GetComponentInChildren<Health>();
                    Debug.Log(hpscript);


                    //make sure on  Kill isnt all ready an event;
                   
                    Debug.Log(hitCollider.name + "Was Caught in Blast");
                    hpscript.TakeDamage(damage, 0);
                    continue;
                }
                else if (hitCollider.transform.parent.TryGetComponent<Health>(out hpscript))
                {
                    Debug.Log(hpscript);
                    Debug.Log(hitCollider.name + "Was Caught in Blast");

                    //make sure on  Kill isnt all ready an event;
                 

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


                         