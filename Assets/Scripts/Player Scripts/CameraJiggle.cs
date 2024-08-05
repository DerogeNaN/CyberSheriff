using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJiggle : MonoBehaviour
{
    public float maxMomentumInfluence = 1.0f;
    public float headTiltAtMax = 5.0f;
    public float tilt;

    // Update is called once per frame
    void Update()
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
}
