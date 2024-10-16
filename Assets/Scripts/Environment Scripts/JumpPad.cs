using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{   
    public float jumpPadStrength = 0.5f;

    public void JumpBoost()
    {
        //Debug.Log("Jump boost");
        Movement.playerMovement.isGrounded = false;
        Movement.playerMovement.velocity.y = jumpPadStrength;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) JumpBoost();
    }
}
