using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        //TODO: ADD GROUND LAYER SO THAT THE PLAYER CANT JUMP OFF ENEMIES
        Movement.playerMovement.isGrounded = true;
        //Vector3 momentumDir = Vector3.Cross(other.transform.up, transform.right);
        //Movement.playerMovement.momentum = momentumDir * Movement.playerMovement.momentum.magnitude;
        Movement.playerMovement.lastGroundedTime = Time.time;
        Movement.playerMovement.jumpCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: ADD GROUND LAYER SO THAT THE PLAYER CANT JUMP OFF ENEMIES
        Movement.playerMovement.isGrounded = true;
        //Vector3 momentumDir = Vector3.Cross(other.transform.up, transform.right);
        //Movement.playerMovement.momentum = momentumDir * Movement.playerMovement.momentum.magnitude;
        Movement.playerMovement.lastGroundedTime = Time.time;
        Movement.playerMovement.jumpCount = 0;
    }
}
