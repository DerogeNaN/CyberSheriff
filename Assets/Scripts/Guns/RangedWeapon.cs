using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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

    [SerializeField]
    public int CurrentReserveAmmo = 50;

    [SerializeField]
    public int ReserveAmmoCap = 50;

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

    [SerializeField]
    [Tooltip("Primary fire Only")]
    public bool shouldPunchThrough = false;

    [Header("Visual Effects")]
    [SerializeField]
    public VisualEffect BulletFlash;

    [SerializeField]
    public GameObject HitEffect;

    [SerializeField]
    public GameObject enemyHitEffect;

    [SerializeField]
    public GameObject BulletHitDecal;

    [SerializeField]
    public Animator animator;

    [Header("Scene Refrences")]
    [SerializeField]
    public Camera camRef;



    public struct RayData
    {
        public Ray ray;
        public List<RaycastHit> hits;
    }


    // Start is called before the first frame update
    public virtual void Awake()
    {
        camRef = FindAnyObjectByType<Camera>();
        currentBullets = BulletsPerClip;
        CurrentReserveAmmo = ReserveAmmoCap;

    }

    // Update is called once per frame
    public virtual void Update()
    {

        if (currentBullets <= 0 && CurrentReserveAmmo > 0 && reloading == false)
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



    //private void OnDrawGizmos()
    //{
    //    RayData rayData = RayCastAndGenGunRayData(muzzlePoint, true);
    //    RaycastHit[] rayhits = Physics.RaycastAll(rayData.ray.origin, rayData.ray.direction, camRef.farClipPlane);

    //    if (rayhits.Length > 1)
    //        for (int i = 0; i < rayhits.Length; i++)
    //        {
    //            if (i != rayhits.Length - 1)
    //            {
    //                Vector3 HitToHitDir = rayhits[i].point - rayhits[i + 1].point;
    //                HitToHitDir = math.normalize(HitToHitDir);

    //                Gizmos.color = Color.red;
    //                Gizmos.DrawRay(rayhits[i].point, HitToHitDir);
    //            }

    //            Debug.Log("Hit Object:" + rayhits[i].collider.name);
    //        }
    //    else Debug.Log("Empty List");
    //}



    public virtual void EngagePrimaryFire()
    {
        //Primary Fire Logic
        if (currentBullets > 0)
        {
            if (shouldPunchThrough) { }
            bool hit;
            RayData rayData = RayCastAndGenGunRayData(muzzlePoint, out hit, shouldPunchThrough);
            BulletFlash.Play();
            ParticleSystem ps = BulletFlash.gameObject.GetComponentInChildren<ParticleSystem>();
            ps.Play();

            currentBullets--;

            for (int i = 0; i < rayData.hits.Count; i++)
                if (hit != false)
                {
                    if (rayData.hits[i].transform.gameObject.layer != 3)
                    {

                        if (rayData.hits[i].rigidbody)
                        {
                            rayData.hits[i].rigidbody.AddForce(rayData.ray.direction * bulletForceMultiplier, ForceMode.Impulse);
                        }
                        else
                        {
                            //  Debug.Log("Does Not have rigidbody");
                        }

                        if (rayData.hits[i].transform.parent)
                        {
                            if (!(rayData.hits[i].transform.parent.gameObject.layer == LayerMask.NameToLayer("Enemy")))
                            {
                                SpawnBulletHoleDecal(rayData);
                                GameObject hitFX = Instantiate(HitEffect);
                                hitFX.transform.position = rayData.hits[0].point;
                            }
                        }
                        else
                        {
                            SpawnBulletHoleDecal(rayData);
                            GameObject hitFX = Instantiate(HitEffect);
                            hitFX.transform.position = rayData.hits[0].point;
                        }

                        if (rayData.hits[i].transform.parent)
                        {
                            if (rayData.hits[i].transform.parent.TryGetComponent<EnemyBase>(out EnemyBase eb2))
                            {
                                GameObject hitFX = Instantiate(HitEffect);
                                hitFX.transform.position = rayData.hits[i].point;

                                GameObject hitFX2 = Instantiate(enemyHitEffect);
                                hitFX2.transform.position = rayData.hits[i].point;

                                Health EnemyHealth = rayData.hits[i].transform.parent.GetComponent<Health>();
                                int damage = DamageValue;


                                if (rayData.hits[i].collider.TryGetComponent(out EnemyHurtbox eh))
                                {
                                    if (eh.isHeadshot == true)
                                    {
                                        damage *= headShotMultiplier;
                                    }

                                }
                                if (this is Revolver)
                                    EnemyHealth.TakeDamage(damage, 1, gameObject);
                                else EnemyHealth.TakeDamage(damage, 2, gameObject);
                            }
                        }
                    }
                }
            canFire = false;
            StartCoroutine(Wait(shotGapTime));
        }
        else if (currentBullets <= 0 && CurrentReserveAmmo > 0 && reloading == false)
        {
            canFire = false;
            StartCoroutine(Reload());
        }
    }


    public void OnKill()
    {

        // Debug.Log(" Enemy was Killed.");
        if (shotgun.currentKillsToRecharge < shotgun.RequiredKillsToRecharge)
        {
            shotgun.currentKillsToRecharge++;
        }

        if (shotgun.currentKillsToRecharge >= shotgun.RequiredKillsToRecharge)
        {
            if (shotgun.grenadeAmmo < shotgun.grenadeAmmoMax)
                shotgun.grenadeAmmo++;
            shotgun.currentKillsToRecharge = 0;
            //  Debug.Log("grenade Gained");
        }

    }

    public virtual void EngageAltFire()
    {
        //altFire Logic

    }

    public void SpawnBulletHoleDecal(RayData rayData)
    {
        GameObject Decal = Instantiate(BulletHitDecal);
        Decal.transform.position = rayData.hits[0].point;
        Vector3 pos = Decal.transform.position;
        Decal.transform.LookAt(pos + rayData.hits[0].normal, Vector3.up);
        Decal.transform.position += -rayData.hits[0].normal;
        //  Debug.Log("ray hit normal: " + rayData.hit.normal);
    }


    public virtual void ManualReload()
    {
        if (reloading == false && canPressAltFire == true && currentBullets != BulletsPerClip && CurrentReserveAmmo > 0)//verifies that im not already altfiring for situations like fanFire 
        {
            canFire = false;
            StartCoroutine(Reload());
        }
        else if (reloading == true)
        {
            //      Debug.Log(" Already Reloading ");
        }
        else
        {
            // Debug.Log(" altfire active cannot reload");
        }
    }


    virtual public RayData RayCastAndGenCameraRayData(bool PunchThrough)
    {

        Ray cameraRay = new Ray();
        cameraRay.origin = camRef.ScreenToWorldPoint(Vector3.zero);
        cameraRay.direction = camRef.transform.forward;

        RaycastHit cameraHit;

        RayData newData = new RayData { ray = cameraRay, hits = new List<RaycastHit>() };

        if (!PunchThrough)
        {
            Physics.Raycast(cameraRay, out cameraHit, camRef.farClipPlane);
            newData.hits.Add(cameraHit);
        }
        else
        {
            newData.hits = Physics.RaycastAll(cameraRay, camRef.farClipPlane).ToList();
        }

        return newData;
    }

    virtual public RayData RayCastAndGenCameraRayData(out bool hitDetected, bool PunchThrough)
    {
        Ray cameraRay = new Ray();
        cameraRay.origin = camRef.ScreenToWorldPoint(Vector3.zero);
        cameraRay.direction = camRef.transform.forward;

        RaycastHit cameraHit;

        RayData newData = new RayData { ray = cameraRay, hits = new List<RaycastHit>() };

        if (!PunchThrough)
        {
            hitDetected = Physics.Raycast(cameraRay, out cameraHit, camRef.farClipPlane);
            newData.hits.Add(cameraHit);
        }
        else
        {
            hitDetected = Physics.RaycastAll(cameraRay, camRef.farClipPlane)[0].collider == null ? false : true;
            newData.hits = Physics.RaycastAll(cameraRay, camRef.farClipPlane).ToList();
        }

        return newData;
    }

    virtual public RayData RayCastAndGenGunRayData(Transform muzzle, bool punchThrough)
    {
        Ray gunRay = new Ray();
        Vector3 barrelToLookPointDir;

        //we get the start and direction for our "Bullet" from our Gun Here
        gunRay.direction = muzzlePoint.transform.forward;
        gunRay.origin = muzzlePoint.position;

        RaycastHit gunHit;
        RayData camRayData = RayCastAndGenCameraRayData(punchThrough);
        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 
        barrelToLookPointDir = camRayData.hits[0].point - muzzle.transform.position;
        //set ray direction to the barrel to look point direction 
        barrelToLookPointDir = math.normalize(barrelToLookPointDir);
        gunRay.direction = barrelToLookPointDir;

        RayData newData = new RayData { ray = gunRay, hits = new List<RaycastHit>() };

        if (!punchThrough)
        {
            Physics.Raycast(gunRay, out gunHit, camRef.farClipPlane);
            newData.hits.Add(gunHit);
        }
        else
        {
            newData.hits = Physics.RaycastAll(gunRay, camRef.farClipPlane).ToList();
        }

        return newData;
    }

    virtual public RayData RayCastAndGenGunRayData(Transform muzzle, out bool hitDetected, bool punchThrough)
    {
        Ray gunRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        gunRay.direction = muzzlePoint.transform.forward;
        gunRay.origin = muzzlePoint.position;

        RaycastHit gunHit;
        RayData camRayData = RayCastAndGenCameraRayData(out hitDetected, punchThrough);
        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 

        Vector3 barrelToLookPointDir = camRayData.hits[0].point - muzzle.transform.position;

        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        gunRay.direction = barrelToLookPointDir;

        RayData newData = new RayData { ray = gunRay, hits = new List<RaycastHit>() };

        if (!punchThrough)
        {
            Physics.Raycast(gunRay, out gunHit, camRef.farClipPlane);
            newData.hits.Add(gunHit);
        }
        else
        {
            hitDetected = Physics.RaycastAll(gunRay, camRef.farClipPlane)[0].collider == null ? false : true;
            newData.hits = Physics.RaycastAll(gunRay, camRef.farClipPlane).ToList();
        }

        return newData;
    }

    //This coroutine  was made so the gun would wait for the shot gap time to pass before being able to fire again
    public IEnumerator Wait(float shotGapTime)
    {
        waiting = true;
        yield return new WaitForSeconds(shotGapTime);
        // Debug.Log("Waiting...");
        canFire = true;
        waiting = false;
    }

    public virtual IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        // Debug.Log("Reloading...");
        canFire = true;
        int bulletDifference;
        if (currentBullets != BulletsPerClip && CurrentReserveAmmo > BulletsPerClip)
        {
            bulletDifference = -(currentBullets - BulletsPerClip);

            CurrentReserveAmmo -= bulletDifference;
            currentBullets += bulletDifference;
        }
        else
        {
            currentBullets = CurrentReserveAmmo;
            CurrentReserveAmmo -= CurrentReserveAmmo;
        }
        reloading = false;
    }


    //active on beginning of Primary fire Action
    public virtual void OnPrimaryFireBegin()
    {
        shouldShootPrimary = true;
        //  Debug.Log("Beginning primary Fire");
    }

    //Active on Begining of alt-firing action
    public virtual void OnAltFireBegin()
    {
        shouldShootAlt = true;
        //  Debug.Log("Beginning primary Fire");
    }

    //Active every interval of Primaryfire set in this script
    public virtual void OnPrimaryFireStay()
    {
        if (shouldShootPrimary)
        {
            //   Debug.Log("Primary fire stay ");
        }
    }

    //Active every interval  of altfire set in this script
    public virtual void OnAltFireStay()
    {
        if (shouldShootAlt)
        {
            //  Debug.Log("alt fire stay ");
        }

    }

    //active on primary fire End
    public virtual void OnprimaryFireEnd()
    {
        shouldShootPrimary = false;
        //   Debug.Log("end Primary Fire");
    }

    //active on Alt-fire End
    public virtual void OnAltFireEnd()
    {
        shouldShootAlt = false;
        //  Debug.Log("end alt fire");
    }

}
