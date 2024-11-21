using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairSwitcher : MonoBehaviour
{
    [SerializeField] GameObject CrossHairRevolver;

    [SerializeField] GameObject CrossHairShotgun;

    [Header("Can't touch this")]
    [SerializeField]WeaponManagement current;

    // Start is called before the first frame update
    void Start()
    {
        current = FindObjectOfType<WeaponManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (current.currentActiveWeapon.GetComponent<Revolver>()) 
        {
            CrossHairRevolver.SetActive(true);
            CrossHairShotgun.SetActive(false);

        }

        if (current.currentActiveWeapon.GetComponent<Shotgun>())
        {
            CrossHairRevolver.SetActive(false);
            CrossHairShotgun.SetActive(true);
        }

    }
}
