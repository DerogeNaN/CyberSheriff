using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private float elapsedTime;
    private bool isTiming;

    public TMP_Text timerText;  // Assign a UI Text element in the Inspector
    public TMP_Text timerShadowText;

    void Start()
    {


        timerText = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        //timerText = GetComponentInChildren<TextMeshProUGUI>();
        //timerShadowText = GetComponentInChildren<TextMeshProUGUI>();
        timerShadowText = GameObject.Find("Time_Shadow").GetComponent<TextMeshProUGUI>();
    }
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
        int milliseconds = Mathf.FloorToInt((time * 100F) % 100F);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        timerShadowText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds); 
        
    }
}
