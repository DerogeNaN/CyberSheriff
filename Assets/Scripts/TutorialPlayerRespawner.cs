using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerRespawner : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Movement.playerMovement.respawnPos = transform;
        }
    }
}
