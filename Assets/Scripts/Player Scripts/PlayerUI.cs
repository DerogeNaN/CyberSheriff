using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    
    int ammoCount;
    int ammoMax;
    [SerializeField]
    WeaponManagement weaponManagmentScript;

    [SerializeField]
    TextMeshProUGUI ammoText;
    [SerializeField]
    TextMeshProUGUI shadowAmmoText;

    // Start is called before the first frame update
    void Start()
    {
        weaponManagmentScript = FindObjectOfType<WeaponManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        ammoCount = weaponManagmentScript.currentActiveWeapon.GetComponent<RangedWeapon>().currentBullets;
        ammoMax = weaponManagmentScript.currentActiveWeapon.GetComponent<RangedWeapon>().BulletsPerClip;

        ammoText.text = ammoCount.ToString() + "/" + ammoMax.ToString();
        shadowAmmoText.text = ammoText.text;
    }
}
