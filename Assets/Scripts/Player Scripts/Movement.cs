using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 4.0f;
    [SerializeField] float maxSpeed = 0.015f;
    [SerializeField] float jumpHeight = 500.0f;
    [SerializeField] float gravityStrength = 9.8f;
    [SerializeField] float airDrag = 60f;
    [SerializeField] float groundDrag = 120f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask player;

    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] Transform rayCastPoint;
    [SerializeField] PlayerInputActions playerInputActions;

    [Space(10.0f)]
    [Header("Backend Variables (TEST)")]//Local Variables
    public Vector3 momentum = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    public bool isGrounded = true;
    private float currDrag;
    private float baseMomentumRatio;
    [SerializeField] private int baseMomentum = 1;
    [SerializeField] int encouragedMomentum = 35;
    float currentGravity = 0.0f;

    void Start()
    {
        InitializeMovement();
    }

    public void UpdateMovement()
    {
        //Assign currDrag to either the ground drag ammount or air drag ammount
        //currDrag = isGrounded ? groundDrag : airDrag;
        //currDrag = groundDrag;
        //encouragedMomentum = isGrounded ? 35 : 50;
        currDrag *= 0.01f * Time.deltaTime;

        baseMomentumRatio = (float)baseMomentum / (float)(baseMomentum + encouragedMomentum);

        //ApplyGravity();
        GroundCheck();
        MovePlayer();
    }

    void MovePlayer()
    { 
        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        //When no key is pressed
        if (moveInput.magnitude <= 0.001f)
        {
            //currDrag *= 10.0f;
        }

        //When Input and we have no momentum
        else
        {
            //Implement momentum ratio where the higher "encouragedMomentum" is in "baseMomentumRatio", the more effort it takes to change the current momentum direction
            momentum += moveDirection * moveSpeed * baseMomentumRatio;
            float encouragedAmount = Vector3.Dot(moveDirection, momentum.normalized) * (1 - baseMomentumRatio);
            momentum += Mathf.Clamp(encouragedAmount, 0, 1) * moveSpeed * moveDirection;
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
        playerInputActions.Player.Jump.performed += Jump_Performed;
    }

    private void Jump_Performed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            Debug.Log("Jump");
            momentum += transform.up * jumpHeight * Time.deltaTime;
            isGrounded = false;
        }
    }

    //void ApplyGravity()
    //{
    //    if (!isGrounded)
    //    {
    //        currentGravity += gravityStrength * Time.deltaTime;
    //    }
    //    else currentGravity = 0.0f;
    //}


    [SerializeField] float distanceToFloor = 0.2f;

    private void GroundCheck()
    {
        int layerMask = 1 << groundLayer;
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(rayCastPoint.position, Vector3.down, out hit, 200.0f, layerMask))
        {
            if (hit.distance <= distanceToFloor)
            {
                isGrounded = true;
                Debug.Log("There's Ground");
            }

            else
            {
                Debug.Log("No Ground");
                isGrounded = false;
                momentum.y += gravityStrength * Time.deltaTime;
                momentum.y = Mathf.Min(momentum.y, 0, hit.distance, distanceToFloor);
            }
            //if (Physics.Raycast(rayCastPoint.position, Vector3.down, out RaycastHit rayHit, 50, groundLayer))
            //{
            //    isGrounded = rayHit.distance <= distanceToFloor;
            //    
            //    if (!isGrounded)
            //    {
            //        momentum.y += currentGravity * Time.deltaTime;
            //        momentum.y = Mathf.Min(momentum.y, 0, rayHit.distance - distanceToFloor);
            //    }
            //}


        }
    }
}
