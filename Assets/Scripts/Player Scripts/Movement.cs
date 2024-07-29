using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float maxSpeed = 10.0f;
    [SerializeField] float airDrag = 0.5f;
    [SerializeField] float groundDrag = 2.5f;
    [SerializeField] int baseMomentum = 1;
    [SerializeField] int encouragedMomentum = 1;
    [SerializeField] LayerMask isGround;


    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] PlayerInputActions playerInputActions;

    //Local Variables
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 momentum = Vector3.zero;
    [SerializeField] float lowMomentumThreshold = 100f;
    private bool grounded = true;
    private float currDrag;
    private float baseMomentumRatio;

    void Start()
    {
        InitializeMovement();
    }

    public void UpdateMovement()
    {
        //Assign currDrag to either the ground drag ammount or air drag ammount
        currDrag = grounded ? groundDrag : airDrag;
        currDrag *= 0.01f * Time.deltaTime;

        baseMomentumRatio = (float)baseMomentum / (float)(baseMomentum + encouragedMomentum);

        MovePlayer();
    }

    void MovePlayer()
    { 
        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        //When no key is pressed
        if (moveInput.magnitude <= 0.001f)
        {
            currDrag *= 10.0f;
        }

        //When Input and we have no momentum
        else
        {
            //Implement momentum ratio where the higher "encouragedMomentum" is in "baseMomentumRatio", the more effort it takes to change the current momentum direction
            momentum += moveDirection * moveSpeed * baseMomentumRatio;
            float encouragedAmount = Vector3.Dot(moveDirection, momentum.normalized) * (1 - baseMomentumRatio);
            momentum += Mathf.Clamp(encouragedAmount,0, 1) * moveSpeed * moveDirection;
            momentum = Vector3.ClampMagnitude(momentum, maxSpeed);
        }

        //Multiply momentum by correct drag type
        currDrag = 1 - currDrag;
        momentum *= currDrag;

        //Finally assign momentum to player
        transform.position += momentum;
    }

    void InitializeMovement()
    {
        if (playerInputActions == null) playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.started += Jump_Press;
    }

    private void Jump_Press(InputAction.CallbackContext context)
    {
        if (grounded)
        {
            Debug.Log("Jump");


        }
    }

    private void OnCollisionStay(Collision other)
    {
        int layer = other.gameObject.layer;
        
    }
}
