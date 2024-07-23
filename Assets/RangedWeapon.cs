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
    public class GunVaribles
    {
        public GunType gunType;
        public Transform MuzzlePoint;
    }
    public GunVaribles gunVaribles;

    [SerializeField]
    GameObject CurrentlyHitting;

    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray();

        ray.direction = gunVaribles.MuzzlePoint.transform.forward;
        ray.origin = gunVaribles.MuzzlePoint.position;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            //Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
            lineRenderer.enabled = true;
            CurrentlyHitting = hit.transform.gameObject;
        }
        else
            lineRenderer.enabled = false;
         //   Debug.DrawRay(ray.origin, ray.direction, Color.yellow);


        //Debug.Log(hit.point);
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, hit.point);
    }
}
