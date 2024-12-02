using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Cooldown : MonoBehaviour
{
    [SerializeField]
    public Slider radialSlider;
    [SerializeField]
    float grappleTime;
    // Update is called once per frame
    void Update()
    {
        grappleTime =Movement.playerMovement.grappleCooldown + Movement.playerMovement.lastGrappleTime/Time.time;
        radialSlider.value = grappleTime;
    }
}
