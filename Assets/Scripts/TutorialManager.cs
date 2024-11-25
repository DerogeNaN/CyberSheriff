using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private GameObject scoreUI;
    private GameObject timerUI;
    private GameObject enemiesRemainingUI;
    private GameObject waveNumberUI;

    private void Awake()
    {
        scoreUI = GameObject.Find("Score");
        timerUI = GameObject.Find("Timer");
        enemiesRemainingUI = GameObject.Find("Enemy");
        waveNumberUI = GameObject.Find("Waves");
    }

    void Start()
    {
        DisableUI();
    }

    
    void Update()
    {
        
    }

    private void DisableUI()
    {
        scoreUI.SetActive(false);
        timerUI.SetActive(false);
        enemiesRemainingUI.SetActive(false);
        waveNumberUI.SetActive(false);
    }
}
