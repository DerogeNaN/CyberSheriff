using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;


public class Shotgun : MonoBehaviour
{

    [Header("Gun Transforms")]

    [SerializeField]
    public Transform MuzzlePoint;

    [SerializeField]
    public Transform SecondaryMuzzlePoint;

    [Header("Gun Behaviour")]

    [SerializeField]
    int DamageValue = 25;

    [SerializeField]
    public GameObject CurrentlyHitting;

    LineRenderer cameraLineRenderer;

    [SerializeField]
    bool shouldDrawBulletTrail = false;

    [SerializeField]
    float bulletForceMultiplier = 1;

    [SerializeField]
    GameObject bulletTrailPrefab;

    [SerializeField]
    float shotGapTime = 1f;

    //ShotGun Varaibles
    [Header("ShotGun Variables")]
    [SerializeField]
    float Spread;

    [SerializeField]
    int BulletsPerShot = 8;

    [SerializeField]
    float timeTillBullet = 0f;

    [SerializeField]
    bool canFire;

    [SerializeField]
    Camera camRef;


    
    // Start is called before the first frame update
    void Start()
    {
        camRef = FindAnyObjectByType<Camera>();
        cameraLineRenderer = camRef.GetComponent<LineRenderer>();
       

    }

    // Update is called once per frame
    void Update()
    {
        Ray cameraRay = new Ray();
        RaycastHit cameraHit;

        //GUN camera Interactions here 

        cameraRay.origin = camRef.ScreenToWorldPoint(Vector3.zero);
        cameraRay.direction = camRef.transform.forward;

        Physics.Raycast(cameraRay, out cameraHit, Mathf.Infinity);

        //This is just so i can see the players line of sight for now
        if (cameraHit.point != null  && cameraLineRenderer)
        {
            cameraLineRenderer.SetPosition(0, cameraRay.origin);
            cameraLineRenderer.SetPosition(1, cameraHit.point);
        }
        else 
        {
            Debug.Log("Not Looking at anything");
        }

        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 
        Vector3 barrelToLookPointDir = cameraHit.point - MuzzlePoint.transform.position;
        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to where the players reticle currently is pointing 

        if (timeTillBullet < shotGapTime)
        {
            timeTillBullet += Time.deltaTime;
        }

        if (timeTillBullet >= shotGapTime)
        {
            Debug.Log("CanFire!");
            canFire = true;
        }

        //Bullet visual Logic 
        if (shouldDrawBulletTrail && canFire)
        {
            for (int i = 0; i < BulletsPerShot; i++)
            {
                Ray ray = new Ray();
                RaycastHit hit;
                ray.origin = MuzzlePoint.position;
                ray.direction = barrelToLookPointDir;
                ray.direction += (Vector3)UnityEngine.Random.insideUnitCircle * Spread;
                if (Physics.Raycast(ray, out hit, 20))
                {
                    GameObject bulletfab = Instantiate(bulletTrailPrefab);
                    CurrentlyHitting = hit.transform.gameObject;

                    if (hit.point != null)
                    {
                        bulletfab.GetComponent<LineRenderer>().SetPosition(0, ray.origin);
                        bulletfab.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                    }

                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(barrelToLookPointDir * bulletForceMultiplier, ForceMode.Impulse);
                    }
                    if (hit.collider.gameObject.GetComponent<Health>())
                    {
                        Debug.Log("Die");
                        hit.collider.gameObject.GetComponent<Health>().TakeDamage(DamageValue, 0);
                    }
                }
                canFire = false;
            }
        }
        //else

    }

    //active on beginning of Primary fire Action
    public void OnPrimaryFireBegin()
    {
        if (timeTillBullet > shotGapTime)
        {
            timeTillBullet = 0;
            shouldDrawBulletTrail = true;
            Debug.Log("Beginning primary Fire");
        }

    }

    //Active on Begining of alt-firing action
    public void OnAltFireBegin()
    {
        if (timeTillBullet > shotGapTime)
        {
            timeTillBullet = 0;
            shouldDrawBulletTrail = true;
            Debug.Log("Beginning Alt Fire");
        }
    }

    //Active every interval of Primaryfire set in this script
    public void OnPrimaryFireStay()
    {

    }

    //Active every interval  of altfire set in this script
    public void OnAltFireStay()
    {


    }

    //active on primary fire End
    public void OnprimaryFireEnd()
    {
        shouldDrawBulletTrail = false;
        Debug.Log("end Primary Fire");
    }

    //active on Alt-fire End
    public void OnAltFireEnd()
    {
        shouldDrawBulletTrail = false;
        Debug.Log("end alt fire");
    }

}
