using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class RangedWeapon : MonoBehaviour
{

    [Header("Gun Transforms")]

    [SerializeField]
    public Transform muzzlePoint;

    [SerializeField]
    public Transform altMuzzlePoint;

    [Header("Gun Behaviour Values")]

    [SerializeField]
    [Tooltip("Ain't it self-explanatory?")]
    public int DamageValue = 25;

    [SerializeField]
    [Tooltip("A multiplier for the amount of force applied to an object  by the bullets")]
    public float bulletForceMultiplier = 1;

    [Tooltip("Essentialy how long it takes to fire the next Bullet after one has already been fired")]
    [SerializeField]
    public float shotGapTime = 1f;

    [Tooltip("Essentialy how long it takes to fire the next Bullet after one has already been fired")]
    [SerializeField]
    public float AltshotGapTime = 0.01f;

    [Tooltip("How Long it takes to Refill The Clip; This is in Seconds")]
    [SerializeField]
    public float reloadTime = 2f;

    [SerializeField]
    [Tooltip("How many bullets you can launch before reloading")]
    public int BulletsPerClip = 6;

    [SerializeField]
    [Tooltip("Current Number of Bullets")]
    public int currentBullets;

    [Header("Gun Behaviour Bools (Do NOT mess With)")]

    [SerializeField]
    [Tooltip("Just a bool for whether Primary Fire should happen")]
    public bool shouldShootPrimary = false;

    [SerializeField]
    [Tooltip("Just a bool for if the secondary fire should happen ")]
    public bool shouldShootAlt = false;

    [SerializeField]
    public bool canFire;

    [SerializeField]
    public bool canPressAltFire = true;

    [SerializeField]
    public bool waiting = false;

    [SerializeField]
    public bool reloading;

    [Header("Visual Effects")]
    [SerializeField]
    public VisualEffect BulletFlash;

    [SerializeField]
    public GameObject HitEffect;

    [SerializeField]
    public GameObject BulletHitDecal;

    [Header("Scene Refrences")]
    [SerializeField]
    public Camera camRef;

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

        if (shouldShootPrimary == true && waiting == false && reloading == false && canPressAltFire == true)
        {
            EngagePrimaryFire();
        }

        if (shouldShootAlt == true && canPressAltFire == true && waiting == false && reloading == false )
        {
            EngageAltFire();
        }
    }

    public virtual void EngagePrimaryFire()
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

                    if (!rayData.hit.transform.parent && !rayData.hit.transform.TryGetComponent<EnemyBase>(out EnemyBase eb)) //AND it isn't an enemy
                    {
                        GameObject Decal = Instantiate(BulletHitDecal);
                        Decal.transform.parent = rayData.hit.transform;
                        Decal.transform.position = rayData.hit.point;
                    }

                    if (rayData.hit.transform.parent)
                    {
                        if (rayData.hit.transform.parent.TryGetComponent<EnemyBase>(out EnemyBase eb2))
                        {
                            Health EnemyHealth = rayData.hit.collider.transform.parent.GetComponentInChildren<Health>();
                            EnemyHealth.TakeDamage(DamageValue, 0);
                        }
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


    public virtual void EngageAltFire()
    {
        //altFire Logic

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
    public IEnumerator Wait(float shotGapTime)
    {
        waiting = true;
        yield return new WaitForSeconds(shotGapTime);
        Debug.Log("Waiting...");
        canFire = true;
        waiting = false;
    }

    public IEnumerator Reload()
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


    //active on beginning of Primary fire Action
    public virtual void OnPrimaryFireBegin()
    {
        shouldShootPrimary = true;
        Debug.Log("Beginning primary Fire");
    }

    //Active on Begining of alt-firing action
    public virtual void OnAltFireBegin()
    {
        shouldShootAlt = true;
        Debug.Log("Beginning primary Fire");
    }

    //Active every interval of Primaryfire set in this script
    public virtual void OnPrimaryFireStay()
    {
        if (shouldShootPrimary)
        {
            Debug.Log("Primary fire stay ");
        }
    }

    //Active every interval  of altfire set in this script
    public virtual void OnAltFireStay()
    {
        if (shouldShootAlt)
        {
            Debug.Log("alt fire stay ");
        }

    }

    //active on primary fire End
    public virtual void OnprimaryFireEnd()
    {
        shouldShootPrimary = false;
        Debug.Log("end Primary Fire");
    }

    //active on Alt-fire End
    public virtual void OnAltFireEnd()
    {
        shouldShootAlt = false;
        Debug.Log("end alt fire");
    }

}
