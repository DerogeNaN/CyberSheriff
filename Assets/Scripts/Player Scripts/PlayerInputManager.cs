using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    //Design friendly variables:
    [Header("Movement Stats")]
    public float moveSpeed = 5.0f;
    public float aimSens = 1.0f;

    //Component serialization
    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] Camera playerCam;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Rigidbody rb;
    
    private PlayerInputActions playerInputActions;
    private PlayerInputActions.GroundMovementActions groundControls;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.GroundMovement.Enable();

        playerInputActions.GroundMovement.Jump.started += Jump_Press;
        //playerInputActions.GroundMovement.Run.performed += Run_performed;
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = playerInputActions.GroundMovement.Run.ReadValue<Vector2>();
        rb.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * moveSpeed, ForceMode.Force);
    }

    private void Jump_Press(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    private void MouseLook(InputAction.CallbackContext context) 
    {
    
    }
}