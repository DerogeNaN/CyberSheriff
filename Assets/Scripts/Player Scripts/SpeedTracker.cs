using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedTracker : MonoBehaviour
{
    [SerializeField] Movement movementScript;
    [SerializeField] TextMeshProUGUI tmp;

    
    void Update()
    {
        tmp.text = movementScript.momentum.magnitude.ToString();
    }
}
