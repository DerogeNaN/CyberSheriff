using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GrenadeAmmoUI : MonoBehaviour
{

    public GameObject AmmoHolderUI;

    [SerializeField]
    public List<Transform> grenadeLogos;

    public WeaponManagement weaponManagementScript;

    int UIGrenadeAmmo;

    Shotgun shotgun;

    private void Awake()
    {
        if (AmmoHolderUI != null)
            grenadeLogos = AmmoHolderUI.GetComponentsInChildren<Transform>().ToList();

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
        UIGrenadeAmmo = shotgun.grenadeAmmo;
        if (shotgun.isActiveAndEnabled)
        {
            for (int i = 0; i < grenadeLogos.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                if (UIGrenadeAmmo >= 1 && i == 1)
                {
                    grenadeLogos[i].gameObject.SetActive(true);

                }
                else if (UIGrenadeAmmo >= 2 && i == 2)
                {
                    grenadeLogos[i].gameObject.SetActive(true);
                }
                else if (UIGrenadeAmmo >= 3 && i == 3)
                {
                    grenadeLogos[i].gameObject.SetActive(true);
                }
                else
                {
                    grenadeLogos[i].gameObject.SetActive(false);
                }
            }
        }
    }



}
