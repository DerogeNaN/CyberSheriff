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
    [SerializeField] Movement movementScript;
    [SerializeField] PlayerInputActions playerInputActions;

    private void Awake()
    {
        //InitializePlayerInputManager();
    }

    private void Update()
    {
        //movementScript.UpdateMovement();
        mouseLookScript.UpdateMouse();
    }

    private void FixedUpdate()
    {
        movementScript.UpdateMovement();
    }

    void InitializePlayerInputManager()
    {
        if (playerInputActions == null) playerInputActions = new PlayerInputActions();
        
        playerInputActions.Player.Enable();
    }
}