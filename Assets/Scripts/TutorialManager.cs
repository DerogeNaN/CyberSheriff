using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private GameObject timerUI;
    private GameObject enemiesRemainingUI;
    private GameObject waveNumberUI;

    private void Awake()
    {
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
        timerUI.SetActive(false);
        enemiesRemainingUI.SetActive(false);
        waveNumberUI.SetActive(false);
    }
}
