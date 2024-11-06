using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraJiggle : MonoBehaviour
{
    public float maxMomentumInfluence = 1.0f;
    public float headTiltAtMax = 3.0f;
    public float tilt;
    public float maxFOV = 90f;
    public float maxDashFOV = 110f;
    public float defaultFOV = 80f;
    public float currFOV;

    public AnimationCurve dashFOVCurve;
    public AnimationCurve defaultFOVCurve;


    
    void Update()
    {
        if (!Movement.playerMovement.isWallRunning && !Movement.playerMovement.isSliding && Time.time > Movement.playerMovement.cameraLeaveWallrunTime)
        {
            tilt = Vector3.Dot(-transform.right, Vector3.ClampMagnitude(Movement.playerMovement.velocity, maxMomentumInfluence));
            tilt /= maxMomentumInfluence;
            tilt *= headTiltAtMax;

            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                tilt
                );
        }

        else if (Movement.playerMovement.isSliding)
        {
            tilt = Vector3.Dot(transform.right, Vector3.ClampMagnitude(Movement.playerMovement.velocity, maxMomentumInfluence));
            tilt /= maxMomentumInfluence;
            tilt *= headTiltAtMax;

            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                tilt
                );
        }

        if (Movement.playerMovement.isDashing)
        {
            float fOVRatio = Mathf.InverseLerp(0, maxMomentumInfluence, Mathf.Abs(Vector3.Dot(Movement.playerMovement.velocity, Movement.playerMovement.transform.forward)));
            fOVRatio = defaultFOVCurve.Evaluate(fOVRatio);

            currFOV = Mathf.Lerp(defaultFOV, maxDashFOV, fOVRatio);

        }
        else
        {
            float fOVRatio = Mathf.InverseLerp(0, maxMomentumInfluence, Mathf.Abs(Vector3.Dot(Movement.playerMovement.velocity, Movement.playerMovement.transform.forward)));
            fOVRatio = defaultFOVCurve.Evaluate(fOVRatio);
            currFOV = Mathf.Lerp(defaultFOV, maxFOV, fOVRatio);
        }
        
        //Camera.main.fieldOfView = currFOV;
    }
}
