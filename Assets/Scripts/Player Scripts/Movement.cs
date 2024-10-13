using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
//using UnityEditor.Build.Content;
using UnityEditor.Rendering;
//using UnityEditor.ShaderGraph;
//using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;


public class Movement : MonoBehaviour
{
    public static Movement playerMovement;
    public PlayerInputActions playerInputActions;

    #region Movement
    [Header("Movement Settings")]
    [SerializeField][Tooltip("How fast the player moves (Think of this as acceleration)")]
    public float moveSpeed = 4.0f;

    [SerializeField][Tooltip("The maximum speed the player can move with just WASD input")]
    private float maxPlayerInputSpeed = 2.0f;

    [SerializeField][Tooltip("The maximum speed the player can move through any means")]
    private float maxSpeed = 100.0f;

    //Backend Variables:

    #endregion

    #region Jumping
    [Header("Jump Settings")]
    [SerializeField][Tooltip("How powerful the jump is")]
    private float jumpStrength = 0.25f;

    [SerializeField][Tooltip("Time allowed once falling to still jump (This is meant to be small / unnoticeable)")]
    private float jumpGraceLength = 0.05f;                                      //Time allowed once falling to still jump

    [SerializeField][Tooltip("Same as Jump Grace Length but for wallruns")]
    private float wallrunJumpGraceLength = 0.5f;                                //Time allowed once falling off a wallrun to still jump
    #endregion

    #region Physics
    [Header("Physic Settings")]
    [SerializeField]
    private float gravityStrength = 5f;

    [SerializeField][Tooltip("Drag is basically friction")] 
    private float airDrag = 10f;

    [SerializeField][Tooltip("Drag is basically friction")] 
    private float oldDrag = 15f;
    #endregion

    #region Sliding
    [Header("Slide Settings")][SerializeField][Tooltip("How fast the player must be moving to slide")]
    private float slideSpeedThreshold = 0.1f;                                   //How fast the player must be moving to slide

    [SerializeField]
    [Tooltip("How much time is seconds before the player starts slowing down")]
    private float slideDragDelay = 4f;                                        //Time it takes to start adding drag back to the player
    #endregion

    #region Wall Running
    [Header("Wall Run Settings")]
    [SerializeField][Tooltip("How fast the player must be moving to enter a wallrun")]
    private float wallrunSpeedThreshold = 0.1f;                                 //How fast the player must be moving to enter a wallrun

    [SerializeField][Tooltip("WIP - Not functional ATM!")] 
    private float wallrunAngleThreshold = 25.0f;                                //How much the player has to deviate from the wall to exit the wallrun
    #endregion

    #region Camera
    [Header("Camera Settings - TEMPORARY! WILL BE MOVED")]
    [SerializeField] Transform cameraSlidePos;
    [SerializeField] Transform cameraDefaultPos;
    [SerializeField][Tooltip("How much the camera tilts during wallruns in degrees")]
    private float cameraWallrunTilt = 8.0f;                                     //How much the camera tilts during a wallrun

    [SerializeField][Range(0, 1)][Tooltip("How quick to tilt during wallrun (0 - never, 1 - instant)")]
    private float cameraWallrunTiltTime = 0.2f;                                 //How quick to tilt during wallrun 0 - never, 1 - instant

    [SerializeField][Tooltip("WIP - Not functional ATM!")]
    public float cameraSlideTransitionTime = 0.1f;
    #endregion

    [Space(10.0f)]
    [Header("Backend Variables (TEST)")]    //Local Variables
    public Vector3 momentum = Vector3.zero;
    //private Vector3 previousMomentum = Vector3.zero;
    public Vector3 moveDirection = Vector3.zero;
    private Vector3 wallTangent = Vector3.zero;
    private Vector3 wallNormal = Vector3.zero;
    public Transform respawnPos;
    public Rigidbody rb;
    public Collider slideCollider;
    private Collider standingCollider;
    private PauseMenu pauseMenu;

    //----WALLRUNNING----
    public bool isWallrunning = false;
    public bool canWallrun = false;
    public float wallrunCooldown = 0.5f;
    public float leavingWallrunTime = 0;
    public float lastWallrunTime = 0;
    public float cameraLeaveWallrunTime = 0;
    public float wallrunJumpAngle = 45.0f;
    public float wallrunMomentumBonus = 0.2f;

