using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float lookSens;
    [SerializeField] float defaultFOV = 90.0f;
    
    float xRotation = 0.0f;

    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] Transform camTransform;
    private Camera cam;
    [SerializeField] PlayerInputActions playerInputActions;
    [SerializeField] CharacterController characterController;
    [SerializeField] float xClamp;
    
    void Start()
    {
        InitializeMouseLook();
    }

    void Update()
    {
        float camRotationX;
        float currRotationX;
    }

    void InitializeMouseLook()
    {
        cam = camTransform.GetComponent<Camera>();
        cam.fieldOfView = defaultFOV;

        if (playerInputActions == null) playerInputActions = new PlayerInputActions();

        
    }

    float GetMouseSense()
    {
        return lookSens;
    }

    void SetMouseSense(float sense)
    {
        lookSens = sense;
    }
}
