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
    [SerializeField] public float mouseSens;

    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] GameObject cameraHolder;
    [SerializeField] PlayerInputActions playerInputActions;
    [SerializeField] float xClamp = 180.0f;

    //Local Variables:
    float camRotationX;
    float currRotationX;
    Vector2 mouseInput;

    void Start()
    {
        InitializeMouseLook();
        mouseSens = (PlayerPrefs.GetFloat("Sensitivity", 09.52f));
    }

    void InitializeMouseLook()
    {
        LockCursor();

        if (playerInputActions == null) playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public void UpdateMouse()
    {
        mouseInput = playerInputActions.Player.Look.ReadValue<Vector2>() * mouseSens * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseInput.x);

        camRotationX = -mouseInput.y;
        currRotationX = cameraHolder.transform.localEulerAngles.x;
        currRotationX += camRotationX;

        if (currRotationX > xClamp) currRotationX -= 360.0f;                        //Stops player camera from looking more than "xClamp" degrees up or down.
        currRotationX = Mathf.Clamp(currRotationX, -89.0f, 89.0f);

        cameraHolder.transform.localEulerAngles = new Vector3(currRotationX, 0.0f, cameraHolder.transform.localEulerAngles.z);    //Assign new mouse input value to camera transform

    }

    public float GetMouseSense()
    {
        return mouseSens;
    }

    //public void SetMouseSense(float sense)
    //{
    //    mouseSens = sense;
    //}

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
