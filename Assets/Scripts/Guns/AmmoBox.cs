using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AmmoBox : MonoBehaviour
{
    // Start is called before the first frame update
    public enum AmmoType
    {
        shotgun,
        revolver,
        grenade,
        ShotAndRev,
        Health,
    }

    [SerializeField]
    public AmmoType ammoType;

    [SerializeField]
    int ammoGiven = 25;

    WeaponManagement weaponManagement;

    [SerializeField]
    float timeSinceActive;

    [SerializeField]
    float respawnTime;

    [SerializeField]
    MeshRenderer mesh;

    [SerializeField]
    bool active;

    [SerializeField]
    PlayerHealth health;

    //public string 
    [SerializeField]
    Transform childOfChild;


    void Start()
    {
        weaponManagement = FindObjectOfType<WeaponManagement>();
        health = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active == false)
            if (timeSinceActive < respawnTime)
            {
                timeSinceActive += Time.deltaTime;
                if (timeSinceActive >= respawnTime)
                {
                    active = true;
                    timeSinceActive = 0;
                }
            }
        mesh.enabled = active;

        if (childOfChild != null) childOfChild.gameObject.SetActive(active);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (active)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (ammoType == AmmoType.shotgun)
                {
                    Debug.Log("Ammo shotgun Given");
                    weaponManagement.shotgunRef.CurrentReserveAmmo = Mathf.Clamp(ammoGiven, 0, weaponManagement.shotgunRef.ReserveAmmoCap);
                    active = false;
                }

                if (ammoType == AmmoType.revolver)
                {
                    Debug.Log("Ammo Revolver Given");
                    weaponManagement.revolverRef.CurrentReserveAmmo = Mathf.Clamp(ammoGiven, 0, weaponManagement.revolverRef.ReserveAmmoCap);
                    active = false;
                }

                if (ammoType == AmmoType.ShotAndRev)
                {
                    Debug.Log("BothGiven");
                    weaponManagement.revolverRef.CurrentReserveAmmo = Mathf.Clamp(ammoGiven, 0, weaponManagement.revolverRef.ReserveAmmoCap);
                    weaponManagement.shotgunRef.CurrentReserveAmmo = Mathf.Clamp(ammoGiven / 4, 0, weaponManagement.shotgunRef.ReserveAmmoCap);
                    active = false;
                }

                if (ammoType == AmmoType.Health)
                {
                    if(!(health.health >=100))
                    {
                        health.health += ammoGiven;
                        active = false;
                    }
                    
                }
            }
        }
    }
}
