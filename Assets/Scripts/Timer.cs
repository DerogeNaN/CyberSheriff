using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private float elapsedTime;
    private bool isTiming;

    public TMP_Text timerText;  // Assign a UI Text element in the Inspector

    void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay(elapsedTime);
        }
    }

    // Method to start the timer
    public void StartTimer()
    {
        isTiming = true;
    }

    // Method to stop the timer
    public void StopTimer()
    {
        isTiming = false;
    }

    // Method to reset the timer (optional)
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay(elapsedTime);
    }

    // Format and display the timer
    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F);

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
