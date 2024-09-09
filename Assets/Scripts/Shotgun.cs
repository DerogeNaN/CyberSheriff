using UnityEngine;
using UnityEngine.AI;


public class Shotgun : RangedWeapon
{
  
    // Update is called once per frame
    void Update()
    {

    }

    public override void EngagePrimaryFire()
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
                    if (!rayData.hit.transform.GetComponent<NavMeshAgent>()) //AND it isn't an enemy
                    {
                        GameObject Decal = Instantiate(BulletHitDecal);
                        Decal.transform.parent = rayData.hit.transform;
                        Decal.transform.position = rayData.hit.point;
                    }
                    //..It isn't the player but it is an enemy...?
                    GameObject hitFX = Instantiate(HitEffect);
                    hitFX.transform.position = rayData.hit.point;
                    //Destroy(hitFX, 5);
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
            StartCoroutine(Wait(shotGapTime));
        }
        else if (currentBullets <= 0 && reloading == false)
        {
            canFire = false;
            StartCoroutine(Reload());
        }
    }


    public override void EngageAltFire()
    {
        ////altFire Logic
        //if (currentBullets > 0)
        //    StartCoroutine(FanFire());
        //else if (currentBullets <= 0 && reloading == false)
        //{
        //    Debug.Log("out off Bullets");
        //    canFire = false;
        //    StartCoroutine(Reload());
        //}
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
    public override  void OnAltFireStay()
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
        Debug.Log("end Primary Fire");
    }

    //active on Alt-fire End
    public override  void OnAltFireEnd()
    {
        shouldShootAlt = false;
        Debug.Log("end alt fire");
    }
}
