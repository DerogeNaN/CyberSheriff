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
        Vector2 speedXZ = new Vector2(Movement.playerMovement.momentum.x, Movement.playerMovement.momentum.z);
        if (!showGravity) tmp.text = speedXZ.magnitude.ToString("F3");
        else tmp.text = Movement.playerMovement.momentum.y.ToString("F3");
    }
}
