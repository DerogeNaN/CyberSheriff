using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerRespawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Movement.playerMovement.respawnPos = transform;
        }
    }
}