    //----SLIDING----
    public bool isSliding = false;
    public bool isTryingSlide = false;
    public bool shouldSlide = false;
    public float slideStartTime = 0;
    [Range(0, 1)]public float slideDrag = 10;

    //----PHYSICS----
    [Range(0,1)]public float groundDrag = 15;
    public float currEncouragment;
    public int encouragedGroundMomentum = 3;
    public int encouragedAirMomentum = 35;
    public int encouragedSlideMomentum = 30;
    private float momentumRatio;
    private float actualGravity = 0;

    //----JUMPING----
    public int jumpCount = 0;
    public float jumpCooldown = 0.01f;
    public float lastJumpTime = 0;
    public float bunnyHopGrace = 0.05f;

    //----DASHING----
    public Vector3 dashDirection = Vector3.zero;
    public float dashFOVChange = 95f; //Camera fov switch during dash
    public bool isDashing = false;
    public bool isTryingDashing = false;
    public float dashDistance = 10.0f;
    public float dashForce = 2.0f;
    public float dashTime = 0.5f;
    public float dashStartTime = 0;
    public float lastDashTime = 0;
    public float dashCooldown = 1;

    //----MOVEMENT----
    public bool isGrounded = true;
    public float lastGroundedTime = 0;
    public MovementState playerState;

    //----GRAPPLE----
    public bool isGrappling = false;
    public bool canGrapple = false;
    public GameObject grappleObject;
    public ParticleSystem grappleUI;
    public float grappleSpeed = 1f;
    public float grappleCooldown = 5;
    public float maxGrappleDistance = 100f;
    public float lastGrappleTime = 0;
    public Vector3 grappleTargetDirection = Vector3.zero;

    public Vector2 rawMoveInput = Vector2.zero;

    public enum MovementState
    {
        grounded,
        air,
        wallrunning,
        sliding,
        dashing,
        grappling
    }


    void Start()
    {
        playerMovement = this;
        standingCollider = GetComponent<Collider>();
        pauseMenu = GetComponentInChildren<PauseMenu>();
        InitialiseMovement();
        playerState = MovementState.grounded;
    }

    public void UpdateMovement()
    {
        actualGravity = 0.1f * gravityStrength;

        //TODO: change the currEncourangment check to account for a new "encouragedSlideMomentum"
        if (isSliding && isGrounded) currEncouragment = encouragedSlideMomentum;
        else if (!isGrounded) currEncouragment = encouragedAirMomentum;
        else currEncouragment = encouragedGroundMomentum;

        //currEncouragment = isGrounded ? encouragedGroundMomentum : encouragedAirMomentum;

        momentumRatio = 1 / currEncouragment;

        //UpdateCamera();
        MovePlayer();
        GroundCheck();
        Debug.DrawRay(transform.position, momentum * 5, Color.red);
    }

