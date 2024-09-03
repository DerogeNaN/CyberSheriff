using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{   
    public float jumpPadStrength = 0.5f;

    public void JumpBoost()
    {
        Movement.playerMovement.momentum.y += jumpPadStrength;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player")) JumpBoost();
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) JumpBoost();
    }
}
