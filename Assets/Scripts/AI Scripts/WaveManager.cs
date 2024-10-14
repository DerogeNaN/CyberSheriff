using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public delegate void NewWaveEvent();
    public static event NewWaveEvent StartNewWave;


    void Awake()
    {
        waveManagerInstance = this;
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartWave();
        }
    }

    public void StartWave()
    {
        Debug.Log("Wave Manager starting new wave");
        if (waveNumber == 3)
        {
            Debug.Log("Start end game loop");
            StartNewWave();
        }
        else
        {
            StartNewWave();
            waveNumber++;
        }

    }
}