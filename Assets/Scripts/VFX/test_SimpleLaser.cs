using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLaserScript : MonoBehaviour
{
    public GameObject laserPrefab;
    public GameObject firePoint;

    private GameObject spawnedLaser;
    void Start()
    {
        spawnedLaser = Instantiate(laserPrefab, firePoint.transform) as GameObject;
        DisableLaser();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EnableLaser();
        }

        if(Input.GetMouseButton(0))
        {
            UpdateLaser();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DisableLaser();
        }
    }

    void EnableLaser()
    {
        spawnedLaser.SetActive(true);
    }

    void UpdateLaser()
    {
        if(firePoint !=null)
        {
            spawnedLaser.transform.position = firePoint.transform.position;
        }
    }

    void DisableLaser()
    {
        spawnedLaser.SetActive(false);
    }
}
