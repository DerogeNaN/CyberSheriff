using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

public class Revolver : RangedWeapon
{
    [Header("Other Values")]
    [SerializeField] float spreadMultiplier = 0.5f;
    [SerializeField] bool altShouldHeadShot = true;

    // Update is called once per frame
    public override void Update()
    {
        if (currentBullets <= 0 && CurrentReserveAmmo > 0 && reloading == false && shouldShootAlt == false)
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

    public override void EngagePrimaryFire()
    {
        if (currentBullets > 0)
        {
            animator.SetTrigger("ShootTrig");
            base.EngagePrimaryFire();
            SoundManager2.Instance.PlaySound("Revolver");
        }
    }

    public override IEnumerator Reload()
    {
        animator.SetTrigger("ReloadTrigger");
        SoundManager2.Instance.PlaySound("RevolverReload");
        yield return base.Reload();
    }

    public override void EngageAltFire()
    {
        //alt Fire Logic
        if (currentBullets > 0)
        {
            bool hit;
            RayData rayData = AltRayCastAndGenGunRayData(muzzlePoint, out hit);
            BulletFlash.Play();
            ParticleSystem ps = BulletFlash.gameObject.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            currentBullets--;
            animator.SetTrigger("ShootAltTrig");
            SoundManager2.Instance.PlaySound("Revolver");
            if (hit != false)
            {

                if (rayData.hits[0].transform.gameObject.layer != 3)
                {

                    if (rayData.hits[0].rigidbody)
                    {
                        rayData.hits[0].rigidbody.AddForce(rayData.ray.direction * bulletForceMultiplier, ForceMode.Impulse);
                    }
                    else
                    {
                        //Debug.Log("Does Not have rigidbody");
                    }


                    if (rayData.hits[0].transform.parent)
                    {
                        if (!(rayData.hits[0].transform.parent.gameObject.layer == LayerMask.NameToLayer("Enemy")))
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

                    if (rayData.hits[0].transform.parent)
                    {
                        if (rayData.hits[0].transform.parent.TryGetComponent(out EnemyBase eb2))
                        {
                            GameObject hitFX = Instantiate(enemyHitEffect);
                            hitFX.transform.position = rayData.hits[0].point;

                            GameObject hitFX2 = Instantiate(HitEffect);
                            hitFX2.transform.position = rayData.hits[0].point;
                            int damage = DamageValue;
                            Health EnemyHealth = rayData.hits[0].collider.transform.parent.GetComponentInChildren<Health>();

                            if (altShouldHeadShot)
                                if (rayData.hits[0].collider.TryGetComponent(out EnemyHurtbox eh))
                                {
                                    if (eh.isHeadshot == true)
                                    {
                                        damage *= headShotMultiplier;
                                    }

                                    GameObject marker = EnemyHealth.health - damage <= 0 ? KillHitMarker : HitMarker;
                                    if (currentMarker)
                                        currentMarker.gameObject.SetActive(false);
                                    currentMarker = marker;
                                    currentMarker.SetActive(true);
                                    StartCoroutine(TurnItOff());
                                }

                            EnemyHealth.TakeDamage(damage, 0, gameObject);
                        }
                    }
                }
            }
            canFire = false;
            StartCoroutine(Wait(AltshotGapTime));
        }
        else if (currentBullets <= 0 && CurrentReserveAmmo > 0 && reloading == false)
        {
            canFire = false;
            StartCoroutine(Reload());
        }

    }

    public RayData AltRayCastAndGenGunRayData(Transform muzzle)
    {
        Ray gunRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        gunRay.direction = muzzlePoint.transform.forward;
        gunRay.origin = muzzlePoint.position;

        RaycastHit gunHit;
        RayData camRayData = RayCastAndGenCameraRayData(false);

        RayData newData = new RayData { ray = gunRay, hits = new List<RaycastHit>() };


        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 

        Vector3 barrelToLookPointDir = camRayData.hits[0].point - muzzle.transform.position;

        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        gunRay.direction = barrelToLookPointDir;
        gunRay.direction = gunRay.direction += (Vector3)UnityEngine.Random.insideUnitSphere * spreadMultiplier;

        Physics.Raycast(gunRay, out gunHit, camRef.farClipPlane, ~LayerMask.NameToLayer("Ignore Raycast"));

        newData.hits.Add(gunHit);
        return newData;
    }


    public RayData AltRayCastAndGenGunRayData(Transform muzzle, out bool hitDetected)
    {
        Ray gunRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        gunRay.direction = muzzlePoint.transform.forward;
        gunRay.origin = muzzlePoint.position;

        RaycastHit gunHit;
        RayData camRayData = RayCastAndGenCameraRayData(out hitDetected, false);
        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 

        RayData newData = new RayData { ray = gunRay, hits = new List<RaycastHit>() };


        Vector3 barrelToLookPointDir = camRayData.hits[0].point - muzzle.transform.position;

        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        gunRay.direction = barrelToLookPointDir;
        gunRay.direction = gunRay.direction += (Vector3)UnityEngine.Random.insideUnitSphere * spreadMultiplier;

        Physics.Raycast(gunRay, out gunHit, camRef.farClipPlane);
        newData.hits.Add(gunHit);
        return newData;
    }

    //active on beginning of Primary fire Action
    public override void OnPrimaryFireBegin()
    {
        shouldShootPrimary = true;

    }

    //Active on Begining of alt-firing action
    public override void OnAltFireBegin()
    {
        shouldShootAlt = true;

    }

    //Active every interval of Primaryfire set in this script
    public override void OnPrimaryFireStay()
    {
        if (shouldShootPrimary)
        {

        }
    }

    //Active every interval  of altfire set in this script
    public override void OnAltFireStay()
    {
        if (shouldShootAlt)
        {

        }

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
