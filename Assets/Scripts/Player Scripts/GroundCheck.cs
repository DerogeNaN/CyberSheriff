using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 2) return;
        //TODO: ADD GROUND LAYER SO THAT THE PLAYER CANT JUMP OFF ENEMIES
        Movement.playerMovement.isGrounded = true;
        //Vector3 momentumDir = Vector3.Cross(other.transform.up, transform.right);
        //Movement.playerMovement.momentum = momentumDir * Movement.playerMovement.momentum.magnitude;
        Movement.playerMovement.lastGroundedTime = Time.time;
        Movement.playerMovement.jumpCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 2) return;
        if (other.gameObject.CompareTag("ResetBox"))
        {
            Movement.playerMovement.transform.position = Movement.playerMovement.respawnPos.position;
            Movement.playerMovement.transform.rotation = Movement.playerMovement.respawnPos.rotation;
            Movement.playerMovement.momentum = new Vector3(0, 0, 0);
        }
        //TODO: ADD GROUND LAYER SO THAT THE PLAYER CANT JUMP OFF ENEMIES
        Movement.playerMovement.isGrounded = true;
        //Vector3 momentumDir = Vector3.Cross(other.transform.up, transform.right);
        //Movement.playerMovement.momentum = momentumDir * Movement.playerMovement.momentum.magnitude;
        Movement.playerMovement.lastGroundedTime = Time.time;
        Movement.playerMovement.jumpCount = 0;
    }
}
