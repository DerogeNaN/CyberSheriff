using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJiggle : MonoBehaviour
{
    public float maxMomentumInfluence = 1.0f;
    public float headTiltAtMax = 3.0f;
    public float tilt;

    void Update()
    {
        if (!Movement.playerMovement.isWallrunning && !Movement.playerMovement.isSliding)
        {
            tilt = Vector3.Dot(-transform.right, Vector3.ClampMagnitude(Movement.playerMovement.momentum, maxMomentumInfluence));
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
            tilt = Vector3.Dot(transform.right, Vector3.ClampMagnitude(Movement.playerMovement.momentum, maxMomentumInfluence));
            tilt /= maxMomentumInfluence;
            tilt *= headTiltAtMax;

            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                tilt
                );
        }
    }
}
