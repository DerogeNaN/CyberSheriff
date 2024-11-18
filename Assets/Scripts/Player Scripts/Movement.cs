using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public static Movement playerMovement;
    public PlayerInputActions playerInputActions;

    [Header("Serialize Fields - PLUG THESE IN!!!")]
    public Transform respawnPos;
    [SerializeField] Transform cameraSlidePos;
    [SerializeField] Transform cameraDefaultPos;
    [SerializeField] GameObject cameraWallrunHolder;
    [SerializeField] CapsuleCollider playerCapsule;
    [SerializeField] Animator revolverAnimator;
    [SerializeField] Animator shotgunAnimator;
    //public Collider slideCollider;
    public GameObject grappleUI;

    //[HideInInspector][Tooltip("")]

    #region Movement
    [Header("Movement Settings")]
    [SerializeField][Tooltip("How fast the player moves each frame (Think of this as acceleration)")]
    public float moveSpeed = 4.0f;

    [SerializeField][Tooltip("The maximum speed the player with just WASD input")]
    private float maxPlayerInputSpeed = 2.0f;

    //Backend Variables:
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public float lastGroundedTime = 0;
    #endregion

    #region Jumping
    [Header("Jump Settings")]
    [SerializeField][Tooltip("How powerful the jump is")]
    private float jumpStrength = 0.25f;

    [SerializeField][Tooltip("Time allowed once falling to still jump (This is meant to be small / unnoticeable)")]
    private float jumpGraceLength = 0.05f;                                      //Time allowed once falling to still jump

    [SerializeField][Tooltip("Same as Jump Grace Length but for wallruns")]
    private float wallrunJumpGraceLength = 0.5f;                                //Time allowed once falling off a wallrun to still jump

    //Backend Variables
    [HideInInspector] public int jumpCount = 0;
    private float jumpCooldown = 0.01f;
    private float lastJumpTime = 0;
    #endregion

    #region Sliding
    [Header("Slide Settings")]
    [SerializeField][Tooltip("How fast the player must be moving to slide")]
    private float slideSpeedThreshold = 0.1f;                                   //How fast the player must be moving to slide

    [SerializeField][Tooltip("How much time is seconds before \"slide drag\" is applied")]
    private float slideDragDelay = 4f;                                        //Time it takes to start adding drag back to the player

    [SerializeField][Tooltip("The amount the player slows down each frame after \"Slide Drag Delay\"")]
    [Range(0, 1)] public float slideDrag = 10;

    //Backend Variables:
    private float slideStartTime = 0;
    [HideInInspector] public bool isTryingSlide = false;
    [HideInInspector] public bool isSliding = false;
    #endregion

    #region Dashing
    [Header("Dash Settings")]
    [SerializeField][Tooltip("The speed the player travels while in a dash")]
    public float dashSpeed = 10.0f;

    [SerializeField][Tooltip("How long in seconds the player travels at \"Dash Speed\"")]
    public float dashTime = 0.5f;

    [SerializeField][Tooltip("Time in seconds between dashes")]
    public float dashCooldown = 1;

    //Backend Variables
    private Vector3 dashDirection = Vector3.zero;
    [HideInInspector] public bool isDashing = false;
    private bool isTryingDashing = false;
    private float dashStartTime = 0;
    private float lastDashTime = 0;
    #endregion

    #region Wall Running
    [Header("Wall Run Settings")]
    [SerializeField][Tooltip("How fast the player must be moving to enter a wallrun")]
    private float wallrunSpeedThreshold = 0.1f;                                 //How fast the player must be moving to enter a wallrun

    [SerializeField][Tooltip("Time delay between wallruns (So player doesn't accidentally instantly enter another wall run)")]
    public float wallrunCooldown = 0.5f;

    [SerializeField][Tooltip("The angle that the player jumps towards while wall running (1 is roughly 90 degrees from the wall)")]
    public float wallrunJumpAngle = 45.0f;

    [SerializeField][Tooltip("Speed the player travels when wallrunning")]
    public float wallrunVelocityBonus = 0.2f;

    [SerializeField][Tooltip("How long in seconds the player can wallrun until gravity kicks in")]
    public float maxWallrunTime = 2.0f;


    //Backend Variables:
    [HideInInspector] public float cameraLeaveWallrunTime = 0;
    private float lastWallRunTime = 0;
    private float leavingWallrunTime = 0;
    [HideInInspector] public bool isWallRunning = false;
    private float wallRunStartTime = 0;
    private bool startWallrun = false;
    #endregion

    #region Grapple
    [Header("Grapple Settings")]
    [SerializeField][Tooltip("How fast the player travels while in a grapple")]
    public float grappleSpeed = 1f;

    [SerializeField][Tooltip("Time in seconds between grapples")]
    public float grappleCooldown = 5;

    [SerializeField][Tooltip("Max distance the player can enter a grapple")]
    public float maxGrappleDistance = 100f;

    //Backend Variables
    private Vector3 grappleTargetDirection = Vector3.zero;
    private float lastGrappleTime = 0;
    private GameObject grappleObject;
    private bool isGrappling = false;
    private bool canGrapple = false;
    #endregion

    #region Physics
    [Header("Physic Settings")]
    [SerializeField][Tooltip("Speed the player accelerates down")]
    private float gravityStrength = 5f;

    [SerializeField][Tooltip("How hard it is to change direction while grounded")]
    public int encouragedGroundMomentum = 3;

    [SerializeField][Tooltip("How hard it is to change direction while falling")]
    public int encouragedAirMomentum = 35;

    [SerializeField][Tooltip("How hard it is to change direction while sliding")]
    public int encouragedSlideMomentum = 30;

    //Backend Variables:
    private float currEncouragment;
    public int slowDownPercentage = 30;
    private float speedLimitEnforceAmmount = 1.5f;
    private float momentumRatio;
    #endregion

    #region Camera
    [Header("Camera Settings")]
    [SerializeField][Tooltip("How much the camera tilts during wallruns in degrees")]
    private float cameraWallrunTilt = 8.0f;                                     //How much the camera tilts during a wallrun

    [SerializeField][Range(0, 1)][Tooltip("How quick to tilt during wallrun (0 - never, 1 - instant)")]
    private float cameraWallRunTiltTime = 0.2f;                                 //How quick to tilt during wallrun 0 - never, 1 - instant

    [SerializeField][Tooltip("How long the camera takes to lower (0 - never, 1 - instant)")]
    public float cameraSlideTransitionTime = 0.1f;

    //Backend Variables:
    [SerializeField] private GameObject cameraHolder;
    #endregion

    #region Sound
    [Header("Sound Settings")]
    [SerializeField][Tooltip("Time in seconds between footsteps")]
    private float footstepInterval = 0.0f;

    [SerializeField][Tooltip("Time in seconds between footsteps while wall running")]
    private float wallRunFootstepInterval = 0.0f;

    //Backend Variables:
    private float lastFootstepTime = 0.0f;
    private float lastWallRunFootstepTime = 0.0f;
    private float slideDelay = 0.0f;
    #endregion

    [Space(20.0f)]
    [Header("Backend Variables")]    //Local Variables
    [HideInInspector] public Vector3 velocity = Vector3.zero;
    [HideInInspector] public Vector3 movementInputWorld = Vector3.zero;
    private Vector3 wallTangent = Vector3.zero;
    private Vector3 wallNormal = Vector3.zero;
    private Collider standingCollider;
    private PauseMenu pauseMenu;

    Vector3 collisionNormal = Vector3.zero;

    private void Awake()
    {
        playerMovement = this;
    }

    void Start()
    {
        standingCollider = GetComponent<Collider>();
        pauseMenu = GetComponentInChildren<PauseMenu>();
        InitialiseMovement();
    }

    public void UpdateMovement()
    {
        if (isSliding && isGrounded) currEncouragment = encouragedSlideMomentum;
        else if (!isGrounded) currEncouragment = encouragedAirMomentum;
        else currEncouragment = encouragedGroundMomentum;

        momentumRatio = 1 / currEncouragment;

        ComputeDepenetration();
        GroundCheck();
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 movementInputLocal = playerInputActions.Player.Move.ReadValue<Vector2>();
        movementInputWorld = transform.forward * movementInputLocal.y + transform.right * movementInputLocal.x;

        if (isTryingSlide) SlideCheck();
        else isSliding = false;


        MoveCameraTowardsTransformHeight(isSliding ? cameraSlidePos : cameraDefaultPos);


        if (isSliding)
        {
            standingCollider.enabled = false;
            //slideCollider.enabled = true;

            if (velocity.magnitude <= 1) isSliding = false;
        }
        else
        {
            standingCollider.enabled = true;
            //slideCollider.enabled = false;

            if (!isGrappling)
            {
                if (isWallRunning)
                {
                    TiltCameraFromWall(cameraWallrunTilt, cameraWallRunTiltTime, wallNormal);
                    movementInputWorld = Vector3.zero;
                }
                else
                {
                    UntiltCamera(0, cameraWallRunTiltTime);
                }
            }
        }




        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        Vector3 targetVelocity = (movementInputWorld * moveSpeed) * momentumRatio;
        float nextFrameMagnitude = (horizontalVelocity + targetVelocity).magnitude;
        float horizontalMag = horizontalVelocity.magnitude;

        if (isSliding)
        {
            float currSpeed = horizontalVelocity.magnitude;
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, currSpeed);
            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;
        }

        else
        {
            if (nextFrameMagnitude >= maxPlayerInputSpeed && !isDashing)
            {
                float resultantMagnitude = Mathf.Max(maxPlayerInputSpeed, horizontalMag);
                resultantMagnitude -= targetVelocity.magnitude;

                horizontalVelocity = horizontalVelocity.normalized * resultantMagnitude;

                velocity.x = horizontalVelocity.x;
                velocity.z = horizontalVelocity.z;
            }

            velocity.x += targetVelocity.x;
            velocity.z += targetVelocity.z;

            if (isGrounded)
            {
                horizontalVelocity += targetVelocity;

                float velocityMag = horizontalVelocity.magnitude;

                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, Mathf.Max(maxPlayerInputSpeed, velocityMag - speedLimitEnforceAmmount));
                velocity.x = horizontalVelocity.x;
                velocity.z = horizontalVelocity.z;
            }
        }

        if (movementInputLocal == Vector2.zero && isGrounded && !isSliding && !isDashing && !isWallRunning)
        {
            float speed = velocity.magnitude;
            float moveMag = moveSpeed * momentumRatio * slowDownPercentage / 100;
            float subtraction = Mathf.Min(speed, moveMag);
            velocity -= velocity.normalized * subtraction;

            if (Mathf.Abs(velocity.x) < 0.0015f && Mathf.Abs(velocity.z) < 0.0015f)
            {
                //velocity.x = 0;
                //velocity.z = 0;
            }
        }

        if (isSliding && Time.time >= slideStartTime + slideDragDelay)
        {
            velocity.x *= slideDrag;
            velocity.z *= slideDrag;
        }

        Gravity();


        CheckForWallRun();

        CheckForGrappleTarget();
        Grappling();

        CheckForOncomingCollision();
        PlayFootstepSound();

        //Actually apply motion to player transform
        transform.position += velocity * Time.deltaTime;
    }

    void PlayWallRunFootstepSound()
    {
        if (isWallRunning)
        {
            if (lastWallRunFootstepTime + wallRunFootstepInterval < Time.time)
            {
                lastWallRunFootstepTime = Time.time;
                SoundManager2.Instance.PlaySound("Wallrunning");
            }
        }
    }

    void PlayFootstepSound()
    {
        if (isGrounded && !isSliding && velocity != Vector3.zero && velocity.magnitude >= 3)
        {
            if (lastFootstepTime + footstepInterval < Time.time)
            {
                lastFootstepTime = Time.time;
                SoundManager2.Instance.PlaySound("Footsteps_Concrete");
            }
        }
    }

    void Gravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravityStrength * Time.deltaTime;
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

        SoundManager2.Instance.PlaySound("SlideSFX");
    }

    private void Slide_Cancelled(InputAction.CallbackContext context)
    {
        isTryingSlide = false;
    }

    private void SlideCheck()
    {
        if (!isSliding && velocity.magnitude > slideSpeedThreshold && isGrounded && !isWallRunning)
        {
            isSliding = true;
            slideStartTime = Time.time;
        }
    }

    private void MoveCameraTowardsTransformHeight(Transform target)
    {
        float newCameraYPos = Mathf.Lerp(cameraHolder.transform.position.y, target.position.y, cameraSlideTransitionTime);
        cameraHolder.transform.position = new Vector3(cameraHolder.transform.position.x, newCameraYPos, cameraHolder.transform.position.z);
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
        dashDirection = movementInputWorld.normalized;

        if (dashDirection == Vector3.zero) dashDirection = transform.forward.normalized;

        if (isTryingDashing && lastDashTime + dashCooldown < Time.time)
        {
            isDashing = true;
            dashStartTime = Time.time;

            if (!Physics.CapsuleCast(transform.position + new Vector3(0, 0.5f, 0), transform.position - new Vector3(0, 0.5f, 0), 
                0.45f, dashDirection, dashSpeed * Time.deltaTime, ~12))
            {
                isWallRunning = false;
                velocity = dashDirection * dashSpeed;
            }

            else
            {
                velocity = new Vector3(0, 0, 0);
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
            velocity.x = newMomentum.x;
            velocity.z = newMomentum.z;
        }

    }

    private void Jump_Started(InputAction.CallbackContext context)
    {
        if (jumpCount < 2)
        {
            isGrounded = false;
            isSliding = false;

            if (isWallRunning)
            {
                leavingWallrunTime = Time.time;
                isWallRunning = false;

                Vector3 velocityHori = new Vector3(velocity.x, 0, velocity.z);
                velocityHori.Normalize();

                Vector3 targetDirection = Vector3.RotateTowards(velocityHori, wallNormal, wallrunJumpAngle, 0);
                velocity = targetDirection * velocity.magnitude;
            }

            if (lastJumpTime + jumpCooldown < Time.time)
            {
                if (lastGroundedTime + jumpGraceLength > Time.time && jumpCount == 0)
                {
                    jumpCount++;
                    SoundManager2.Instance.PlaySound("JumpSFX");
                }

                else if (lastWallRunTime + wallrunJumpGraceLength > Time.time)
                {
                    jumpCount++;
                    SoundManager2.Instance.PlaySound("JumpSFX");
                }

                else
                {
                    jumpCount += 2;
                    SoundManager2.Instance.PlaySound("DoubleJumpSFX");
                }

                velocity.y = jumpStrength;
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
            if (lastGrappleTime + grappleCooldown < Time.time)
            {
                canGrapple = true;
                grappleUI.gameObject.SetActive(true);
            }
            
        }

        else
        {
            isGrappling = false;
            canGrapple = false;
            grappleObject = null;
            grappleTargetDirection = Vector3.zero;
            grappleUI.gameObject.SetActive(false);
        }
    }

    private void Grapple_Performed(InputAction.CallbackContext context)
    {
        if (canGrapple)
        {
            Vector3 targetDirection = grappleObject.transform.position - transform.position;
            grappleTargetDirection = targetDirection.normalized;
            isGrappling = true;
            lastGrappleTime = Time.time;
            SoundManager2.Instance.PlaySound("Grapple");
        }
    }

    private void Grappling()
    {
        if (canGrapple && isGrappling)
        {
            velocity = grappleTargetDirection.normalized * grappleSpeed;
        }
    }

    private void Grapple_Canceled(InputAction.CallbackContext context)
    {
        isGrappling = false;
        SoundManager2.Instance.StopSound("Grapple");
    }

    private void UntiltCamera(float tiltAngle, float tiltSpeed)
    {
        float newCameraAngle = Mathf.LerpAngle(Camera.main.transform.localEulerAngles.z, tiltAngle, tiltSpeed);

        cameraWallrunHolder.transform.localEulerAngles = new Vector3(
            Camera.main.transform.localEulerAngles.x,
            Camera.main.transform.localEulerAngles.y,
            newCameraAngle
            );
    }

    private void TiltCameraFromWall(float tiltAngle, float tiltSpeed, Vector3 normal)
    {
        float tiltCorrectionSign = Mathf.Sign(Vector3.Dot(-Camera.main.transform.right, normal));

        float newCameraAngle = Mathf.LerpAngle(Camera.main.transform.localEulerAngles.z, tiltAngle * tiltCorrectionSign, tiltSpeed);


        cameraWallrunHolder.transform.localEulerAngles = new Vector3(
            Camera.main.transform.localEulerAngles.x,
            Camera.main.transform.localEulerAngles.y,
            newCameraAngle
            );
    }

    void GroundCheck()
    {
        if (Physics.SphereCast(transform.position, 0.35f, Vector3.down, out RaycastHit hitInfo, 100.0f, ~0b00001100))
        {
            if (hitInfo.distance <= 1.1f)
            {
                if (!hitInfo.collider.isTrigger)
                {
                    if (!isGrounded) SoundManager2.Instance.PlaySound("LandingSFX");
                    isGrounded = true;
                    isWallRunning = false;
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

    void ComputeDepenetration()
    {
        Vector3 resolveDirection = Vector3.zero;
        float resolveDistance = 0.0f;
        Collider[] overlappingColliders = { };

        overlappingColliders = Physics.OverlapCapsule(transform.position + new Vector3(0, 0.7f, 0),
                                                        transform.position - new Vector3(0, 0.7f, 0),
                                                        0.6f, ~12, QueryTriggerInteraction.Ignore);

        

        for (int i = 0; i < overlappingColliders.Length; i++)
        {
            if (Physics.ComputePenetration(playerCapsule, transform.position, transform.rotation,
                                        overlappingColliders[i], overlappingColliders[i].transform.position, overlappingColliders[i].transform.rotation,
                                        out resolveDirection, out resolveDistance))
            {
                transform.position += resolveDirection * resolveDistance;
            }
        }
    }

    void CheckForOncomingCollision()
    {
        RaycastHit[] hitArray;

        float overShoot = 0f;

        if (!isSliding)
        {
            Vector3 movementDirection = velocity.normalized;
            hitArray = Physics.CapsuleCastAll(transform.position + new Vector3(0, 0.7f, 0) - movementDirection * overShoot,
                                              transform.position - new Vector3(0, 0.7f, 0) - movementDirection * overShoot,
                                              0.5f, movementDirection, velocity.magnitude * Time.deltaTime + overShoot, ~12, QueryTriggerInteraction.Ignore);

            //if (Physics.Raycast(transform.position - transform.forward * 0.5f, velocity.normalized, out RaycastHit hit, velocity.magnitude * Time.deltaTime, ~12, QueryTriggerInteraction.Ignore))

            for (int i = 0; i < hitArray.Length; i++)
            {
                collisionNormal = hitArray[i].normal;
                collisionNormal *= -Mathf.Sign(Vector3.Dot(transform.position - hitArray[i].point, hitArray[i].normal));
                Debug.DrawRay(hitArray[i].point, hitArray[i].normal * 2, Color.magenta);
                if (hitArray[i].point == Vector3.zero)
                {
                    Debug.Log("What the eff");
                }

                if (Vector3.Dot(velocity, collisionNormal) < 0) continue;

                float normalInUp = Vector3.Dot(Vector3.up, collisionNormal);
                
                if (normalInUp < 0.95f && normalInUp > 0.05f)
                {
                    //If the collision is with something that's not a vertical wall *or* a horizontal floor
                    Vector3 horiNormal = new Vector3(collisionNormal.x, 0, collisionNormal.z).normalized;
                    float velocityInHoriNormalDirection = Vector3.Dot(velocity, horiNormal);
                    velocity -= velocityInHoriNormalDirection * horiNormal;
                }

                float clampAmount = Vector3.Dot(movementInputWorld, collisionNormal);
                if (clampAmount < 0) continue;

                if (normalInUp <= 0.25f)
                {
                    clampAmount = 1 - clampAmount;

                    velocity.x = Mathf.Clamp(velocity.x, -(clampAmount * maxPlayerInputSpeed), clampAmount * maxPlayerInputSpeed);
                    velocity.z = Mathf.Clamp(velocity.z, -(clampAmount * maxPlayerInputSpeed), clampAmount * maxPlayerInputSpeed);
                }

                float velocityInNormalDirection = Vector3.Dot(velocity, collisionNormal);
                velocity -= velocityInNormalDirection * collisionNormal;
            }
        }

        else
        {
            hitArray = Physics.SphereCastAll(transform.position + new Vector3(0, -0.7f, 0),
                                              0.45f, velocity.normalized, velocity.magnitude * Time.deltaTime, ~12, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitArray.Length; i++)
            {
                Vector3 normal = hitArray[i].normal;
                normal *= -Mathf.Sign(Vector3.Dot(transform.position - hitArray[i].collider.transform.position, hitArray[i].normal));

                if (Vector3.Dot(velocity, normal) < 0); //continue;

                float normalInUp = Vector3.Dot(Vector3.up, normal);
                if (normalInUp < 0.95f && normalInUp > 0.05f)
                {
                    Vector3 horiNormal = new Vector3(normal.x, 0, normal.z).normalized;
                    float velocityInHoriNormalDirection = Vector3.Dot(velocity, horiNormal);
                    velocity -= velocityInHoriNormalDirection * horiNormal;
                }
                float velocityInNormalDirection = Vector3.Dot(velocity, normal);
                velocity -= velocityInNormalDirection * normal;

                float clampAmmount = Vector3.Dot(movementInputWorld, normal);
                if (clampAmmount < 0)
                {
                    continue;
                }

                if (Vector3.Dot(normal, Vector3.up) <= 0.25f)
                {
                    clampAmmount = 1 - clampAmmount;

                    velocity.x = Mathf.Clamp(velocity.x, -(clampAmmount * maxPlayerInputSpeed), clampAmmount * maxPlayerInputSpeed);
                    velocity.z = Mathf.Clamp(velocity.z, -(clampAmmount * maxPlayerInputSpeed), clampAmmount * maxPlayerInputSpeed);
                }
            }
        }
    }

    private void CheckForWallRun()
    {
        RaycastHit wallHit;
        Vector3 normal = Vector3.zero;

        if (!isGrounded)
        {
            //---check RIGHT for wall----
            if (Physics.CapsuleCast(
                transform.position + new Vector3(0, 0.7f, 0),
                transform.position - new Vector3(0, 0.7f, 0), 0.45f, transform.right, out wallHit, 0.5f, LayerMask.GetMask("Wall"), QueryTriggerInteraction.Ignore) &&
                Mathf.Abs(Vector3.Dot(wallHit.normal, transform.up)) < 0.0001f && leavingWallrunTime + wallrunCooldown < Time.time && !isGrounded)
            {
                normal = wallHit.normal;
                normal *= -Mathf.Sign(Vector3.Dot(transform.position - wallHit.point, normal));

                wallNormal = -normal;
                WallRun();
            }

            //---check LEFT for wall----
            else if (Physics.CapsuleCast(
                transform.position + new Vector3(0, 0.7f, 0),
                transform.position - new Vector3(0, 0.7f, 0), 0.45f, -transform.right, out wallHit, 0.5f, LayerMask.GetMask("Wall"), QueryTriggerInteraction.Ignore) &&
                Mathf.Abs(Vector3.Dot(wallHit.normal, transform.up)) < 0.0001f && leavingWallrunTime + wallrunCooldown < Time.time && !isGrounded)
            {
                normal = wallHit.normal;
                normal *= Mathf.Sign(Vector3.Dot(transform.position - wallHit.point, normal));

                wallNormal = normal;
                WallRun();
            }

            else if (isWallRunning)
            {
                WallRunReset();
            }
        }

        else 
        {
            isWallRunning = false;
            wallNormal = Vector3.zero;
        }
    }

    private void WallRun()
    {
        Vector3 tangent = Vector3.Cross(Vector3.up, wallNormal);
        wallTangent = tangent * Mathf.Sign(Vector3.Dot(velocity, tangent));
        float wallSpeed = Vector3.Dot(tangent, velocity);

        if (!isWallRunning) wallRunStartTime = Time.time;
        if (Mathf.Abs(wallSpeed) > wallrunSpeedThreshold && wallRunStartTime + maxWallrunTime >= Time.time)
        {
            lastWallRunTime = Time.time;
            cameraLeaveWallrunTime = Time.time + 0.2f;
            isWallRunning = true;
            velocity = wallSpeed * tangent;
            jumpCount = 0;

            if (velocity.magnitude < wallrunVelocityBonus)
            {
                velocity = velocity.normalized * wallrunVelocityBonus;
            }
            PlayWallRunFootstepSound();
            TiltCameraFromWall(cameraWallrunTilt, cameraLeaveWallrunTime, wallNormal);
            return;
        }

        else if (wallRunStartTime + 0.1f < Time.time)
        {
            Vector3 velocityHori = new Vector3(velocity.x, 0, velocity.z);
            velocityHori.Normalize();

            Vector3 targetDirection = Vector3.RotateTowards(velocityHori, wallNormal, wallrunJumpAngle, 0);
            velocity = targetDirection * velocity.magnitude;
        }
    }

    private void WallRunReset()
    {
        lastWallRunTime = Time.time;
        cameraLeaveWallrunTime = Time.time + 0.2f;
        isWallRunning = false;
        wallNormal = Vector3.zero;
    }
}