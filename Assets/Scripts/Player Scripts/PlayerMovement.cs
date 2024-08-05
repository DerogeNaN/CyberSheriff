using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed;

    [Space(10)]
    [Header("Serializeable Variables")]
    [SerializeField] Rigidbody rb;

    [Space(10)]
    [Header("Testing Purposes")]
    public Transform orientation;

    // Local variables:
    PlayerInputActions inputActions;
    private Vector3 moveDirection;
    

    private void Start()
    {
        rb.freezeRotation = true;
        //if (inputActions == null)
        inputActions = gameObject.GetComponent<PlayerInputActions>();
        inputActions.Player.Enable();
    }

    public void UpdateMovement()
    {
        PlayerInput();
        MovePlayer();
    }

    void PlayerInput()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
    }

    void MovePlayer()
    {
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
    }
}
