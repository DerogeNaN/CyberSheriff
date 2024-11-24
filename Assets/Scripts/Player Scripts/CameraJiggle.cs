using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraJiggle : MonoBehaviour
{
    [Header("Camera Tilt Settings:")]
    [SerializeField][Tooltip("How much the camera tilts in degrees once moving at \'Max Momentum Influence\'")]
    public float headTiltAtMax = 3.0f;

    [Range(0, 1)][Tooltip("How quickly the camera tilts to \'Head Tilt At Max\' from 0 (Never) to 1 (Instant)")]
    public float tiltTime = 0.0f;

    [SerializeField][Tooltip("The camera won't tilt any further past this speed (Usually same as \'Max Player Input Speed \')")]
    public float maxMomentumInfluence = 1.0f;
    public AnimationCurve cameraTiltCurve;

    //Backend:
    private float targetTilt = 0.0f;
    private float tiltLastFrame = 0.0f;

    [Space(10)]
    [Header("Camera FOV Settings")]
    [SerializeField][Tooltip("The default camera field of view")]
    private float defaultFOV = 80.0f;

    [SerializeField][Tooltip("How much the FOV increases performing certain actions (I.e. sliding or dashing")]
    public float changeInFOV = 0.0f;
    public AnimationCurve cameraFOVCurve;

    //Backend:
    private float lastFrameFOV = 0.0f;
    private float targetFOV = 0.0f;

    [Space(10)]
    [Header("Serialize Fields")]
    public GameObject cameraHolder;
    private Camera mainCamera;


    private void Start()
    {
        mainCamera = Camera.main;
        mainCamera.fieldOfView = defaultFOV;
    }

    void Update()
    {
        UpdateTilt();
        UpdateFOV();
    }

    void UpdateTilt()
    {
        if (!Movement.playerMovement.isWallRunning && !Movement.playerMovement.isSliding && Time.time > Movement.playerMovement.cameraLeaveWallrunTime)
        {
            targetTilt = Vector3.Dot(-transform.right, Vector3.ClampMagnitude(Movement.playerMovement.velocity, maxMomentumInfluence));
            targetTilt /= maxMomentumInfluence;
            targetTilt *= headTiltAtMax;

            if (targetTilt < 0)
            {
                targetTilt = cameraTiltCurve.Evaluate(Mathf.Abs(targetTilt));
                targetTilt = -targetTilt;
            }
            else targetTilt = cameraTiltCurve.Evaluate(targetTilt);
        }

        else if (Movement.playerMovement.isSliding)
        {
            targetTilt = Vector3.Dot(transform.right, Vector3.ClampMagnitude(Movement.playerMovement.velocity, maxMomentumInfluence));
            targetTilt /= maxMomentumInfluence;
            targetTilt *= headTiltAtMax;

            if (targetTilt < 0)
            {
                targetTilt = cameraTiltCurve.Evaluate(Mathf.Abs(targetTilt));
                targetTilt = -targetTilt;
            }

            else
            {
                targetTilt = cameraTiltCurve.Evaluate(targetTilt);
            }
        }

        float actualTilt = Mathf.Lerp(tiltLastFrame, targetTilt, tiltTime);

        cameraHolder.transform.localEulerAngles = new Vector3(
                cameraHolder.transform.localEulerAngles.x,
                cameraHolder.transform.localEulerAngles.y,
                actualTilt
                );

        tiltLastFrame = actualTilt;
    }

    void UpdateFOV()
    {
        if (Movement.playerMovement.isDashing || Movement.playerMovement.isDashing)
        {

        }

        else
        {

        }
    }
}
