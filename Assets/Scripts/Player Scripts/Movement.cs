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
    [SerializeField] float jumpGraceLength = 0.1f;                              //Time allowed once falling to still jump
    [SerializeField] float wallrunGraceLength = 0.5f;                           //Time allowed once falling off a wallrun to still jump
    [SerializeField] private float wallrunSpeedThreshold = 0.8f;                //How fast the player must be moving to enter a wallrun
    [SerializeField] private float wallrunAngleThreshold = 25.0f;               //How much the player has to deviate from the wall to exit the wallrun
    [SerializeField] private float slideSpeedThreshold = 0.1f;                  //How fast the player must be moving to slide
    [SerializeField] private float slideDragDelay = 2.5f;                       //Time it takes to start adding drag back to the player
    [SerializeField] private float cameraWallrunTilt = 8.0f;                    //How much the camera tilts during a wallrun
    [SerializeField][Range(0,1)] private float cameraWallrunTiltTime = 0.1f;    //How quick to tilt during wallrun 0 - never, 1 - instant

    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] Transform cameraSlidePos;
    [SerializeField] Transform cameraDefaultPos;
    [SerializeField] public PlayerInputActions playerInputActions;

    [Space(10.0f)]
    [Header("Backend Variables (TEST)")]//Local Variables
    public Vector3 momentum = Vector3.zero;
    public Vector3 moveDirection = Vector3.zero;
    private Vector3 wallTangent = Vector3.zero;

    public float currDrag;
    public float currEncouragment;
    private float momentumRatio;
    private float actualGravity = 0;
    public int jumpCount = 0;
    public bool isGrounded = true;
    public bool isWallrunning = false;
    public bool isSliding = false;
    public float lastGroundedTime = 0;
    public float jumpCooldown = 0.01f;
    public float wallrunCooldown = 0.5f;
    public float leavingWallrunTime = 0;
    public float lastWallrunTime = 0;
    public float lastJumpTime = 0;
    public float cameraSlideTransitionTime = 0.75f;

    [SerializeField] int encouragedGroundMomentum = 3;
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
        currEncouragment = isGrounded ? encouragedGroundMomentum : encouragedAirMomentum;
        //currDrag = groundDrag;
        //encouragedMomentum = isGrounded ? 35 : 50;
        currDrag *= 0.01f;

        momentumRatio = 1 / currEncouragment;

        UpdateCamera();
        MovePlayer();
    }

    void MovePlayer()
    { 
        if (isSliding)
        {
            
        }

        else if (isWallrunning)
        {
            Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
            moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        }

        else
        {
            Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
            moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        }

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
            playerWASDMomentum += moveDirection * moveSpeed * momentumRatio;
            Vector3 momentumNoY = new Vector3(momentum.x, 0, momentum.z);
            float encouragedAmount = Vector3.Dot(moveDirection, momentumNoY.normalized) * (1 - momentumRatio);
            playerWASDMomentum += Mathf.Clamp(encouragedAmount, 0, 1) * moveSpeed * moveDirection;
            playerWASDMomentum += momentum;

            if (momentum.magnitude > maxPlayerInputSpeed) momentum = Vector3.ClampMagnitude(playerWASDMomentum, momentum.magnitude);

            else momentum = playerWASDMomentum;
        }

        Gravity();

        //Multiply momentum by correct drag type
        currDrag = 1 - currDrag;
        momentum.x *= currDrag;
        momentum.z *= currDrag;

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
        playerInputActions.Player.Slide.performed += Slide_Performed;
        playerInputActions.Player.Slide.canceled += Slide_Cancelled;
    }

    private void Slide_Performed(InputAction.CallbackContext context)
    {
        if (!isWallrunning && momentum.magnitude > slideSpeedThreshold)
        {
            isSliding = true;
            Camera.main.transform.position = cameraSlidePos.position;
        }
    }

    private void Slide_Cancelled(InputAction.CallbackContext context)
    {
        isSliding = false;
        Camera.main.transform.position = cameraDefaultPos.position;
    }

    private void SlideTransition()
    {
        
    }

    private void UpdateCamera()
    {
        if (lastWallrunTime < Time.time)
        {
            TiltCamera(0, 0.1f);
        }
    }

    private void Jump_Started(InputAction.CallbackContext context)
    {
        if (jumpCount < 2)
        {
            isGrounded = false;

            if (isWallrunning)
            {
                leavingWallrunTime = Time.time;
                isWallrunning = false;
            }

            if (lastJumpTime + jumpCooldown < Time.time)
            {
                if (lastGroundedTime + jumpGraceLength > Time.time && jumpCount == 0)
                {
                    jumpCount++;
                }

                else if (lastWallrunTime + wallrunGraceLength > Time.time)
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

    private void TiltCamera(float tiltAngle, float tiltSpeed)
    {
        float newCameraAngle = Mathf.LerpAngle(Camera.main.transform.localEulerAngles.z, tiltAngle, tiltSpeed);

        Camera.main.transform.localEulerAngles = new Vector3(
            Camera.main.transform.localEulerAngles.x,
            Camera.main.transform.localEulerAngles.y,
            newCameraAngle
            );
    }

    private void TiltCamera(float tiltAngle, float tiltSpeed, Vector3 normal)
    {
        float tiltCorrectionSign = Mathf.Sign(Vector3.Dot(-Camera.main.transform.right, normal));
        //float newCameraAngle *= tilt;

        float newCameraAngle = Mathf.LerpAngle(Camera.main.transform.localEulerAngles.z, tiltAngle * tiltCorrectionSign, tiltSpeed);


        Camera.main.transform.localEulerAngles = new Vector3(
            Camera.main.transform.localEulerAngles.x,
            Camera.main.transform.localEulerAngles.y,
            newCameraAngle
            );
    }

    void CheckForOncomingCollision()
    {
        RaycastHit hit;
        if (Physics.CapsuleCast(
            transform.position + new Vector3(0, 0.5f, 0),
            transform.position - new Vector3(0, 0.5f, 0),
            0.45f, momentum.normalized, out hit, momentum.magnitude, ~8
            ))
        {
            momentum = Vector3.ClampMagnitude(momentum, hit.distance);
        }
    }

    //TODO: Add isWallRunning to mess with gravity
    //TODO: Add dirtyflag check in jump to add Wall Run Boost
    private void OnCollisionEnter(Collision collision)
    { 
        Vector3 normal = collision.GetContact(0).normal;
        normal *= Mathf.Sign(Vector3.Dot(transform.position - collision.transform.position, normal));

        TiltCamera(cameraWallrunTilt, cameraWallrunTiltTime, normal);

        //If the normal of the wall collision points not up or down
        if (Mathf.Abs(Vector3.Dot(normal, transform.up)) < 0.0001f && leavingWallrunTime + wallrunCooldown < Time.time && !isGrounded)
        {
            Vector3 tangent = Vector3.Cross(Vector3.up, normal);
            wallTangent = tangent;
            float wallSpeed = Vector3.Dot(tangent, momentum) * Mathf.Abs(Vector3.Dot(Camera.main.transform.forward, momentum.normalized));

            Vector3 newMomentum = momentum.magnitude * wallTangent;

            if(Mathf.Abs(wallSpeed) > wallrunSpeedThreshold)
            {
                lastWallrunTime = Time.time;
                isWallrunning = true;
                momentum = newMomentum;
                //momentum = wallSpeed * tangent;
                jumpCount = 0;
                return;
            }
        }

        momentum -= Vector3.Dot(momentum, normal) * normal;
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Leaving collision");
        
        if (isWallrunning)
        {
            leavingWallrunTime = Time.time;
            isWallrunning = false;
        }
    }
}