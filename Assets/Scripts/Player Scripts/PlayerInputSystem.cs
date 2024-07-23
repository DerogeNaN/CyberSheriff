using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    //Design friendly variables:
    [Header("Player Stats")]
    public int health = 100;
    public float moveSpeed = 5.0f;

    //Component serialization
    [Space(10.0f)]
    [Header("Serializeable Fields")]
    [SerializeField] Camera playerCam;
    [SerializeField] PlayerInput playerInput;
    
    private PlayerInputActions controls;
    private PlayerInputActions.GroundMovementActions groundControls;

    private void Awake()
    {
        controls = new PlayerInputActions();
    }
}