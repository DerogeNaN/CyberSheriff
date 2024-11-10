using System.Collections;
using UnityEngine;
using Unity.Mathematics;

public class Revolver : RangedWeapon
{
    [Header("Other Values")]
    [SerializeField] float spreadMultiplier = 0.5f;
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

        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        //Debug.Log("Reloading...");
        canFire = true;
        if (currentBullets != BulletsPerClip && CurrentReserveAmmo > BulletsPerClip)
        {
            CurrentReserveAmmo -= BulletsPerClip;
            currentBullets = BulletsPerClip;
        }
        else
        {
            currentBullets = CurrentReserveAmmo;
            CurrentReserveAmmo -= CurrentReserveAmmo;
        }
        reloading = false;
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
                CurrentlyHitting = rayData.hit.transform.gameObject;

                if (rayData.hit.transform.gameObject.layer != 3)
                {

                    if (rayData.hit.rigidbody)
                    {
                        rayData.hit.rigidbody.AddForce(rayData.ray.direction * bulletForceMultiplier, ForceMode.Impulse);
                    }
                    else
                    {
                        //Debug.Log("Does Not have rigidbody");
                    }


                    if (rayData.hit.transform.parent)
                    {
                        if (!(rayData.hit.transform.parent.gameObject.layer == LayerMask.NameToLayer("Enemy")))
                        {
                            SpawnBulletHoleDecal(rayData);
                            GameObject hitFX = Instantiate(HitEffect);
                            hitFX.transform.position = rayData.hit.point;
                        }
                    }
                    else
                    {
                        SpawnBulletHoleDecal(rayData);
                        GameObject hitFX = Instantiate(HitEffect);
                        hitFX.transform.position = rayData.hit.point;
                    }

                    if (rayData.hit.transform.parent)
                    {
                        if (rayData.hit.transform.parent.TryGetComponent(out EnemyBase eb2))
                        {
                            GameObject hitFX = Instantiate(enemyHitEffect);
                            hitFX.transform.position = rayData.hit.point;

                            GameObject hitFX2 = Instantiate(HitEffect);
                            hitFX2.transform.position = rayData.hit.point;

                            int damage = DamageValue;
                            Health EnemyHealth = rayData.hit.collider.transform.parent.GetComponentInChildren<Health>();
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
        RayData camRayData = RayCastAndGenCameraRayData();
        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 

        Vector3 barrelToLookPointDir = camRayData.hit.point - muzzle.transform.position;

        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        gunRay.direction = barrelToLookPointDir;
        gunRay.direction = gunRay.direction += (Vector3)UnityEngine.Random.insideUnitSphere * spreadMultiplier;

        Physics.Raycast(gunRay, out gunHit, camRef.farClipPlane);

        return new RayData { ray = gunRay, hit = gunHit };
    }


    public RayData AltRayCastAndGenGunRayData(Transform muzzle, out bool hitDetected)
    {
        Ray gunRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        gunRay.direction = muzzlePoint.transform.forward;
        gunRay.origin = muzzlePoint.position;

        RaycastHit gunHit;
        RayData camRayData = RayCastAndGenCameraRayData(out hitDetected);
        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 

        Vector3 barrelToLookPointDir = camRayData.hit.point - muzzle.transform.position;

        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to the barrel to look point direction 
        gunRay.direction = barrelToLookPointDir;
        gunRay.direction = gunRay.direction += (Vector3)UnityEngine.Random.insideUnitSphere * spreadMultiplier;

        Physics.Raycast(gunRay, out gunHit, camRef.farClipPlane);

        return new RayData { ray = gunRay, hit = gunHit };
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
