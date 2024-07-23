using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponManagement : MonoBehaviour
{
    [SerializeField]
    List<GameObject> weaponPrefabList;

    [SerializeField]
    int weaponIterator = 0;

    [SerializeField]
    GameObject activeWeapon;

    [SerializeField]
    Transform WeaponGripTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (WeaponGripTransform.GetComponentInChildren<RangedWeapon>().gameObject)
            activeWeapon = WeaponGripTransform.GetComponentInChildren<RangedWeapon>().gameObject;
        else
            Debug.Log("no weapons found");
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeWeapon || activeWeapon.GetComponent<RangedWeapon>().gunVaribles.gunType != weaponPrefabList[weaponIterator].GetComponent<RangedWeapon>().gunVaribles.gunType)
        {
            SetWeapon(weaponPrefabList[weaponIterator]);
            Debug.Log("settingWeapon");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponIterator++;
            weaponIterator %= weaponPrefabList.Count - 1;
            if (weaponPrefabList[weaponIterator + 1])
            {
                weaponIterator++;
            }
           
        }
    }

    void SetWeapon(GameObject weap)
    {
        if (activeWeapon)
        {
            Destroy(activeWeapon);
        }
        activeWeapon = Instantiate(weap);
        activeWeapon.transform.position = WeaponGripTransform.position;
        activeWeapon.transform.rotation = WeaponGripTransform.rotation;
        activeWeapon.transform.parent = WeaponGripTransform;
    }
}
