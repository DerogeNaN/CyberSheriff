using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Revolver : MonoBehaviour
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
        Ray ray = new Ray();
        Ray cameraRay = new Ray();

        //we get the start and direction for our "Bullet" from our Gun Here
        ray.direction = MuzzlePoint.transform.forward;
        ray.origin = MuzzlePoint.position;

        RaycastHit hit = new RaycastHit();
        RaycastHit cameraHit;

        //GUN camera Interactions here 

        cameraRay.origin = camRef.ScreenToWorldPoint(Vector3.zero);
        cameraRay.direction = camRef.transform.forward;

        Physics.Raycast(cameraRay, out cameraHit, Mathf.Infinity);

        //This is just so i can see the players line of sight for now
        if (cameraHit.point != null && cameraLineRenderer)
        {
            cameraLineRenderer.SetPosition(0, cameraRay.origin);
            cameraLineRenderer.SetPosition(1, cameraHit.point);
        }


        //Here im getting the direction of a vector from the gun muzzle to reticle hit point 
        Vector3 barrelToLookPointDir = cameraHit.point - MuzzlePoint.transform.position;
        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to where the players reticle currently is pointing 
        ray.direction = barrelToLookPointDir;

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
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject bulletfab = Instantiate(bulletTrailPrefab);
                CurrentlyHitting = hit.transform.gameObject;

                if (hit.point != null)
                {
                    bulletfab.GetComponent<LineRenderer>().SetPosition(0, ray.origin);
                    bulletfab.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                }

                if (hit.rigidbody != null && hit.transform.root.GetComponent<Movement>() == false)
                {
                    Debug.Log("Root" + hit.rigidbody.transform.root);
                    Debug.Log("Impulse" + hit.rigidbody.name);
                    hit.rigidbody.AddForce(barrelToLookPointDir * bulletForceMultiplier, ForceMode.Impulse);
                }
                else 
                {
                    Debug.Log("part of The Player ");
                }


                if (hit.collider.gameObject.GetComponent<Health>())
                {
                    Debug.Log("Die");
                    hit.collider.gameObject.GetComponent<Health>().TakeDamage(DamageValue, 0);
                }
            }
            canFire = false;
        }
        //else

    }

    //active on beginning of Primary fire Action
    public void OnPrimaryFireBegin()
    {
        if (timeTillBullet > shotGapTime && canFire)
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
