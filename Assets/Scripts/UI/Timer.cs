using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeLeft;
    public bool isTiming = false;
    private bool isTimingBreak = false;

    public TMP_Text timerText;  // Assign a UI Text element in the Inspector
    public TMP_Text timerShadowText;

    void Start()
    {
        StartBreakTimer();

        timerText = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        //timerText = GetComponentInChildren<TextMeshProUGUI>();
        //timerShadowText = GetComponentInChildren<TextMeshProUGUI>();
        timerShadowText = GameObject.Find("Time_Shadow").GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (isTiming && !isTimingBreak)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                WaveManager.waveManagerInstance.LoseCondition();
            }
        }
        else if (isTimingBreak && !isTiming)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                WaveManager.waveManagerInstance.StartWave();
            }
        }
        UpdateTimerDisplay(timeLeft);
    }

    // Method to start the timer
    public void StartTimer()
    {
        isTiming = true;
        isTimingBreak = false;
        timeLeft = WaveManager.waveManagerInstance.waveTime;
    }

    // Method to stop the timer
    public void StopTimer()
    {
        isTiming = false;
    }

    public void StartBreakTimer()
    {
        isTimingBreak = true;
        timeLeft = WaveManager.waveManagerInstance.timeBetweenWaves;
        isTiming = false;
    }

    // Method to reset the timer (optional)
    public void ResetTimer()
    {
        timeLeft = WaveManager.waveManagerInstance.waveTime;
        UpdateTimerDisplay(timeLeft);
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
