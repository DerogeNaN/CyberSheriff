using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.VFX;
using TMPro;
using System.Linq.Expressions;
using UnityEditor.Rendering;
using UnityEngineInternal;
using System.Diagnostics.CodeAnalysis;

public class Revolver : MonoBehaviour
{

    [Header("Gun Transforms")]

    [SerializeField]
    public Transform muzzlePoint;

    [SerializeField]
    public Transform altMuzzlePoint;

    [Header("Gun Behaviour Values")]

    [SerializeField]
    [Tooltip("Ain't it self-explanatory?")]
    int DamageValue = 25;

    [SerializeField]
    [Tooltip("A multiplier for the amount of force applied to an object  by the bullets")]
    float bulletForceMultiplier = 1;

    [Tooltip("Essentialy how long it takes to fire the next Bullet after one has already been fired")]
    [SerializeField]
    float shotGapTime = 1f;

    [Tooltip("Essentialy how long it takes to fire the next Bullet after one has already been fired")]
    [SerializeField]
    float AltshotGapTime = 0.01f;

    [Tooltip("How Long it takes to Refill The Clip")]
    [SerializeField]
    float reloadTime = 2f;

    [SerializeField]
    [Tooltip("How many bullets you can launch before reloading")]
    public int BulletsPerClip = 6;

    [SerializeField]
    [Tooltip("Current Number of Bullets")]
    public int currentBullets;

    float BulletSpeed = 200;//obsolete till further notice

    [Header("Gun Behaviour Bools (Do NOT mess With)")]

    [SerializeField]
    [Tooltip("Just a bool for whether Primary Fire should happen")]
    bool shouldShootPrimary = false;

    [SerializeField]
    [Tooltip("Just a bool for if the secondary fire should happen ")]
    bool shouldShootAlt = false;

    [SerializeField]
    bool canFire;

    [SerializeField]
    bool canPressAltFire = true;

    [SerializeField]
    bool waiting = false;

    [SerializeField]
    bool reloading;

    [Header("Visual Effects")]
    [SerializeField]
    VisualEffect BulletFlash;

    [SerializeField]
    GameObject HitEffect;

    [SerializeField]
    GameObject BulletHitDecal;

    [Header("Scene Refrences")]
    [SerializeField]
    Camera camRef;

    [SerializeField]
    public GameObject CurrentlyHitting;


    public struct RayData
    {
        public Ray ray;
        public RaycastHit hit;
    }

    // Start is called before the first frame update
    void Start()
    {
        camRef = FindAnyObjectByType<Camera>();
        currentBullets = BulletsPerClip;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentBullets <= 0 && reloading == false)
        {
            canFire = false;
            StartCoroutine(Reload());
        }

        if (shouldShootPrimary == true && waiting == false && reloading == false)
        {
            EngagePrimaryFire();
        }

