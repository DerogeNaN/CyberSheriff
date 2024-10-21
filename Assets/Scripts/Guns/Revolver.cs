using System.Collections;
using UnityEngine;
using Unity.Mathematics;

public class Revolver : RangedWeapon
{
    public Animator animator;

    [Header("Other Values")]
    [SerializeField] float spreadMultiplier = 0.5f;
    public override void EngagePrimaryFire()
    {
        //make sure on  Kill isnt all ready an event;

        animator.SetBool("ShootBool", true);
        base.EngagePrimaryFire();
    }

    public override IEnumerator Reload()
    {
        animator.SetBool("ShootBool", false);
        animator.SetBool("ReloadBool", true);

        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        Debug.Log("Reloading...");
        canFire = true;
        if (currentBullets != BulletsPerClip)
        {
            currentBullets = BulletsPerClip;
        }
        reloading = false;
        animator.SetBool("ReloadBool", false);
    }

    public override void EngageAltFire()
    {
        //alt Fire Logic
        if (currentBullets > 0)
        {
            RayData rayData = AltRayCastAndGenGunRayData(muzzlePoint);
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
                            GameObject hitFX = Instantiate(enemyHitEffect);
                            hitFX.transform.position = rayData.hit.point;

                            GameObject hitFX2 = Instantiate(HitEffect);
                            hitFX2.transform.position = rayData.hit.point;

                            int damage = DamageValue;
                            Health EnemyHealth = rayData.hit.collider.transform.parent.GetComponentInChildren<Health>();
                            EnemyHealth.TakeDamage(damage, 0,gameObject);
                        }
                    }
                }
            }
            canFire = false;
            StartCoroutine(Wait(AltshotGapTime));
        }
        else if (currentBullets <= 0 && reloading == false)
        {
            canFire = false;
            StartCoroutine(base.Reload());
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

        Physics.Raycast(gunRay, out gunHit, Mathf.Infinity);

        return new RayData { ray = gunRay, hit = gunHit };
    }

    //active on beginning of Primary fire Action
    public override void OnPrimaryFireBegin()
    {
        shouldShootPrimary = true;
        Debug.Log("Beginning primary Fire");
    }

    //Active on Begining of alt-firing action
    public override void OnAltFireBegin()
    {
        shouldShootAlt = true;
        Debug.Log("Beginning primary Fire");
    }

    //Active every interval of Primaryfire set in this script
    public override void OnPrimaryFireStay()
    {
        if (shouldShootPrimary)
        {
            Debug.Log("Primary fire stay ");
        }
    }

    //Active every interval  of altfire set in this script
    public override void OnAltFireStay()
    {
        if (shouldShootAlt)
        {
            Debug.Log("alt fire stay ");
        }

    }

    //active on primary fire End
    public override void OnprimaryFireEnd()
    {
        shouldShootPrimary = false;
        animator.SetBool("ShootBool", false);
        Debug.Log("end Primary Fire");
    }

    //active on Alt-fire End
    public override void OnAltFireEnd()
    {
        shouldShootAlt = false;
        Debug.Log("end alt fire");
    }

}
