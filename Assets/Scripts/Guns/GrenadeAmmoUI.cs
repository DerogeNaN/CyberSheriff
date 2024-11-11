using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class GrenadeAmmoUI : MonoBehaviour
{

    public GameObject AmmoHolderUI;

    [SerializeField]
    public List<Slider> grenadeLogos;

    public WeaponManagement weaponManagementScript;

    [SerializeField]
    int UIGrenadeAmmo;

    Shotgun shotgun;

    private void Awake()
    {
        for (int i = 0; i < weaponManagementScript.weaponList.Count; i++)
        {
            if (weaponManagementScript.weaponList[i].TryGetComponent(out shotgun))
                UIGrenadeAmmo = shotgun.grenadeAmmo;
            else
                UIGrenadeAmmo = 0;
        }
    }

    // Update is called once per frame 
    void Update()
    {
        float percentage = ((float)shotgun.currentKillsToRecharge / (float)shotgun.RequiredKillsToRecharge);

        UIGrenadeAmmo = shotgun.grenadeAmmo;


        //weaponManagementScript.
        for (int i = 0; i < grenadeLogos.Count; i++)
        {
            if (UIGrenadeAmmo < 1 && i == 0)
            {
                grenadeLogos[i].value = percentage;
            }
            else if (UIGrenadeAmmo == 1 && i == 1)
            {
                grenadeLogos[i - 1].value = 1;
                grenadeLogos[i].value = percentage;
            }
            else if (UIGrenadeAmmo == 2 && i == 2)
            {
                grenadeLogos[i - 1].value = 1;
                grenadeLogos[i].value = percentage;
            }
            else if (UIGrenadeAmmo == 3 && i == 2)
            {
                grenadeLogos[i].value = 1;
            }
            else if (UIGrenadeAmmo < 1 && i == 0)
            {
                grenadeLogos[i].value = 0;
            }
            else if (UIGrenadeAmmo < 2 && i == 1)
            {
                grenadeLogos[i].value = 0;
            }
            else if (UIGrenadeAmmo < 3 && i == 2)
            {
                grenadeLogos[i].value = 0;
            }
        }

    }



}