    void MovePlayer()
    {
        Vector2 moveInput = Vector2.zero;

        if (isTryingSlide) SlideCheck();

        if (isSliding)
        {
            //SetState(MovementState.sliding);
            SlideStartTransition();
            moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
            moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
            if (momentum.magnitude <= 0.01f) isSliding = false;
        }

        else if (isGrappling)
        {
            moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
        }

        else if (isWallrunning)
        {
            TiltCamera(cameraWallrunTilt, cameraWallrunTiltTime, wallNormal);
            //moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
            //moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        }

        else
        {
            TiltCamera(0, cameraWallrunTiltTime);
            SlideExitTransition();
            rawMoveInput = playerInputActions.Player.Move.ReadValue<Vector2>();
            moveInput = playerInputActions.Player.Move.ReadValue<Vector2>() * Time.deltaTime;
            moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        }

        Vector3 playerWASDMomentum = Vector3.zero;

        playerWASDMomentum += moveDirection * moveSpeed * momentumRatio;
        Vector3 momentumNoY = new Vector3(momentum.x, 0, momentum.z);
        float encouragedAmount = Vector3.Dot(moveDirection, momentumNoY.normalized) * (1 - momentumRatio);
        playerWASDMomentum += Mathf.Clamp(encouragedAmount, 0, 1) * moveSpeed * moveDirection;

        //NEW VARIABLE NEEDS TO BE INTRODUCED HERE, value that represents a desired velocity vector opposed to the actual velocity vector
        //then figure out how much of the actual velocity is in desired velocity and reduces the x,z componenets of actual to be desired

        /*
        if playerWASD + momentum 's momentum is bigger than the maxPlayerInputSpeed
            momentum = Vector3.ClampMagnitude(momentum, momentum.magnitude - playerWASD.magnitude);
        momentum += playerWASD;
         */

        if ((playerWASDMomentum + momentumNoY).magnitude >= maxPlayerInputSpeed)
        {
            momentumNoY = Vector3.ClampMagnitude(momentumNoY, momentumNoY.magnitude - playerWASDMomentum.magnitude);
        }

        momentum.x = momentumNoY.x;
        momentum.z = momentumNoY.z;

        momentum.x += playerWASDMomentum.x;
        momentum.z += playerWASDMomentum.z;

        //Multiply momentum by correct drag type

        if (moveInput == Vector2.zero && isGrounded && !isSliding && !isDashing)
        {
            momentum.x += -momentum.x / moveSpeed;
            momentum.z += -momentum.z / moveSpeed;

            if (Mathf.Abs(momentum.x) < 0.015f && Mathf.Abs(momentum.z) < 0.015f)
            {
                momentum.x = 0;
                momentum.z = 0;
            }
        }

        if (isSliding && Time.time >= slideStartTime + slideDragDelay)
        {
            momentum.x *= slideDrag;
            momentum.z *= slideDrag;
        }

        Gravity();

        CheckForOncomingCollision();

        //CheckForWallrun();

        CheckForGrappleTarget();
        Grappling();

        rb.velocity = Vector3.zero;
        momentum = Vector3.ClampMagnitude(momentum, maxSpeed);
        transform.position += momentum;
    }

    void SetState(MovementState desiredState)
    {
        MovementState prevState = playerState;
        playerState = desiredState;

        StateTransition(prevState, playerState);
    }

    void StateTransition(MovementState oldState, MovementState newState)
    {
        if (oldState == MovementState.grounded && newState == MovementState.air)
        {
            
        }

        if (oldState == MovementState.grounded && newState == MovementState.sliding)
        {
            isSliding = true;
            slideStartTime = Time.time;
            isGrounded = false;
        }

        if (oldState == MovementState.sliding && newState == MovementState.grounded)
        {
            isSliding = false;
            isGrounded = true;
        }
    }

    void Gravity()
    {
        if (!isGrounded)
        {
            momentum.y -= actualGravity * Time.deltaTime;
        }
    }

    void InitialiseMovement()
    {
        if (playerInputActions == null) playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.started += Jump_Started;
        playerInputActions.Player.Slide.performed += Slide_Performed;
        playerInputActions.Player.Slide.canceled += Slide_Cancelled;
        playerInputActions.Player.Dash.performed += Dash_Performed;
        playerInputActions.Player.Dash.canceled += Dash_Canceled;
        playerInputActions.Player.Grapple.performed += Grapple_Performed;
        playerInputActions.Player.Grapple.canceled += Grapple_Canceled;
        playerInputActions.Player.Pause.performed += pauseMenu.PauseMenuToggle;
    }

    private void Slide_Performed(InputAction.CallbackContext context)
    {
        isTryingSlide = true;
    }

    private void Slide_Cancelled(InputAction.CallbackContext context)
    {
        isTryingSlide = false;
        shouldSlide = false;
        //SetState(MovementState.grounded);
    }

    private void SlideCheck()
    {
        if (momentum.magnitude > slideSpeedThreshold && isGrounded && !isWallrunning)
        {
            shouldSlide = true;
            if (shouldSlide) SlideStart();
        }
    }

    private void SlideStart()
    {
        isSliding = true;
        slideStartTime = Time.time;
    }

    private void SlideStartTransition()
    {
        float newCameraYPos = Mathf.Lerp(Camera.main.transform.position.y, cameraSlidePos.position.y, cameraSlideTransitionTime);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, newCameraYPos, Camera.main.transform.position.z);

        standingCollider.enabled = false;
        slideCollider.enabled = true;
    }

    private void SlideExitTransition()
    {
        float newCameraYPos = Mathf.Lerp(Camera.main.transform.position.y, cameraDefaultPos.position.y, cameraSlideTransitionTime);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, newCameraYPos, Camera.main.transform.position.z);

