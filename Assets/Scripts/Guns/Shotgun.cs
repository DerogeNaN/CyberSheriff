using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using static RangedWeapon;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;



public class Shotgun : RangedWeapon
{
    [Header("Shotgun Settings")]
    [SerializeField]
    int bulletsPerShot = 5;

    [SerializeField]
    public int punchThroughAmount = 0;

    [SerializeField]
    float spreadMultiplier = 1;

    [Header("Grenade Settings")]
    [SerializeField]
    GameObject Grenade;

    [Tooltip("Amount of force to be applied to the grenade rigidbody")]
    [SerializeField]
    float grenadeLaucherForceMultiplier;

    [Tooltip("Please dont mess with This(Grenade will be ready so Long as the ammo is above zero)")]

    [SerializeField]
    float grenadeYEffectMult = 0.2f;

    [SerializeField]
    public bool grenadeReady = false;

    [SerializeField]
    public int RequiredKillsToRecharge = 5;

    [SerializeField]
    public int currentKillsToRecharge = 0;

    [SerializeField]
    public int grenadeAmmo = 3;

    [SerializeField]
    public int grenadeAmmoMax = 3;

   

    public override void Update()
    {

        //animator.SetBool("ChargedBool", charged);
        if (currentBullets <= 0 && CurrentReserveAmmo > 0 && reloading == false)
        {
            canFire = false;
            StartCoroutine(Reload());
        }

        if (grenadeAmmo > 0)
        {
            grenadeReady = true;

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

    private void OnDrawGizmos()
    {
        //RayData ray = base.RayCastAndGenGunRayData(muzzlePoint);
        //Gizmos.DrawLine(altMuzzlePoint.position, ray.ray.direction * grenadeLaucherForceMultiplier);
    }


    public override IEnumerator Reload()
    {
        animator.SetTrigger("ReloadTrigger");
        SoundManager2.Instance.PlaySound("ShotgunReload");
        yield return base.Reload();
    }

    public override void EngagePrimaryFire()
    {
        if (currentBullets > 0)
        {
            int pellets;
            //Primary Fire Logic

            animator.SetTrigger("ShootTrig");
            pellets = bulletsPerShot;

            currentBullets--;
            SoundManager2.Instance.PlaySound("ShotgunFire");
            BulletFlash.Play();


            for (int i = 0; i < pellets; i++)
            {
                bool hitDetected;
                RayData rayData = RayCastAndGenGunRayData(muzzlePoint, out hitDetected, shouldPunchThrough);
                if (hitDetected != false)
                {
                    for (int hit = 0; hit < rayData.hits.Count; hit++)
                    {
                        if (rayData.hits[hit].transform.gameObject.layer != 3)
                        {
                            if (rayData.hits[hit].rigidbody)
                            {
                                rayData.hits[hit].rigidbody.AddForce(rayData.ray.direction * bulletForceMultiplier, ForceMode.Impulse);
                            }
                            else
                            {
                                Debug.Log("Does Not have rigidbody");
                            }

                            if (rayData.hits[hit].transform.parent)
                            {
                                if (!(rayData.hits[hit].transform.parent.gameObject.layer == LayerMask.NameToLayer("Enemy")))
                                {
                                    SpawnBulletHoleDecal(rayData);
                                    GameObject hitFX = Instantiate(HitEffect);
                                    hitFX.transform.position = rayData.hits[hit].point;
                                }
                            }
                            else
                            {
                                SpawnBulletHoleDecal(rayData);
                                GameObject hitFX = Instantiate(HitEffect);
                                hitFX.transform.position = rayData.hits[hit].point;
                            }

                            if (rayData.hits[hit].transform.parent)
                            {
                                if (rayData.hits[hit].transform.parent.TryGetComponent(out EnemyBase eb2))
                                {
                                    GameObject hitFX = Instantiate(enemyHitEffect);
                                    hitFX.transform.position = rayData.hits[hit].point;

                                    GameObject hitFX2 = Instantiate(HitEffect);
                                    hitFX2.transform.position = rayData.hits[hit].point;

                                    Health EnemyHealth = rayData.hits[hit].collider.transform.parent.GetComponentInChildren<Health>();
                                    int damage = DamageValue;
                                    if (rayData.hits[hit].collider.TryGetComponent(out EnemyHurtbox eh))
                                    {
                                        if (eh.isHeadshot == true)
                                        {
                                            if (currentMarker)
                                                currentMarker.gameObject.SetActive(false);
                                            currentMarker = HitMarker;
                                            currentMarker.SetActive(true);
                                            damage *= headShotMultiplier;
                                        }
                                        else
                                        {
                                            if (currentMarker)
                                                currentMarker.gameObject.SetActive(false);
                                            currentMarker = HitMarker;
                                            currentMarker.SetActive(true);
                                            StartCoroutine(TurnItOff());
                                        }

                                    }
                                    EnemyHealth.TakeDamage(damage, 0, gameObject);
                                }
                            }
                        }
                        if (hit >= punchThroughAmount) break;
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



    public override RayData RayCastAndGenGunRayData(Transform muzzle, bool punchthrough)
    {
        Ray gunRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        gunRay.direction = muzzlePoint.transform.forward;
        gunRay.origin = muzzlePoint.position;

        RaycastHit gunHit;
        RayData camRayData = RayCastAndGenCameraRayData(punchthrough);
        RayData newData = new RayData { ray = gunRay, hits = new List<RaycastHit>() };
        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 

        Vector3 barrelToLookPointDir = camRayData.hits[0].point - muzzle.transform.position;

        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        gunRay.direction = barrelToLookPointDir;
        gunRay.direction = gunRay.direction += (Vector3)UnityEngine.Random.insideUnitSphere * spreadMultiplier;


        if (!punchthrough)
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

    public override RayData RayCastAndGenGunRayData(Transform muzzle, out bool hitDetected, bool punchthrough)
    {
        Ray gunRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        gunRay.direction = muzzlePoint.transform.forward;
        gunRay.origin = muzzlePoint.position;

        RaycastHit gunHit;
        RayData camRayData = RayCastAndGenCameraRayData(out hitDetected, punchthrough);
        RayData newData = new RayData { ray = gunRay, hits = new List<RaycastHit>() };

        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 


        Vector3 barrelToLookPointDir = camRayData.hits[0].point - muzzle.transform.position;

        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        gunRay.direction = barrelToLookPointDir;
        gunRay.direction = gunRay.direction += UnityEngine.Random.insideUnitSphere * spreadMultiplier;

        if (!punchthrough)
        {
            hitDetected = Physics.Raycast(gunRay, out gunHit, camRef.farClipPlane);
            newData.hits.Add(gunHit);
        }
        else
        {
            hitDetected = Physics.RaycastAll(gunRay, camRef.farClipPlane)[0].collider == null ? false : true;
            newData.hits = Physics.RaycastAll(gunRay, camRef.farClipPlane).ToList();
        }

        hitDetected = Physics.Raycast(gunRay, out gunHit, camRef.farClipPlane);

        return newData;
    }

    public override void EngageAltFire()
    {
        bool hit = false;
        if (grenadeReady)
        {
            animator.SetTrigger("ShootAltTrig");
            SoundManager2.Instance.PlaySound("ShotgunLauncherThunk");
            Rigidbody grenadeRB = Instantiate(Grenade).GetComponent<Rigidbody>();
            RayData Gunray = base.RayCastAndGenGunRayData(muzzlePoint, out hit, false);

            if (hit == false)
            {
                Gunray.ray.direction = Gunray.ray.origin + (RayCastAndGenCameraRayData(false).ray.direction * camRef.farClipPlane);
            }

            grenadeRB.gameObject.transform.position = muzzlePoint.position;
            grenadeRB.AddForce(Gunray.ray.direction * grenadeLaucherForceMultiplier, ForceMode.Impulse);
            grenadeRB.AddForce(new Vector3(Movement.playerMovement.velocity.x, Movement.playerMovement.velocity.y * grenadeYEffectMult, Movement.playerMovement.velocity.z), ForceMode.Impulse);
            grenadeReady = false;
            grenadeAmmo--;
            StartCoroutine(Wait(AltshotGapTime));
        }

    }

    //active on beginning of Primary fire Action
    public override void OnPrimaryFireBegin()
    {

    }

    //Active on Begining of alt-firing action
    public override void OnAltFireBegin()
    {
        shouldShootAlt = true;
    }

    //Active every interval of Primaryfire set in this script
    public override void OnPrimaryFireStay()
    {
        if (reloading == false && currentBullets > 0)
            shouldShootPrimary = true;
    }

    //Active every interval  of altfire set in this script
    public override void OnAltFireStay()
    {


    }

    //active on primary fire End
    public override void OnprimaryFireEnd()
    {
        shouldShootPrimary = false;
    }

    //active on Alt-fire End
    public override void OnAltFireEnd()
    {
        shouldShootAlt = false;
    }
}
