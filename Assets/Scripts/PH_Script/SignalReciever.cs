using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalReciever : MonoBehaviour
{
    //private ScoringSystem scoringSystem;
    // Start is called before the first frame update
     void Start()
    {
       // scoringSystem = FindObjectOfType<ScoringSystem>();
    }
    void OnStartWalking()
    {

    }
    void OnDoubleJump()
    {
        //scoringSystem.AwardPoints(ActionType.DoubleJump);
    }

    void OnJump()
    {
        //scoringSystem.AwardPoints(ActionType.Jump); 

    }
    void OnStartWallrunning()
    {
        //  scoringSystem.AwardPoints(ActionType.WallRunning);
    }
}
