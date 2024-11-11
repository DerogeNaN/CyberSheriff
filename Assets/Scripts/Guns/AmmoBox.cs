using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    // Start is called before the first frame update
    public enum AmmoType
    {
        shotgun,
        revolver,
        grenade,
        ShotandRev,
    }

    [SerializeField]
    public AmmoType ammoType;

    [SerializeField]
    int ammoGiven = 100;

    WeaponManagement weaponManagement;

    [SerializeField]
    float timeSinceActive;

    [SerializeField]
    float respawnTime;

    [SerializeField]
    MeshRenderer mesh;

    [SerializeField]
    bool active;

    void Start()
    {
        weaponManagement = FindObjectOfType<WeaponManagement>();
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

                if (ammoType == AmmoType.ShotandRev)
                {
                    Debug.Log("BothGiven");
                    weaponManagement.revolverRef.CurrentReserveAmmo = Mathf.Clamp(ammoGiven, 0, weaponManagement.revolverRef.ReserveAmmoCap);
                    weaponManagement.shotgunRef.CurrentReserveAmmo = Mathf.Clamp(ammoGiven / 4, 0, weaponManagement.shotgunRef.ReserveAmmoCap);
                    active = false;
                }
            }
        }
    }
}
