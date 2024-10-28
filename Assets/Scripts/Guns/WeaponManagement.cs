using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeaponManagement : MonoBehaviour
{
    [SerializeField]
    List<GameObject> weaponList;

    [SerializeField]
    int weaponIterator = 0;

    [SerializeField]
    Transform WeaponGripTransform;
    [SerializeField]
    Transform BoomstickTransform;

    [SerializeField]
    public GameObject currentActiveWeapon;

    [Header("Current Active Weapon Attributes")]
    [SerializeField]
    public int CAWMaxAmmo;

    [SerializeField]
    public int CAWCurrentAmmo;

    [SerializeField]
    public string ammoText;

    bool start = false;



    // Start is called before the first frame update
    void Start()
    {
       
        //make sure on  Kill isnt all ready an event;
        Health.enemyKill += currentActiveWeapon.GetComponent<RangedWeapon>().OnKill;
    }

    private void OnDrawGizmos()
    {
        CAWMaxAmmo = currentActiveWeapon.GetComponent<RangedWeapon>().BulletsPerClip;
        CAWCurrentAmmo = currentActiveWeapon.GetComponent<RangedWeapon>().currentBullets;
        ammoText = CAWMaxAmmo + " / " + CAWCurrentAmmo;
    }

    // Update is called once per frame
    void Update()
    {

        if (start == false)
        {
            Movement.playerMovement.playerInputActions.Player.Enable();
            Movement.playerMovement.playerInputActions.Player.PrimaryFire.started += PrimaryFireWeaponBegin;
            Movement.playerMovement.playerInputActions.Player.PrimaryFire.canceled += PrimaryFireWeaponEnd;
            Movement.playerMovement.playerInputActions.Player.AltFire.started += AltFireWeaponBegin;
            Movement.playerMovement.playerInputActions.Player.AltFire.canceled += AltFireWeaponEnd;
            Movement.playerMovement.playerInputActions.Player.WeaponSwitch.started += ScrollSetWeapon;
            Movement.playerMovement.playerInputActions.Player.KeyWeaponSwitch1.started += keySetWeapon1;
            Movement.playerMovement.playerInputActions.Player.KeyWeaponSwitch2.started += keySetWeapon2;
            Movement.playerMovement.playerInputActions.Player.Reload.started += ManualReload;
            start = true;
        }

        PrimaryFireStayCheck(Movement.playerMovement.playerInputActions.Player.PrimaryFire.inProgress);
        AltFireStayCheck(Movement.playerMovement.playerInputActions.Player.AltFire.inProgress);

    }

    void PrimaryFireWeaponBegin(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (currentActiveWeapon.GetComponent<RangedWeapon>())
            currentActiveWeapon.GetComponent<RangedWeapon>().OnPrimaryFireBegin();
    }

    void PrimaryFireWeaponEnd(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (currentActiveWeapon.GetComponent<RangedWeapon>())
            currentActiveWeapon.GetComponent<RangedWeapon>().OnprimaryFireEnd();
    }

    void PrimaryFireStayCheck(bool inProgress)
    {
        if (inProgress)
        {
            if (currentActiveWeapon.GetComponent<RangedWeapon>())
                currentActiveWeapon.GetComponent<RangedWeapon>().OnPrimaryFireStay();
        }
    }

    void AltFireStayCheck(bool inProgress)
    {
        if (inProgress)
        {
            if (currentActiveWeapon.GetComponent<RangedWeapon>())
                currentActiveWeapon.GetComponent<RangedWeapon>().OnAltFireStay();
        }
    }


    void AltFireWeaponEnd(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (currentActiveWeapon.GetComponent<RangedWeapon>())
            currentActiveWeapon.GetComponent<RangedWeapon>().OnAltFireEnd();
    }


    void AltFireWeaponBegin(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (currentActiveWeapon.GetComponent<RangedWeapon>())
            currentActiveWeapon.GetComponent<RangedWeapon>().OnAltFireBegin();
    }

    void ManualReload(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!Movement.playerMovement.playerInputActions.Player.PrimaryFire.inProgress && !Movement.playerMovement.playerInputActions.Player.AltFire.inProgress)
        {
            if (currentActiveWeapon.GetComponent<RangedWeapon>())
                currentActiveWeapon.GetComponent<RangedWeapon>().ManualReload();
        }
        else
        {

            Debug.Log("Cannot reload at This Time");
        }
    }

    void ScrollSetWeapon(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //if (currentActiveWeapon.GetComponent<RangedWeapon>().reloading || currentActiveWeapon.GetComponent<RangedWeapon>().waiting)
        //{
            float CurrentWeapon = obj.ReadValue<Vector2>().y;

            if (CurrentWeapon > 0)
        {
            CurrentWeapon = 1;
            SoundManager2.Instance.PlaySound("WeaponSwap");
        }
        else
        {
            CurrentWeapon = 0;
            SoundManager2.Instance.PlaySound("WeaponSwap");
        }
                

            Debug.Log("Changing Weapon");
            Debug.Log("Mouse Wheel Value : " + CurrentWeapon);

            //set previous to false
            if (currentActiveWeapon)
                currentActiveWeapon.gameObject.SetActive(false);

            Debug.Log("weapon type " + CurrentWeapon);

            currentActiveWeapon = weaponList[(int)CurrentWeapon];
            if (currentActiveWeapon)
            {
                Debug.Log("WeaponFound!!");
            }

            if (currentActiveWeapon.GetComponent<RangedWeapon>().reloading)
            {
                currentActiveWeapon.GetComponent<RangedWeapon>().reloading = false;
            }

            if (currentActiveWeapon.GetComponent<RangedWeapon>().waiting)
            {
                currentActiveWeapon.GetComponent<RangedWeapon>().waiting = false;
                currentActiveWeapon.GetComponent<RangedWeapon>().canFire = true;
            }
            //set next to true 
            currentActiveWeapon.gameObject.SetActive(true);
        //}

    }

    void keySetWeapon1(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //if (currentActiveWeapon.GetComponent<RangedWeapon>().reloading || currentActiveWeapon.GetComponent<RangedWeapon>().waiting)
        //{
            //set previous to false
            if (currentActiveWeapon)
                currentActiveWeapon.gameObject.SetActive(false);

            Debug.Log("weapon type " + 0);

            currentActiveWeapon = weaponList[0];
            if (currentActiveWeapon)
            {
                Debug.Log("WeaponFound!!");
            }

            //set next to true 
            currentActiveWeapon.gameObject.SetActive(true);


            if (currentActiveWeapon.GetComponent<RangedWeapon>().reloading)
            {
                currentActiveWeapon.GetComponent<RangedWeapon>().reloading = false;
            }

            if (currentActiveWeapon.GetComponent<RangedWeapon>().waiting)
            {
                currentActiveWeapon.GetComponent<RangedWeapon>().waiting = false;
                currentActiveWeapon.GetComponent<RangedWeapon>().canFire = true;
            }

            currentActiveWeapon.transform.position = WeaponGripTransform.position;
            currentActiveWeapon.transform.rotation = WeaponGripTransform.rotation;
        //}
    }


    void keySetWeapon2(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //if (currentActiveWeapon.GetComponent<RangedWeapon>().reloading || currentActiveWeapon.GetComponent<RangedWeapon>().waiting)
        //{

            //set previous to false
            if (currentActiveWeapon)
                currentActiveWeapon.gameObject.SetActive(false);

            Debug.Log("weapon type " + 1);

            currentActiveWeapon = weaponList[1];
            if (currentActiveWeapon)
            {
                Debug.Log("WeaponFound!!");
            }


            //set next to true 
            currentActiveWeapon.gameObject.SetActive(true);

            if (currentActiveWeapon.GetComponent<RangedWeapon>().reloading)
            {
                currentActiveWeapon.GetComponent<RangedWeapon>().reloading = false;
            }

            if (currentActiveWeapon.GetComponent<RangedWeapon>().waiting)
            {
                currentActiveWeapon.GetComponent<RangedWeapon>().waiting = false;
                currentActiveWeapon.GetComponent<RangedWeapon>().canFire = true;
            }
            currentActiveWeapon.transform.position = BoomstickTransform.position;
            currentActiveWeapon.transform.rotation = BoomstickTransform.rotation;
        //}
    }

}
