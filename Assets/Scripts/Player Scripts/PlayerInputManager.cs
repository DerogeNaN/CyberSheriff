using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    //Component serialization
    [Header("Player Scripts")]
    //[SerializeField] Movement movementScript;
    [SerializeField] MouseLook mouseLookScript;
    [SerializeField] PlayerMovement movementScript;
    [SerializeField] PlayerInputActions playerInputActions;

    private void Awake()
    {
        //InitializePlayerInputManager();
    }

    private void FixedUpdate()
    {
        //movementScript.UpdateMovement();
    }

    private void Update()
    {
        mouseLookScript.UpdateMouse();
        movementScript.UpdateMovement();
    }

    void InitializePlayerInputManager()
    {
        if (playerInputActions == null) playerInputActions = new PlayerInputActions();
        
        playerInputActions.Player.Enable();
    }
}