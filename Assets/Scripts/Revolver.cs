using System.Collections;
using UnityEngine;

public class Revolver : RangedWeapon
{
    public Animator animator;

    public override void EngagePrimaryFire()
    {
        animator.SetBool("ShootBool", true);
        base.EngagePrimaryFire();
    }

    public override IEnumerator Reload()
    {
        animator.SetBool("ShootBool", false);
        animator.SetBool("ReloadBool", true);

        //float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        //float reloadAnimationSpeed = animationLength / reloadTime;
        //
        //animator.GetCurrentAnimatorClipInfo(0).S = reloadAnimationSpeed;

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
        //altFire Logic
        if (currentBullets > 0)
        {
            StartCoroutine(FanFire());

        }

        else if (currentBullets <= 0 && reloading == false)
        {
            Debug.Log("out off Bullets");
            canFire = false;
            StartCoroutine(Reload());
        }
    }

    IEnumerator FanFire()
    {
        Debug.Log("Fire start");
        canPressAltFire = false;
        int BulletsAtTimeOfFiring = currentBullets;
        Debug.Log("Current Bullet Number:" + BulletsAtTimeOfFiring);
        for (int i = 0; i < BulletsAtTimeOfFiring; i++)
        {
            Debug.Log("FanFire Bullet Loosed");
            BulletFlash.Play();
            ParticleSystem ps = BulletFlash.gameObject.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            currentBullets--;

            RayData rayData = RayCastAndGenGunRayData(altMuzzlePoint);
            if (rayData.hit.point != null)
            {
                CurrentlyHitting = rayData.hit.transform.gameObject;

                // bullet Hole Decal Placement Logic 
                if (rayData.hit.transform.gameObject.layer != 3)
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

            canFire = false;
            yield return new WaitForSeconds(AltshotGapTime);
        }
        Debug.Log("Fan Fire end");
        canPressAltFire = true;
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
