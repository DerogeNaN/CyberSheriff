using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedTracker : MonoBehaviour
{
    [SerializeField] Movement movementScript;
    [SerializeField] TextMeshProUGUI tmp;
    public bool showGravity;
    
    void Update()
    {
        if (!showGravity) tmp.text = movementScript.momentum.magnitude.ToString("F3");
        else tmp.text = movementScript.momentum.y.ToString("F3");
    }
}
