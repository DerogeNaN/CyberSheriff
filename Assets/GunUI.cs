using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour
{

    [SerializeField]
    WeaponManagement weaponManagement;
    [SerializeField]
    Image revActiveAmmo;
    [SerializeField]
    Image revActiveNoAmmo;

    [SerializeField]
    Image revDisabledAmmo;

    [SerializeField]
    Image revDisabledNoAmmo;

    [SerializeField]
    Image shotActiveAmmo;

    [SerializeField]
    Image shotActiveNoAmmo;

    [SerializeField]
    Image shotDisabledAmmo;

    [SerializeField]
    Image shotDisabledNoAmmo;


    [Header("Do Not Touch These")]
    [SerializeField]
    Image currentActiveImage;

    [SerializeField]
    Image currentDisabledImage;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (weaponManagement != null)
        {

            if (weaponManagement.currentActiveWeapon.GetComponent<Revolver>())
            {
                if (weaponManagement.currentActiveWeapon.GetComponent<Revolver>().CurrentReserveAmmo <= 0 && weaponManagement.currentActiveWeapon.GetComponent<Revolver>().currentBullets <= 0)
                {
                    if (currentActiveImage)
                        currentActiveImage.gameObject.SetActive(false);
                    currentActiveImage = revActiveNoAmmo;
                    currentActiveImage.gameObject.SetActive(true);

                }
                else
                {
                    if (currentActiveImage)
                        currentActiveImage.gameObject.SetActive(false);
                    currentActiveImage = revActiveAmmo;
                    currentActiveImage.gameObject.SetActive(true);
                }
            }

            if (weaponManagement.currentActiveWeapon.GetComponent<Shotgun>())
            {
                if (weaponManagement.currentActiveWeapon.GetComponent<Shotgun>().CurrentReserveAmmo <= 0 && weaponManagement.currentActiveWeapon.GetComponent<Shotgun>().currentBullets <= 0)
                {

                    if (currentActiveImage)
                        currentActiveImage.gameObject.SetActive(false);
                    currentActiveImage = shotActiveNoAmmo;
                    currentActiveImage.gameObject.SetActive(true);
                }
                else
                {
                    if (currentActiveImage)
                        currentActiveImage.gameObject.SetActive(false);
                    currentActiveImage = shotActiveAmmo;
                    currentActiveImage.gameObject.SetActive(true);
                }
            }

            if (!weaponManagement.currentActiveWeapon.GetComponent<Revolver>())
            {
                if (weaponManagement.weaponList[0].GetComponent<Revolver>().CurrentReserveAmmo <= 0 && weaponManagement.weaponList[0].GetComponent<Revolver>().currentBullets <= 0)
                {
                    if (currentDisabledImage)
                        currentDisabledImage.gameObject.SetActive(false);
                    currentDisabledImage = revDisabledNoAmmo;
                    currentDisabledImage.gameObject.SetActive(true);

                }
                else
                {
                    if (currentDisabledImage)
                        currentDisabledImage.gameObject.SetActive(false);
                    currentDisabledImage = revDisabledAmmo;
                    currentDisabledImage.gameObject.SetActive(true);
                }
            }

            if (!weaponManagement.currentActiveWeapon.GetComponent<Shotgun>())
            {
                if (weaponManagement.weaponList[1].GetComponent<Shotgun>().CurrentReserveAmmo <= 0 && weaponManagement.weaponList[0].GetComponent<Shotgun>().currentBullets <= 0)
                {
                    Debug.Log("Why");
                    if (currentDisabledImage)
                        currentDisabledImage.gameObject.SetActive(false);
                    currentDisabledImage = shotDisabledNoAmmo;
                    currentDisabledImage.gameObject.SetActive(true);
                }
                else
                {
                    if (currentDisabledImage)
                        currentDisabledImage.gameObject.SetActive(false);
                    currentDisabledImage = shotDisabledAmmo;
                    currentDisabledImage.gameObject.SetActive(true);
                }
            }

        }

    }
}
