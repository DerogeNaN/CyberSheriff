using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Cooldown : MonoBehaviour
{
    [SerializeField]
    public Slider radialSlider;
    [SerializeField]
    float grappleSliderValue;
    [SerializeField]
    float time;

    [SerializeField]
    float grappleCooldown;

    [SerializeField]
    float LastGrappleTime;

    // Update is called once per frame
    void Update()
    {

        time = Time.time;
        grappleCooldown = Movement.playerMovement.grappleCooldown;
        LastGrappleTime = Movement.playerMovement.lastGrappleTime;
        grappleSliderValue = -(LastGrappleTime - Time.time)/grappleCooldown;
        radialSlider.value = grappleSliderValue;
    }
}
