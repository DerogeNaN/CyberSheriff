using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;

    private void Start()
    {
        //playerInput = new PlayerInputActions();
        //
        //playerInput.Player.Enable();
    }

    private void Update()
    {
        // get mouse input
        float mouseX = Movement.playerMovement.playerInputActions.Player.Look.ReadValue<Vector2>().x * multiplier;
        float mouseY = Movement.playerMovement.playerInputActions.Player.Look.ReadValue<Vector2>().x * multiplier;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
