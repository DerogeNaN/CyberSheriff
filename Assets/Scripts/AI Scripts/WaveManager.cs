using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager waveManagerInstance;

    [Header("Global Wave Settings")]
    public bool tutorialLevel;
    public float waveTime;
    public float timeBetweenWaves;
    public float maxWave;
    public float forceTargetTime;
    [Tooltip("minimum remaining wave time before all enemies automatically lock on to the player")]
    public float forceTargetTimeMin;
    [Tooltip("maximum remaining wave time before all enemies automatically lock on to the player")]
    public float forceTargetTimeMax;

    [Header("Global Wave Stats")]
    public int waveNumber = 0;
    public float timeLeftInWave; // use timerScript.timeLeft for time left in wave
    public int enemiesRemaining;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI enemiesRemainingShadow;
    [SerializeField] private TextMeshProUGUI waveCountText;
    [SerializeField] private TextMeshProUGUI waveCountShadow;

    [SerializeField] PauseMenu pauseMenuScript;
    [SerializeField] public Timer timerScript;

    public delegate void NewWaveEvent();
    public static event NewWaveEvent StartNewWave;


    void Awake()
    {
        if (waveManagerInstance == null)
        {
            waveManagerInstance = this;
        }
        else if (waveManagerInstance != this)
        {
            Destroy(gameObject);  // Avoid keeping duplicate managers
        }

        SetupWaveManager();
    }

    
    void Update()
    {
        enemiesRemainingShadow.text = enemiesRemainingText.text;
        waveCountShadow.text = waveCountText.text;
        
        if (!tutorialLevel)
        {
            if (enemiesRemainingText != null)
            {
                enemiesRemainingText.text = enemiesRemaining.ToString();
            }

            if (waveCountText != null) waveCountText.text = waveNumber.ToString();
        }

        if (Input.GetKeyDown(KeyCode.N)) StartWave();
    }

    private void SetupWaveManager()
    {
        if (enemiesRemainingText == null) enemiesRemainingText = GameObject.Find("Enemy_Amount_Text").GetComponent<TextMeshProUGUI>();
        if (enemiesRemainingShadow == null) enemiesRemainingShadow = GameObject.Find("Enemy_Amount_Shadow").GetComponent<TextMeshProUGUI>();
        if (waveCountText == null) waveCountText = GameObject.Find("Wave_Amount_Text").GetComponent<TextMeshProUGUI>();
        if (waveCountShadow == null) waveCountShadow = GameObject.Find("Wave_Amount_Shadow").GetComponent<TextMeshProUGUI>();

        if (timerScript == null) timerScript = GameObject.Find("Timer").GetComponent<Timer>();
        if (pauseMenuScript == null) pauseMenuScript = GameObject.Find("PauseMenuScript").GetComponent<PauseMenu>();
    }

    public void StartWave()
    {
        Debug.Log("Wave Manager starting new wave");
        if (waveNumber == 0)
        {
            SoundManager2.Instance.PlaySound("FirstWave");
        }
        else SoundManager2.Instance.PlaySound("OtherWaves");
       

        if (waveNumber > maxWave)
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
        StartNewWave = null;  // Unsubscribe from event ADDED THIS MAYBE THIS DID IT?
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        pauseMenuScript.loseScreen.SetActive(true);
        Movement.playerMovement.playerInputActions.Player.Disable();
        Movement.playerMovement.playerInputActions.UI.Enable();
    }
}