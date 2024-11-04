using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PH_DebugScoring : MonoBehaviour
{
    private ScoringSystem scoringSystem;

    void Start()
    {
        scoringSystem = FindObjectOfType<ScoringSystem>();
    }
     void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            scoringSystem.AwardPoints(ActionType.Jump);
        }
    }
}