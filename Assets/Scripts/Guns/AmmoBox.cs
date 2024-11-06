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
        grenade
    }

    [SerializeField]
    public AmmoType ammoType;

    [SerializeField]
    int ammoGiven = 100;

    [SerializeField]
    WeaponManagement weaponManagement;

    void Start()
    {
        weaponManagement = FindObjectOfType<WeaponManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) 
        {
            if (ammoType == AmmoType.shotgun)
            {
                Debug.Log("Ammo shotgun Given");
                weaponManagement.shotgunRef.CurrentReserveAmmo = Mathf.Clamp(ammoGiven,0, weaponManagement.shotgunRef.ReserveAmmoCap);
            }

            if (ammoType == AmmoType.revolver)
            {
                Debug.Log("Ammo Revolver Given");

                weaponManagement.revolverRef.CurrentReserveAmmo = Mathf.Clamp(ammoGiven, 0, weaponManagement.revolverRef.ReserveAmmoCap);
            }

            if (ammoType == AmmoType.grenade)
            {
                Debug.Log("Ammo Grenade Given");
            }
        }
    }
}
