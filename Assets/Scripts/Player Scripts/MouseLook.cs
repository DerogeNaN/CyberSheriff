using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MouseLook : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] float mouseSens;
    [SerializeField] float defaultFOV = 90.0f;

    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] Camera cam;
    [SerializeField] PlayerInputActions playerInputActions;
    [SerializeField] float xClamp = 180.0f;

    public GameObject RevCam;
    public float ShakeMultiplier = 0.5f;

    //Local Variables:
    float camRotationX;
    float currRotationX;
    Vector2 mouseInput;

    void Start()
    {
        InitializeMouseLook();
    }

    void InitializeMouseLook()
    {
        LockCursor();

        //cam.GetComponent<Camera>().fieldOfView = defaultFOV;
        cam.fieldOfView = defaultFOV;

        if (playerInputActions == null) playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public void UpdateMouse()
    {
        mouseInput = playerInputActions.Player.Look.ReadValue<Vector2>() * mouseSens * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseInput.x);

        camRotationX = -mouseInput.y;
        currRotationX = cam.transform.localEulerAngles.x;
        currRotationX += camRotationX;

        if (currRotationX > xClamp) currRotationX -= 360.0f;                        //Stops player camera from looking more than "xClamp" degrees up or down.
        currRotationX = Mathf.Clamp(currRotationX, -89.0f, 89.0f);

        cam.transform.localEulerAngles = new Vector3(currRotationX, 0.0f + RevCam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z + RevCam.transform.localEulerAngles.z);    //Assign new mouse input value to camera transform

    }

    float GetMouseSense()
    {
        return mouseSens;
    }

    void SetMouseSense(float sense)
    {
        mouseSens = sense;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
