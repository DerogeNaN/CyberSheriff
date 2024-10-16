using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    GameManager gameManager;
    public static WaveManager waveManagerInstance;

    [Header("Global Wave Settings")]
    public float waveTime;
    public float timeBetweenWaves;

    [Header("Global Wave Stats")]
    public int waveNumber = 0;
    public float timeLeftInWave;
    public int enemiesRemaining;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI waveCountText;

    [SerializeField] PauseMenu pauseMenuScript;
    [SerializeField] public Timer timerScript;

    public delegate void NewWaveEvent();
    public static event NewWaveEvent StartNewWave;


    void Awake()
    {
        waveManagerInstance = this;
    }

    
    void Update()
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = enemiesRemaining.ToString();
        }

        if (waveCountText != null) waveCountText.text = waveNumber.ToString();
    }

    public void StartWave()
    {
        Debug.Log("Wave Manager starting new wave");
        if (enemiesRemaining <= 0)
        {
            if (waveNumber > 2)
            {
                WinCondition();
            }
            else
            {
                StartNewWave();
                timerScript.StartTimer();
                waveNumber++;
            }
        }
        else LoseCondition();
    }

    public void WinCondition()
    {
        waveNumber = 0;
        enemiesRemaining = 0;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        pauseMenuScript.winScreen.SetActive(true);
        Movement.playerMovement.playerInputActions.Player.Disable();
        Movement.playerMovement.playerInputActions.UI.Enable();
    }

    public void LoseCondition()
    {
        waveNumber = 0;
        enemiesRemaining = 0;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        pauseMenuScript.loseScreen.SetActive(true);
        Movement.playerMovement.playerInputActions.Player.Disable();
        Movement.playerMovement.playerInputActions.UI.Enable();
    }
}