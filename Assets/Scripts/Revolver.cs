using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.AI;

public class Revolver : MonoBehaviour
{

    [Header("Gun Transforms")]

    [SerializeField]
    public Transform muzzlePoint;

    [SerializeField]
    public Transform altMuzzlePoint;

    [Header("Gun Behaviour")]

    [SerializeField]
    int DamageValue = 25;

    [SerializeField]
    public GameObject CurrentlyHitting;

    [SerializeField]
    [Tooltip("Just a bool for whether Primary Fire Should Happen")]
    bool shouldShootPrimary = false;

    [SerializeField]
    [Tooltip("Just a Bool for if the seondary fire shoud happen ")]
    bool shouldShootAlt = false;


    [SerializeField]
    [Tooltip("a multiplier for the amount of force applied to an object  by the bullets")]
    float bulletForceMultiplier = 1;

    [SerializeField]
    GameObject bulletTrailPrefab;

    [SerializeField]
    float shotGapTime = 1f;

    [SerializeField]
    float AltshotGapTime = 0.01f;

    [SerializeField]
    float reloadTime = 2f;

    [SerializeField]
    bool canFire;

    [SerializeField]
    Camera camRef;

    [SerializeField]
    GameObject BulletHitDecal;

    [SerializeField]
    [Tooltip("How many bullets you can launch before reloading")]
    public int BulletsPerClip = 6;

    [SerializeField]
    public int currentBullets;

    [SerializeField]
    bool waiting = false;

    [SerializeField]
    bool reloading;

    // Start is called before the first frame update
    void Start()
    {
        camRef = FindAnyObjectByType<Camera>();
        currentBullets = BulletsPerClip;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray();
        Ray cameraRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        ray.direction = muzzlePoint.transform.forward;
        ray.origin = muzzlePoint.position;

        RaycastHit hit = new RaycastHit();
        RaycastHit cameraHit;


        //GUN camera Interactions here 
        cameraRay.origin = camRef.ScreenToWorldPoint(Vector3.zero);
        cameraRay.direction = camRef.transform.forward;

        Physics.Raycast(cameraRay, out cameraHit, Mathf.Infinity);

        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 
        Vector3 barrelToLookPointDir = cameraHit.point - muzzlePoint.transform.position;
        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        ray.direction = barrelToLookPointDir;

        if (shouldShootPrimary == true && waiting == false)
        {
            ray.origin = muzzlePoint.transform.position;
            canFire = true;
        }

        if (shouldShootAlt == true && waiting == false)
        {
            ray.origin = altMuzzlePoint.transform.position;
            canFire = true;
        }

        //Primary Fire Logic
        if (shouldShootPrimary && canFire && currentBullets > 0)
        {
            currentBullets--;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject bulletfab = Instantiate(bulletTrailPrefab);
                CurrentlyHitting = hit.transform.gameObject;

                if (hit.point != null)
                {
                    bulletfab.GetComponent<LineRenderer>().SetPosition(0, ray.origin);
                    bulletfab.GetComponent<LineRenderer>().SetPosition(1, hit.point);

                    // bullet Hole Decal Placement Logic 
                    if (!hit.transform.GetComponent<NavMeshAgent>())
                    {
                        GameObject Decal = Instantiate(BulletHitDecal);
                        Decal.transform.parent = hit.transform;
                        Decal.transform.position = hit.point;
                    }
                }

                if (hit.rigidbody != null && hit.transform.root.GetComponent<Movement>() == false)
                {
                    Debug.Log("Root" + hit.rigidbody.transform.root);
                    Debug.Log("Impulse" + hit.rigidbody.name);
                    hit.rigidbody.AddForce(barrelToLookPointDir * bulletForceMultiplier, ForceMode.Impulse);
                }
                else
                {
                    Debug.Log("part of The Player ");
                }


                if (hit.collider.gameObject.GetComponent<Health>())
                {
                    Debug.Log("Die");
                    hit.collider.gameObject.GetComponent<Health>().TakeDamage(DamageValue, 0);
                }


            }
            canFire = false;
            StartCoroutine(Wait(shotGapTime));
        }
        else if (currentBullets <= 0)
        {
            canFire = false;
            StartCoroutine(Reload());
        }

        //altFire Logic
        if (shouldShootAlt && canFire && currentBullets > 0)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject bulletfab = Instantiate(bulletTrailPrefab);
                CurrentlyHitting = hit.transform.gameObject;

                if (hit.point != null)
                {
                    bulletfab.GetComponent<LineRenderer>().SetPosition(0, ray.origin);
                    bulletfab.GetComponent<LineRenderer>().SetPosition(1, hit.point);

                    // bullet Hole Decal Placement Logic 
                    if (!hit.transform.GetComponent<NavMeshAgent>())
                    {
                        GameObject Decal = Instantiate(BulletHitDecal);
                        Decal.transform.parent = hit.transform;
                        Decal.transform.position = hit.point;
                    }
                }

                if (hit.rigidbody != null && hit.transform.root.GetComponent<Movement>() == false)
                {
                    Debug.Log("Root" + hit.rigidbody.transform.root);
                    Debug.Log("Impulse" + hit.rigidbody.name);
                    hit.rigidbody.AddForce(barrelToLookPointDir * bulletForceMultiplier, ForceMode.Impulse);
                }
                else
                {
                    Debug.Log("part of The Player ");
                }


                if (hit.collider.gameObject.GetComponent<Health>())
                {
                    Debug.Log("Die");
                    hit.collider.gameObject.GetComponent<Health>().TakeDamage(DamageValue, 0);
                }


            }
            canFire = false;
            StartCoroutine(Wait(AltshotGapTime));
        }

    }

    //This coroutine  was made so the gun would wait for the shot gap time to pass before being able to fire again

    IEnumerator Wait(float shotGapTime)
    {
        waiting = true;
        yield return new WaitForSeconds(shotGapTime);
        Debug.Log("Waiting...");
        canFire = true;
        waiting = false;
    }

    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        Debug.Log("Reloading...");
        canFire = false;
        if(currentBullets != BulletsPerClip)
        {
            currentBullets = BulletsPerClip;
        }
        reloading = false;

    }



    //active on beginning of Primary fire Action
    public void OnPrimaryFireBegin()
    {
        shouldShootPrimary = true;
        Debug.Log("Beginning primary Fire");
    }

    //Active on Begining of alt-firing action
    public void OnAltFireBegin()
    {
        shouldShootAlt = true;
        Debug.Log("Beginning primary Fire");

    }

    //Active every interval of Primaryfire set in this script
    public void OnPrimaryFireStay()
    {
        if (shouldShootPrimary)
        {
            Debug.Log("Primary fire stay ");
        }

    }

    //Active every interval  of altfire set in this script
    public void OnAltFireStay()
    {
        if (shouldShootAlt)
        {
            Debug.Log("alt fire stay ");
        }

    }

    //active on primary fire End
    public void OnprimaryFireEnd()
    {
        shouldShootPrimary = false;
        Debug.Log("end Primary Fire");
    }

    //active on Alt-fire End
    public void OnAltFireEnd()
    {
        shouldShootAlt = false;
        Debug.Log("end alt fire");
    }

}
