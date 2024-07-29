using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    //Component serialization
    [Header("Player Scripts")]
    [SerializeField] Movement movementScript;
    [SerializeField] MouseLook mouseLookScript;
    [SerializeField] PlayerInputActions playerInputActions;

    private void Awake()
    {
        InitializePlayerInputManager();
    }

    private void Update()
    {
        movementScript.UpdateMovement();
        mouseLookScript.UpdateMouse();
    }

    void InitializePlayerInputManager()
    {
        if (playerInputActions == null) playerInputActions = new PlayerInputActions();
        
        playerInputActions.Player.Enable();
    }
}