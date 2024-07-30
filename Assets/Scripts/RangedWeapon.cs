using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;

public class RangedWeapon : MonoBehaviour
{


    public enum GunType
    {
        sidearm,
        revolver,
        rifle,
        shotgun,
    }

    [Serializable]
    public class GunDetails
    {
        public GunType gunType;
        public Transform MuzzlePoint;
        public Transform SecondaryMuzzlePoint;
    }
    public GunDetails gunDetails;

    [SerializeField]
    public GameObject CurrentlyHitting;

    LineRenderer lineRenderer;
    LineRenderer cameraLineRenderer;

    bool shouldDrawBulletTrail = false;

    [SerializeField]
    Camera camRef;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        camRef = FindAnyObjectByType<Camera>();
        cameraLineRenderer = camRef.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray();
        Ray cameraRay = new Ray();


        //we get the start and direction for our "Bullet" from our Gun Here
        ray.direction = gunDetails.MuzzlePoint.transform.forward;
        ray.origin = gunDetails.MuzzlePoint.position;

        RaycastHit hit;

        RaycastHit cameraHit;


        //GUN camera Interactions here 

        cameraRay.origin = camRef.ScreenToWorldPoint(Vector3.zero);
        cameraRay.direction = camRef.transform.forward;

        Physics.Raycast(cameraRay, out cameraHit, Mathf.Infinity);

        //This is just so i can see the players line of sight for now
        cameraLineRenderer.SetPosition(0, cameraRay.origin);
        cameraLineRenderer.SetPosition(1, cameraHit.point);

        //Here im getting the directionof of a gun muzzle to reticle hit point 
        Vector3 barrelToLookPointDir = cameraHit.point - gunDetails.MuzzlePoint.transform.position;
        barrelToLookPointDir = math.normalize(barrelToLookPointDir);

        //set ray direction to where the players reticle currently is pointing 
        ray.direction = barrelToLookPointDir;

        //Bullet visual Logic 
        if (shouldDrawBulletTrail)
        {
            if (Physics.Raycast(ray, out hit, 20))
            {
                lineRenderer.enabled = true;
                CurrentlyHitting = hit.transform.gameObject;

            }
            else
                lineRenderer.enabled = false;
            lineRenderer.SetPosition(0, ray.origin);
            if (hit.point != null)
            {
                lineRenderer.SetPosition(1, hit.point);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(barrelToLookPointDir, ForceMode.Impulse);

            }
        }
        else
            lineRenderer.enabled = false;


    }

    //active on beginning of Primary fire Action
    public void OnPrimaryFireBegin()
    {
        shouldDrawBulletTrail = true;
        Debug.Log("Beginning primary Fire");
    }

    //Active on Begining of alt-firing action
    public void OnAltFireBegin()
    {
        shouldDrawBulletTrail = true;
        Debug.Log("Beginnig alt Fire");
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
