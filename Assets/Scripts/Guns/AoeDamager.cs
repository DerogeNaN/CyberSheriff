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

    [Tooltip("area of effect  for damage")]
    [SerializeField]
    float damageRadius;

    [Tooltip("area of effect  for damage")]
    [SerializeField]
    float stunRadius;

    [SerializeField]
    GameObject explosionVFX;

    [Tooltip("This is the Object That would be affected By the velocity based Rotaion")]
    [SerializeField]
    GameObject meshObjectRotator;

    [Tooltip("This activates the Fall Movement for the grenades(Mostly for shotgun)")]
    [SerializeField]
    bool InstallVelocityBasedObjectRotation = false;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();

        if (InstallVelocityBasedObjectRotation)
        {
            meshObjectRotator.transform.LookAt(transform.position + GetComponent<Rigidbody>().velocity, Vector3.up);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stunRadius);


    }


    // Update is called once per frame
    void Update()
    {
        if (InstallVelocityBasedObjectRotation)
        {
            meshObjectRotator.transform.LookAt(transform.position + GetComponent<Rigidbody>().velocity, Vector3.up);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.parent)
        {
            if (collision.gameObject.transform.parent.gameObject.layer != 3)
            {
                Debug.Log("exploding");
                Explosion(collision);
            }
        }
        else if (collision.gameObject.transform.gameObject.layer != 3)
        {
            Debug.Log("exploding");
            Explosion(collision);
        }
    }

    void Explosion(Collision collision)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        Collider[] stunHitColliders = Physics.OverlapSphere(transform.position, stunRadius);
        GameObject vfx = Instantiate(explosionVFX);
        SoundManager2.Instance.PlaySound("ShotgunGranadeExplosion", vfx.gameObject.transform);
        vfx.transform.position = transform.position;
        vfx.transform.LookAt(vfx.transform.position + collision.contacts[0].normal, Vector3.up);
        vfx.transform.position += -collision.contacts[0].normal;


        foreach (var stunHitCollider in stunHitColliders)
        {
            if (stunHitCollider.GetComponentInParent<EnemyBase>())
            {
                if (stunHitCollider.GetComponentInParent<EnemyMelee>())
                {
                    stunHitCollider.GetComponentInParent<EnemyMelee>().stun = 0.5f;
                    stunHitCollider.GetComponentInParent<EnemyMelee>().SetState(EnemyState.stunned);
                }

                if (stunHitCollider.GetComponentInParent<EnemyRanged>()) 
                {
                    stunHitCollider.GetComponentInParent<EnemyRanged>().stun = 0.5f;
                    stunHitCollider.GetComponentInParent<EnemyRanged>().SetState(EnemyState.stunned);
                }
            }
        }


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
                    hpscript.TakeDamage(damage, 3, gameObject);
                    continue;
                }
                else if (hitCollider.transform.parent.TryGetComponent(out hpscript))
                {
                    Debug.Log(hpscript);
                    Debug.Log(hitCollider.name + "Was Caught in Blast");

                    //make sure on  Kill isnt all ready an event;


                    hpscript.TakeDamage(damage, 3, gameObject);
                    continue;
                }
                else
                    Debug.Log("Not allowed to take Damage");
            }
            else if (hitCollider.TryGetComponent(out hpscript))
            {
                Debug.Log(hpscript);
                Debug.Log(hitCollider.name + "Was Caught in Blast");
                hpscript.TakeDamage(damage, 3, gameObject);
            }
        }
        Destroy(gameObject);
    }



}