        if (shouldShootAlt == true && canPressAltFire == true && waiting == false && reloading == false)
        {
            EngageAltFire();
        }
    }

    public void EngagePrimaryFire()
    {
        //Primary Fire Logic
        if (currentBullets > 0)
        {
            RayData rayData = RayCastAndGenGunRayData(muzzlePoint);
            BulletFlash.Play();
            currentBullets--;
            if (rayData.hit.point != null)
            {
                CurrentlyHitting = rayData.hit.transform.gameObject;

                if (rayData.hit.transform.gameObject.layer != 3) //If the thing hit isn't the player...
                {
                    if (!rayData.hit.transform.GetComponent<EnemyBase>()) //AND it isn't an enemy
                    {
                        GameObject Decal = Instantiate(BulletHitDecal);
                        Decal.transform.parent = rayData.hit.transform;
                        Decal.transform.position = rayData.hit.point;
                    }
                    //..It isn't the player but it is an enemy...?
                    GameObject hitFX = Instantiate(HitEffect);
                    hitFX.transform.position = rayData.hit.point;
                    if (rayData.hit.rigidbody)
                    {
                        rayData.hit.rigidbody.AddForce(rayData.ray.direction * bulletForceMultiplier, ForceMode.Impulse);
                    }
                    else
                    {
                        Debug.Log("Does Not have rigidbody");
                    }

                    if (rayData.hit.transform.GetComponent<EnemyBase>()) //AND it isn't an enemy
                    {
                        Debug.Log("collider:" + rayData.hit.collider);
                        Debug.Log("parent:" + rayData.hit.collider.transform.parent);
                        Debug.Log("health component:" + rayData.hit.collider.transform.parent.GetComponent<Health>());

                        //    Debug.Log("taking away Health");
                        //  //  rayData.hit.collider.transform.parent.GetComponent<Health>().TakeDamage(DamageValue, 0);
                    }
                }
            }
            canFire = false;
            StartCoroutine(Wait(shotGapTime));
        }
        else if (currentBullets <= 0 && reloading == false)
        {
            canFire = false;
            StartCoroutine(Reload());
        }
    }


    public void EngageAltFire()
    {
        //altFire Logic
        if (currentBullets > 0)
            StartCoroutine(FanFire());
        else if (currentBullets <= 0 && reloading == false)
        {
            Debug.Log("out off Bullets");
            canFire = false;
            StartCoroutine(Reload());
        }
    }


    public RayData RayCastAndGenCameraRayData()
    {
        Ray cameraRay = new Ray();

        RaycastHit cameraHit;

        cameraRay.origin = camRef.ScreenToWorldPoint(Vector3.zero);

        cameraRay.direction = camRef.transform.forward;

        Physics.Raycast(cameraRay, out cameraHit, Mathf.Infinity);

        return new RayData { ray = cameraRay, hit = cameraHit };
    }

    public RayData RayCastAndGenGunRayData(Transform muzzle)
    {
        Ray gunRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        gunRay.direction = muzzlePoint.transform.forward;
        gunRay.origin = muzzlePoint.position;

        RaycastHit gunHit;
        RayData camRayData = RayCastAndGenCameraRayData();
        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 

        Vector3 barrelToLookPointDir = camRayData.hit.point - muzzle.transform.position;

        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        gunRay.direction = barrelToLookPointDir;

        Physics.Raycast(gunRay, out gunHit, Mathf.Infinity);

        return new RayData { ray = gunRay, hit = gunHit };
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
        if (currentBullets != BulletsPerClip)
        {
            currentBullets = BulletsPerClip;
        }
        reloading = false;
    }


    IEnumerator FanFire()
    {
        Debug.Log("Fire start");
        canPressAltFire = false;
        int BulletsAtTimeOfFiring = currentBullets;
        Debug.Log("Current Bullet Number:" + BulletsAtTimeOfFiring);
        for (int i = 0; i < BulletsAtTimeOfFiring; i++)
        {
            Debug.Log("FanFire Bullet Loosed");
            BulletFlash.Play();
            currentBullets--;

            RayData rayData = RayCastAndGenGunRayData(altMuzzlePoint);
            if (rayData.hit.point != null)
            {
                CurrentlyHitting = rayData.hit.transform.gameObject;

                // bullet Hole Decal Placement Logic 
                if (!rayData.hit.transform.GetComponent<NavMeshAgent>() && rayData.hit.transform.gameObject.layer != 3)
                {
                    GameObject Decal = Instantiate(BulletHitDecal);
                    Decal.transform.parent = rayData.hit.transform;
                    Decal.transform.position = rayData.hit.point;

                    GameObject hitFX = Instantiate(HitEffect);
                    hitFX.transform.position = rayData.hit.point;
                    Destroy(hitFX, 5);

                }

                if (rayData.hit.rigidbody != null && rayData.hit.transform.root.GetComponent<Movement>() == false)
                {
                    rayData.hit.rigidbody.AddForce(rayData.ray.direction * bulletForceMultiplier, ForceMode.Impulse);
                }

                if (rayData.hit.collider.gameObject.GetComponent<Health>())
                {
                    rayData.hit.collider.gameObject.GetComponent<Health>().TakeDamage(DamageValue, 0);
                }
            }

            canFire = false;
            yield return new WaitForSeconds(AltshotGapTime);
        }
        Debug.Log("Fan Fire end");
        canPressAltFire = true;
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
