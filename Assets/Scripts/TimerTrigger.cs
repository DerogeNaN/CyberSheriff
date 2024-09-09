using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
    public Timer timer;  // Assign the Timer script in the Inspector
    public bool startTrigger;

    private void Start()
    {
        timer = FindObjectOfType<Timer>();
    }
    // Detect when the player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure this only activates when the player enters
        {
            if (startTrigger)
                timer.StartTimer();
            else
                timer.StopTimer();
        }
    }
}
