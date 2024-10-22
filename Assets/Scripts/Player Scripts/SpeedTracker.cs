using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedTracker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    public bool showGravity;
    
    void Update()
    {
        Vector2 speedXZ = new Vector2(Movement.playerMovement.velocity.x, Movement.playerMovement.velocity.z);
        if (!showGravity) tmp.text = speedXZ.magnitude.ToString("F3");
        else tmp.text = Movement.playerMovement.velocity.y.ToString("F3");
    }
}
