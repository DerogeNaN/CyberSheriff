using UnityEngine;
using Unity.Mathematics;
using static RangedWeapon;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal.Internal;


public class Shotgun : RangedWeapon
{
    [Header("I be riding shotgun underneath the hot sun...")]
    [SerializeField]
    int bulletsPerShot = 5;

    [SerializeField]
    bool chargeExited = false;

    [SerializeField]
    float spreadMultiplier = 1;

    [SerializeField]
    float chargeTime = 2.0f;

    [SerializeField]
    float inputTime = 0;

    [SerializeField]
    bool charged = false;

    [Header("I'd Catch a grenade for ya(yeah,yeah,yeah...)")]
    [SerializeField]
    GameObject Grenade;

    [Tooltip("Amount of force to be applied to the grenade rigidbody")]
    [SerializeField]
    float grenadeLaucherForceMultiplier;

    [Tooltip("Please dont mess with This(Grenade will be ready once grenade timer reach grenade cooldown  just reduce th cooldown if its to slow)")]
    [SerializeField]
    public bool grenadeReady = false;

    [SerializeField]
    public int RequiredKillsToRecharge = 5;

    [SerializeField]
    public int currentKillstoRecharge = 0;

    [SerializeField]
    public int grenadeAmmo = 3;

    [SerializeField]
    public int grenadeAmmoMax = 3;

    public override void Update()
    {
        if (currentBullets <= 0 && reloading == false)
        {
            canFire = false;
            StartCoroutine(Reload());
        }

        if (currentBullets == BulletsPerClip)
        {

            if (shouldShootPrimary == true && waiting == false && reloading == false && canPressAltFire == true)
            {
                if (inputTime < chargeTime && charged == false)
                {
                    inputTime += Time.deltaTime;
                }

                if (inputTime >= chargeTime)
                {
                    charged = true;
                    inputTime = 0;

                }
            }
        }

        if (grenadeAmmo > 0)
        {
            grenadeReady = true;

        }

        if (shouldShootPrimary == false && chargeExited == true && waiting == false && reloading == false && canPressAltFire == true)
        {
            EngagePrimaryFire(charged);
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




    public void EngagePrimaryFire(bool charged)
    {
        chargeExited = false;
        inputTime = 0;
        int pellets;
        //Primary Fire Logic
        if (charged)
        {
            pellets = bulletsPerShot * 2;
        }
        else
        {
            pellets = bulletsPerShot;
        }

        if (currentBullets > 0)
        {
            if (charged)
            {
                currentBullets -= 2;
                BulletFlash.Play();
                BulletFlash.Play();
                this.charged = false;
            }
            else
            {
                currentBullets--;
                BulletFlash.Play();
            }

            for (int i = 0; i < pellets; i++)
            {
                RayData rayData = RayCastAndGenGunRayData(muzzlePoint);
                if (rayData.hit.point != null)
                {
                    CurrentlyHitting = rayData.hit.transform.gameObject;

                    if (rayData.hit.transform.gameObject.layer != 3) //If the thing hit isn't the player...
                    {
                        //..It isn't the player but it is an enemy...?
                       
                        if (rayData.hit.rigidbody)
                        {
                            rayData.hit.rigidbody.AddForce(rayData.ray.direction * bulletForceMultiplier, ForceMode.Impulse);
                        }
                        else
                        {
                            Debug.Log("Does Not have rigidbody");
                        }

                        if (!rayData.hit.transform.parent && !rayData.hit.transform.TryGetComponent(out EnemyBase eb)) //AND it isn't an enemy
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

                                Health EnemyHealth = rayData.hit.collider.transform.parent.GetComponentInChildren<Health>();
                                int damage = DamageValue;
                                if (rayData.hit.collider.TryGetComponent(out EnemyHurtbox eh))
                                {
                                    if (eh.isHeadshot == true)
                                    {
                                        Debug.Log("HeadShot");
                                        damage *= headShotMultiplier;
                                    }

                                }
                                EnemyHealth.TakeDamage(damage, 0,gameObject);
                            }
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



    public override RayData RayCastAndGenGunRayData(Transform muzzle)
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

    public override void EngageAltFire()
    {
        if (grenadeReady)
        {
            Rigidbody grenadeRB = Instantiate(Grenade).GetComponent<Rigidbody>();
            RayData ray = base.RayCastAndGenGunRayData(muzzlePoint);
            grenadeRB.gameObject.transform.position = muzzlePoint.position;
            grenadeRB.AddForce(ray.ray.direction * grenadeLaucherForceMultiplier, ForceMode.Impulse);
            grenadeRB.AddForce(Movement.playerMovement.momentum, ForceMode.Impulse);
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
        Debug.Log("Beginning primary Fire");
    }

    //Active every interval of Primaryfire set in this script
    public override void OnPrimaryFireStay()
    {
        shouldShootPrimary = true;
    }

    //Active every interval  of altfire set in this script
    public override void OnAltFireStay()
    {
        //if (shouldShootAlt)
        //{
        //    Debug.Log("alt fire stay ");
        //}

    }

    //active on primary fire End
    public override void OnprimaryFireEnd()
    {
        shouldShootPrimary = false;
        chargeExited = true;
        Debug.Log("end Primary Fire");
    }

    //active on Alt-fire End
    public override void OnAltFireEnd()
    {
        shouldShootAlt = false;
        Debug.Log("end alt fire");
    }
}