        slideCollider.enabled = false;
        standingCollider.enabled = true;

        //SetState(MovementState.grounded);
    }

    private void Dash_Performed(InputAction.CallbackContext context)
    {
        isTryingDashing = true;
        DashStartTransition();
    }

    private void Dash_Canceled(InputAction.CallbackContext context)
    {
        isTryingDashing = false;
    }

    private void DashStartTransition()
    {
        dashDirection = moveDirection.normalized;

        if (dashDirection == Vector3.zero) dashDirection = transform.forward.normalized;

        if (isTryingDashing && lastDashTime + dashCooldown < Time.time)
        {
            isDashing = true;
            dashStartTime = Time.time;

            RaycastHit hit;
            if (!Physics.CapsuleCast(transform.position + new Vector3(0, 0.5f, 0), transform.position - new Vector3(0, 0.5f, 0), 
                0.45f, dashDirection, out hit, dashDistance, ~12))
            {
                isWallrunning = false;
                momentum = dashDirection * dashDistance;
            }

            else
            {
                momentum = new Vector3(0, 0, 0);
            }

            Invoke(nameof(DashReset), dashTime);
        }
    }

    private void DashReset()
    {
        isDashing = false;
        lastDashTime = Time.time;

        if (!isSliding)
        {
            Vector3 newMomentum = dashDirection * maxPlayerInputSpeed;
            momentum.x = newMomentum.x;
            momentum.z = newMomentum.z;
        }

    }

    private void Jump_Started(InputAction.CallbackContext context)
    {
        if (jumpCount < 2)
        {
            isGrounded = false;
            isSliding = false;

            if (isWallrunning)
            {
                leavingWallrunTime = Time.time;
                isWallrunning = false;

                Vector3 targetDirection = Vector3.RotateTowards(wallTangent, wallNormal, wallrunJumpAngle, 0);
                momentum = targetDirection;
                momentum *= wallrunMomentumBonus;
            }

            if (lastJumpTime + jumpCooldown < Time.time)
            {
                if (lastGroundedTime + jumpGraceLength > Time.time && jumpCount == 0)
                {
                    jumpCount++;
                }

                else if (lastWallrunTime + wallrunJumpGraceLength > Time.time)
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

    private void CheckForGrappleTarget()
    {
        if ((Physics.Raycast(transform.position, Camera.main.transform.forward, out RaycastHit hit, maxGrappleDistance, ~8) &&
            hit.transform.CompareTag("GrappleableObject")))
        {
            grappleObject = hit.collider.gameObject;
            grappleUI.transform.position = hit.collider.transform.position;
            if (!grappleUI.isPlaying && lastGrappleTime + grappleCooldown < Time.time)
            {
                canGrapple = true;
                grappleUI.gameObject.SetActive(true);
                grappleUI.Play();
            }
            
        }

        else
        {
            canGrapple = false;
            grappleObject = null;
            if (grappleUI.isPlaying)
            {
                grappleUI.gameObject.SetActive(false);
                grappleUI.Stop();
            }
        }
    }

    private void Grapple_Performed(InputAction.CallbackContext context)
    {
        if (canGrapple)
        {
            Vector3 targetDirection = grappleObject.transform.position - transform.position;
            grappleTargetDirection = targetDirection.normalized;
            isGrappling = true;
        }
    }

    private void Grappling()
    {
        if (canGrapple && isGrappling)
        {
            momentum = grappleTargetDirection.normalized * grappleSpeed;
        }
    }

    private void Grapple_Canceled(InputAction.CallbackContext context)
    {
        isGrappling = false;
        lastGrappleTime = Time.time;
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

    void GroundCheck()
    {
        if (Physics.SphereCast(transform.position, 0.35f, Vector3.down, out RaycastHit hitInfo, 100.0f, ~0b00001100))
        {
           // Debug.Log(hitInfo.collider.gameObject);
            if (hitInfo.distance <= 0.75f)
            {
                if (!hitInfo.collider.isTrigger)
                {
                    isGrounded = true;
                    isWallrunning = false;
                    lastGroundedTime = Time.time;
                    jumpCount = 0;
                }
            }

            else isGrounded = false;
        }

        else
        {
            isGrounded = false;
        }
    }

    void CheckForOncomingCollision()
    {
        if (!isSliding)
        {
            if (Physics.CapsuleCast(
                transform.position + new Vector3(0, 0.5f, 0),
                transform.position - new Vector3(0, 0.5f, 0),
                0.45f, momentum.normalized, out RaycastHit hit, momentum.magnitude, ~12, QueryTriggerInteraction.Ignore
                ))
            {
                momentum = Vector3.ClampMagnitude(momentum, hit.distance);

                //float velocityInNormalDirection = Vector3.Dot(momentum, hit.normal);
                //
                //if (velocityInNormalDirection < 0)
                //{
                //    momentum -= velocityInNormalDirection * hit.normal;
                //}
            }
        }
        
        else
        {
            if (Physics.CapsuleCast(
                slideCollider.transform.position - new Vector3(0.5f, 0, 0),
                slideCollider.transform.position + new Vector3(0.5f, 0, 0),
                0.35f, momentum.normalized, out RaycastHit hit, momentum.magnitude, ~12, QueryTriggerInteraction.Ignore
                ))
            {
                momentum = Vector3.ClampMagnitude(momentum, hit.distance);

                //float velocityInNormalDirection = Vector3.Dot(momentum, hit.normal);
                //
                //if (velocityInNormalDirection < 0)
                //{
                //    momentum -= velocityInNormalDirection * hit.normal;
                //}
            }
        }
    }

    //private Vector3 GetCollisionNormal()
    //{
    //    return Vector3.zero;
    //}

    private void CheckForWallrun()
    {
        RaycastHit wallHit;

        //---check RIGHT for wall----
        if (Physics.CapsuleCast(
            transform.position + new Vector3(0, 0.5f, 0), 
            transform.position - new Vector3(0, 0.5f, 0), 0.35f, transform.right, out wallHit, 0.5f, ~12, QueryTriggerInteraction.Ignore) &&
            Mathf.Abs(Vector3.Dot(wallHit.normal, transform.up)) < 0.0001f && leavingWallrunTime + wallrunCooldown < Time.time && !isGrounded)
        {
            Debug.Log("There's a wall on my RIGHT");
            canWallrun = true;
        }


        //---check LEFT for wall----
        if (Physics.CapsuleCast(
            transform.position + new Vector3(0, 0.5f, 0),
            transform.position - new Vector3(0, 0.5f, 0), 0.35f, -transform.right, out wallHit, 0.5f, ~12, QueryTriggerInteraction.Ignore) &&
            Mathf.Abs(Vector3.Dot(wallHit.normal, transform.up)) < 0.0001f && leavingWallrunTime + wallrunCooldown < Time.time && !isGrounded)
        {
            Debug.Log("There's a wall on my LEFT");
            canWallrun = true;
        }


        else if (isWallrunning)
        {
            isWallrunning = false;
            leavingWallrunTime = Time.time;
        }
    }

    private void OnCollisionEnter(Collision collision)
    { 
        Vector3 normal = collision.GetContact(0).normal.normalized;
        normal *= Mathf.Sign(Vector3.Dot(transform.position - collision.transform.position, normal));
        
        wallNormal = normal;
        
        //If the normal of the wall collision points not up or down
        if (Mathf.Abs(Vector3.Dot(normal, transform.up)) < 0.0001f && leavingWallrunTime + wallrunCooldown < Time.time && !isGrounded)
        {
            Vector3 tangent = Vector3.Cross(Vector3.up, normal);
            wallTangent = tangent * Mathf.Sign(Vector3.Dot(momentum, tangent));
            float wallSpeed = Vector3.Dot(tangent, momentum);
        
            if(Mathf.Abs(wallSpeed) > wallrunSpeedThreshold)
            {
                lastWallrunTime = Time.time;
                cameraLeaveWallrunTime = Time.time + 0.2f;
                isWallrunning = true;
                momentum = wallSpeed * tangent;
                jumpCount = 0;
                return;
            }
        }
    
        //Get velocity relative to the collision normal
        float velocityInNormalDirection = Vector3.Dot(momentum, normal);
    
        //Check if positive or negative, if negative the player is trying to move into a wall so run the below code
        if (velocityInNormalDirection < 0)
        {
            momentum -= velocityInNormalDirection * normal;
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }
    
    private void OnCollisionExit(Collision collision)
    {   
        if (isWallrunning)
        {
            leavingWallrunTime = Time.time;
            isWallrunning = false;
        }
    }
}