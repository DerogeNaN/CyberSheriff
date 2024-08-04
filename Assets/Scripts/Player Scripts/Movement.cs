using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;


public class Movement : MonoBehaviour
{
    public static Movement playerMovement;

    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 4.0f;
    [SerializeField] float maxSpeed = 0.035f;
    [SerializeField] float maxPlayerInputSpeed = 0.015f;
    [SerializeField] float gravityStrength = -1.0f;
    [SerializeField] float airDrag = 60f;
    [SerializeField] float groundDrag = 120f;
    [SerializeField] float jumpStrength = 10f;
    [SerializeField] float jumpGraceLength = 0.1f; //Time allowed after being grounded for first jump


    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] PlayerInputActions playerInputActions;

    [Space(10.0f)]
    [Header("Backend Variables (TEST)")]//Local Variables
    public Vector3 momentum = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;

    public float currDrag;
    private float baseMomentumRatio;
    private float actualGravity = 0;
    public int jumpCount = 0;
    public bool isGrounded = true;
    public float lastGroundedTime = 0;
    public float jumpCooldown = 0.01f;
    public float lastJumpTime;
    [SerializeField] private int baseMomentum = 1;
    [SerializeField] int encouragedMomentum = 3;
    [SerializeField] int encouragedAirMomentum = 35;

    void Start()
    {
        playerMovement = this;
        InitialiseMovement();
    }

    public void UpdateMovement()
    {
        actualGravity = 0.1f * gravityStrength;

        //Assign currDrag to either the ground drag ammount or air drag ammount
        currDrag = isGrounded ? groundDrag : airDrag;
        //currDrag = groundDrag;
        //encouragedMomentum = isGrounded ? 35 : 50;
        currDrag *= 0.01f;

        baseMomentumRatio = (float)baseMomentum / (float)(baseMomentum + encouragedMomentum);

        MovePlayer();
    }

    void MovePlayer()
    { 
        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        //When no key is pressed
        //if (isGrounded && moveInput.magnitude <= 0.001f)
        //{
        //    currDrag = 0.01f;
        //}

        //When Input and we have no momentum
        //else
        {
            Vector3 playerWASDMomentum = Vector3.zero;
            //Implement momentum ratio where the higher "encouragedMomentum" is in "baseMomentumRatio", the more effort it takes to change the current momentum direction
            playerWASDMomentum += moveDirection * moveSpeed * baseMomentumRatio;
            float encouragedAmount = Vector3.Dot(moveDirection, momentum.normalized) * (1 - baseMomentumRatio);
            playerWASDMomentum += Mathf.Clamp(encouragedAmount, 0, 1) * moveSpeed * moveDirection;
            playerWASDMomentum += momentum;
            if (momentum.magnitude > maxPlayerInputSpeed)
                momentum = Vector3.ClampMagnitude(playerWASDMomentum, momentum.magnitude);
            else
                momentum = playerWASDMomentum;
        }

        Gravity();

        //Multiply momentum by correct drag type
        currDrag = 1 - currDrag;
        momentum *= currDrag;


        CheckForOncomingCollision();


        momentum = Vector3.ClampMagnitude(momentum, maxSpeed);
        transform.position += momentum;
    }



    void Gravity()
    {
        if (!isGrounded)
        {
            momentum.y -= actualGravity * Time.deltaTime - actualGravity * Time.deltaTime * momentum.y;
        }
        else
        {
            momentum.y = 0.0f;
        }
        isGrounded = false;
    }

    void InitialiseMovement()
    {
        if (playerInputActions == null) playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.started += Jump_Started;
    }

    private void Jump_Started(InputAction.CallbackContext context)
    {

        Debug.Log("Starting Jump");
        if (jumpCount < 2)
        {
            isGrounded = false;
            Debug.Log("Have enoough jump counts");
            if (lastJumpTime + jumpCooldown < Time.time)
            {
                Debug.Log("Beginning Jump");
                if (lastGroundedTime + jumpGraceLength > Time.time && jumpCount == 0)
                {
                    jumpCount++;
                }
                else
                {
                    jumpCount += 2;
                }
                momentum.y = jumpStrength;
                lastJumpTime = Time.time;
            }
        }
    }
    void CheckForOncomingCollision()
    {
        RaycastHit hit;
        if (Physics.CapsuleCast(
            transform.position + new Vector3(0, 0.5f, 0),
            transform.position - new Vector3(0, 0.5f, 0),
            0.45f, momentum.normalized, out hit, momentum.magnitude, ~3
            ))
        {
            momentum = Vector3.ClampMagnitude(momentum, hit.distance);
            Debug.Log(hit.collider);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //TODO: WALL RUNNING
        /*
         * Check if wall
         * check if normal is not pointing up
         * check direction of relative velocity
         */
        Vector3 normal = collision.GetContact(0).normal;
        normal *= Mathf.Sign(Vector3.Dot(transform.position - collision.transform.position, normal));
        momentum -= Vector3.Dot(momentum, normal) * normal;
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }
}
