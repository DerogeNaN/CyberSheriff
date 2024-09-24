using UnityEngine;
using Unity.Mathematics;
using static RangedWeapon;


public class Shotgun : RangedWeapon
{
    [Header("I be riding shotgun underneath the hot sun...")]
    [SerializeField]
    int bulletsPerShot = 5;

    [SerializeField]
    float spreadMultiplier = 1;

    [Header("I'd Catch a grenade for ya(yeah,yeah,yeah...)")]
    [SerializeField]
    GameObject Grenade;

    //  [Tooltip("Seconds it takes for the grenade launcher to recharge")]
    // [SerializeField]
    float grenadeLauncherCooldown;


    [Tooltip("Seconds it takes for the grenade launcher to  completely Reload")]
    [SerializeField]
    float grenadeReloadTime;


    [Tooltip("Seconds it takes for the grenade launcher to Load the Next grenade")]
    [SerializeField]
    float grenadeLoadTime;

    [Tooltip("Time since last shot")]
    [SerializeField]
    float grenadeLauncherTimer;

    [Tooltip("Amount of force to be applied to the grenade rigidbody")]
    [SerializeField]
    float grenadeLaucherForceMultiplier;

    [Tooltip("Please dont mess with This(Grenade will be ready once grenade timer reach grenade cooldown  just reduce th cooldown if its to slow)")]
    [SerializeField]
    bool grenadeReady = false;

    [SerializeField]
    int grenadeAmmo = 3;

    [SerializeField]
    int grenadeAmmoMax = 3;

    public override void Start()
    {
        base.Start();
        grenadeLauncherCooldown = grenadeLoadTime;

    }

    public override void Update()
    {
        base.Update();

        if (grenadeAmmo <= 0)
        {
            grenadeLauncherCooldown = grenadeReloadTime;
        }

        if (!grenadeReady)
        {
            grenadeLauncherTimer += Time.deltaTime;
        }

        if (grenadeLauncherTimer >= grenadeLauncherCooldown && grenadeAmmo > 0)
        {
            grenadeReady = true;
            grenadeLauncherTimer = 0;
        }
        else if (grenadeLauncherTimer >= grenadeLauncherCooldown)
        {
            grenadeReady = true;
            grenadeLauncherTimer = 0;
            grenadeLauncherCooldown = grenadeLoadTime;
            grenadeAmmo = grenadeAmmoMax;
        }

    }

    private void OnDrawGizmos()
    {
        //RayData ray = base.RayCastAndGenGunRayData(muzzlePoint);
        //Gizmos.DrawLine(altMuzzlePoint.position, ray.ray.direction * grenadeLaucherForceMultiplier);

    }


    public override void EngagePrimaryFire()
    {
        //Primary Fire Logic
        int pellets = bulletsPerShot;
        if (currentBullets > 0)
        {
            currentBullets--;
            BulletFlash.Play();

            for (int i = 0; i < pellets; i++)
            {
                RayData rayData = RayCastAndGenGunRayData(muzzlePoint);
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
                            SpawnBulletHoleDecal(rayData);
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
        }
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
        //Debug.Log("Primary fire stay ");
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
        Debug.Log("end Primary Fire");
    }

    //active on Alt-fire End
    public override void OnAltFireEnd()
    {
        shouldShootAlt = false;
        Debug.Log("end alt fire");
    }
}
