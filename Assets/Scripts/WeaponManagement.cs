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
    public GameObject currentActiveWeapon;

    [SerializeField]
    PlayerInputActions playerInput;

    [Header("Current Active Weapon Attributes")]
    [SerializeField]
    public int CAWMaxAmmo;

    [SerializeField]
    public int CAWCurrentAmmo;

    [SerializeField]
    public string ammoText;



    // Start is called before the first frame update
    void Start()
    {
        currentActiveWeapon = weaponList[0];
        //currentActiveWeapon =  GetComponentsInChildren<RangedWeapon>()[1].gameObject;

        if (WeaponGripTransform.GetComponentInChildren<RangedWeapon>())
        {
            Debug.Log(WeaponGripTransform.GetComponentInChildren<RangedWeapon>().gameObject.name);
        }
        else
            //Debug.Log("no weapons found");
            playerInput = new PlayerInputActions();
        if (playerInput != null)
        {
            playerInput.Player.Enable();
            playerInput.Player.PrimaryFire.started += PrimaryFireWeaponBegin;
            playerInput.Player.PrimaryFire.canceled += PrimaryFireWeaponEnd;
            playerInput.Player.AltFire.started += AltFireWeaponBegin;
            playerInput.Player.AltFire.canceled += AltFireWeaponEnd;
            playerInput.Player.WeaponSwitch.started += ScrollSetWeapon;
            playerInput.Player.KeyWeaponSwitch1.started += keySetWeapon1;
            playerInput.Player.KeyWeaponSwitch2.started += keySetWeapon2;
        }

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

        //CAWMaxAmmo = currentActiveWeapon.GetComponent<RangedWeapon>().BulletsPerClip;
        //CAWCurrentAmmo = currentActiveWeapon.GetComponent<RangedWeapon>().currentBullets;
        //ammoText.text =  CAWMaxAmmo + " / " + CAWCurrentAmmo;

        if (playerInput == null)
        {
            Debug.Log("PlayerInput is Null now setting...");
            playerInput = new PlayerInputActions();
            playerInput.Player.Enable();
            playerInput.Player.PrimaryFire.started += PrimaryFireWeaponBegin;
            playerInput.Player.PrimaryFire.canceled += PrimaryFireWeaponEnd;
            playerInput.Player.AltFire.started += AltFireWeaponBegin;
            playerInput.Player.AltFire.canceled += AltFireWeaponEnd;
            playerInput.Player.WeaponSwitch.started += ScrollSetWeapon;
            playerInput.Player.KeyWeaponSwitch1.started += keySetWeapon1;
            playerInput.Player.KeyWeaponSwitch2.started += keySetWeapon2;
            playerInput.Player.Reload.started += ManualReload;
        }

        if (playerInput != null)
        {

            PrimaryFireStayCheck(playerInput.Player.PrimaryFire.inProgress);
            AltFireStayCheck(playerInput.Player.AltFire.inProgress);
        }
        else
        {
            Debug.Log("PlayerInput is Null");
        }
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
        if (!playerInput.Player.PrimaryFire.inProgress && !playerInput.Player.AltFire.inProgress)
        {
            if (currentActiveWeapon.GetComponent<RangedWeapon>())
                currentActiveWeapon.GetComponent<RangedWeapon>().ManualReload();
        }
        else
        {

            Debug.Log("Cannot reload at This Time");
        }
    }

    //maybe
    void WeaponListSet()
    {


    }

    void ScrollSetWeapon(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

        float CurrentWeapon = obj.ReadValue<Vector2>().y;

        if (CurrentWeapon > 0)
        {
            CurrentWeapon = 1;
        }
        else
            CurrentWeapon = 0;

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

        //set next to true 
        currentActiveWeapon.gameObject.SetActive(true);

        currentActiveWeapon.transform.position = WeaponGripTransform.position;
        currentActiveWeapon.transform.rotation = WeaponGripTransform.rotation;
    }

    void keySetWeapon1(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

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

        currentActiveWeapon.transform.position = WeaponGripTransform.position;
        currentActiveWeapon.transform.rotation = WeaponGripTransform.rotation;
    }


    void keySetWeapon2(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

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

        currentActiveWeapon.transform.position = WeaponGripTransform.position;
        currentActiveWeapon.transform.rotation = WeaponGripTransform.rotation;
    }

}
