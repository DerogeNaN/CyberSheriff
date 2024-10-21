using System.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using static RangedWeapon;

public class RangedWeapon : MonoBehaviour
{

    [Header("Gun Transforms")]

    [SerializeField]
    public Transform muzzlePoint;

    [SerializeField]
    public Transform altMuzzlePoint;

    [Header("Gun Behaviour Values")]

    [SerializeField]
    public Shotgun shotgun;

    [SerializeField]
    [Tooltip("Ain't it self-explanatory?")]
    public int DamageValue = 25;

    [SerializeField]
    public int headShotMultiplier = 2;

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
    public GameObject enemyHitEffect;

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
    public virtual void Awake()
    {
        camRef = FindAnyObjectByType<Camera>();
        currentBullets = BulletsPerClip;
    }

    // Update is called once per frame
    public virtual void Update()
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

        if (shouldShootAlt == true && canPressAltFire == true && waiting == false && reloading == false)
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
            ParticleSystem ps = BulletFlash.gameObject.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            currentBullets--;
            if (rayData.hit.point != null)
            {
                CurrentlyHitting = rayData.hit.transform.gameObject;

                if (rayData.hit.transform.gameObject.layer != 3)
                {

                    if (rayData.hit.rigidbody)
                    {
                        rayData.hit.rigidbody.AddForce(rayData.ray.direction * bulletForceMultiplier, ForceMode.Impulse);
                    }
                    else
                    {
                        Debug.Log("Does Not have rigidbody");
                    }

                    if (!rayData.hit.transform.parent && !rayData.hit.transform.TryGetComponent<EnemyBase>(out EnemyBase eb))
                    {
                        SpawnBulletHoleDecal(rayData);
                        GameObject hitFX = Instantiate(HitEffect);
                        hitFX.transform.position = rayData.hit.point;
                    }

                    if (rayData.hit.transform.parent)
                    {
                        if (rayData.hit.transform.parent.TryGetComponent<EnemyBase>(out EnemyBase eb2))
                        {
                            GameObject hitFX = Instantiate(HitEffect);
                            hitFX.transform.position = rayData.hit.point; 

                            GameObject hitFX2 = Instantiate(enemyHitEffect);
                            hitFX2.transform.position = rayData.hit.point;

                            Health EnemyHealth = rayData.hit.collider.transform.parent.GetComponentInChildren<Health>();
                            int damage = DamageValue;


                            if (rayData.hit.collider.TryGetComponent(out EnemyHurtbox eh))
                            {
                                if (eh.isHeadshot == true)
                                {
                                    damage *= headShotMultiplier;
                                }

                            }
                            EnemyHealth.TakeDamage(damage, 0,gameObject);
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


    public void OnKill()
    {
        
        Debug.Log(" Enemy was Killed.");
        if (shotgun.currentKillstoRecharge < shotgun.RequiredKillsToRecharge)
        {
            shotgun.currentKillstoRecharge++;
        }

        if (shotgun.currentKillstoRecharge >= shotgun.RequiredKillsToRecharge)
        {
            if (shotgun.grenadeAmmo < shotgun.grenadeAmmoMax)
                shotgun.grenadeAmmo++;
            shotgun.currentKillstoRecharge = 0;
            Debug.Log("grenade Gained");
        }

    }

    public virtual void EngageAltFire()
    {
        //altFire Logic

    }

    public void SpawnBulletHoleDecal(RayData rayData)
    {
        GameObject Decal = Instantiate(BulletHitDecal);
        Decal.transform.position = rayData.hit.point;
        Vector3 pos = Decal.transform.position;
        Decal.transform.LookAt(pos + rayData.hit.normal, Vector3.up);
        Decal.transform.position += -rayData.hit.normal;
        Debug.Log("ray hit normal: " + rayData.hit.normal);
    }


    public virtual void ManualReload()
    {
        if (reloading == false && canPressAltFire == true)//verifies that im not already altfiring for situations like fanFire 
        {
            canFire = false;
            StartCoroutine(Reload());
        }
        else if (reloading == true)
        {
            Debug.Log(" Already Reloading ");
        }
        else
        {
            Debug.Log(" altfire active cannot reload");
        }
    }

    virtual public RayData RayCastAndGenCameraRayData()
    {
        Ray cameraRay = new Ray();

        RaycastHit cameraHit;

        cameraRay.origin = camRef.ScreenToWorldPoint(Vector3.zero);

        cameraRay.direction = camRef.transform.forward;

        Physics.Raycast(cameraRay, out cameraHit, Mathf.Infinity);

        return new RayData { ray = cameraRay, hit = cameraHit };
    }

    virtual public RayData RayCastAndGenGunRayData(Transform muzzle)
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

    public virtual IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        Debug.Log("Reloading...");
        canFire = true;
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
