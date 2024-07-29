using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponManagement : MonoBehaviour
{
    [SerializeField]
    List<RangedWeapon> weaponList;

    [SerializeField]
    int weaponIterator = 0;


    [SerializeField]
    Transform WeaponGripTransform;


    [SerializeField]
    RangedWeapon currentActiveWeapon;


    // Start is called before the first frame update
    void Start()
    {
        SetWeapon(weaponIterator);

        if (WeaponGripTransform.GetComponentInChildren<RangedWeapon>())
        {
            Debug.Log(WeaponGripTransform.GetComponentInChildren<RangedWeapon>().gameObject.name);
        }
        else
            Debug.Log("no weapons found");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponIterator++;

            if (weaponIterator < weaponList.Count)
            {
                Debug.Log("theres still weapons in this list ");
            }
            else
            {
                Debug.Log("at weapon List end");
                weaponIterator %= weaponList.Count;
                Debug.Log(weaponIterator);
            }

            SetWeapon(weaponIterator);
            Debug.Log(weaponIterator);
        }

        if (Input.GetMouseButton(0))
        {
            PrimaryFireWeapon();

        }

        if (Input.GetMouseButtonUp(0))
        {

            currentActiveWeapon.OnprimaryFireEnd();


        }

        if (Input.GetMouseButton(1))
        {
            SecondaryFireWeapon();

        }

        if (Input.GetMouseButtonUp(1))
        {
            currentActiveWeapon.OnAltFireEnd();
        }


    }
    void PrimaryFireWeapon()
    {
        currentActiveWeapon.OnPrimaryFireBegin();
    }


    void SecondaryFireWeapon()
    {
        currentActiveWeapon.OnAltFireBegin();
    }


    //maybe
    void WeaponListSet()
    {


    }

    void SetWeapon(int weaponIndex)
    {

        //set previous to false
        if (currentActiveWeapon)
            currentActiveWeapon.gameObject.SetActive(false);

        Debug.Log("weapon type " + weaponIndex);

        currentActiveWeapon = weaponList[weaponIndex];
        if (currentActiveWeapon)
        {
            Debug.Log("WeaponFound!!");
        }

        //set next to true 
        currentActiveWeapon.gameObject.SetActive(true);

        currentActiveWeapon.transform.position = WeaponGripTransform.position;
        currentActiveWeapon.transform.rotation = WeaponGripTransform.rotation;
        currentActiveWeapon.transform.parent = WeaponGripTransform;
    }
}
