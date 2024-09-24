using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    private Scoreboard scoreboard;

    [Header("Point Values")]
    public int revolverKillPoints = 100;
    public int revolverHeadshotPoints = 1000;
    public int revolverAltFirePoints = 100;
    public int shotgunKillPoints = 200;
    public int shotgunAltFirePoints = 300;
    public int jumpPoints = 50;
    public int DoubleJumpPoints = 50;
    public int WallRunningPoints = 1;
    public int GrapplePoints = 40;




    // Start is called before the first frame update
    void Start()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
    }

    public void AwardPoints(ActionType actionType)
    {
        int points = 0;

        switch (actionType)
        {
            case ActionType.Jump:
                points = jumpPoints;
                scoreboard.AddScore(points);
                //scoreboard.IncrementCombo();
                break;
            case ActionType.DoubleJump:
                points = DoubleJumpPoints;
                scoreboard.AddScore(points);
                //scoreboard.IncrementCombo();
                break;
            case ActionType.WallRunning:
                points = WallRunningPoints;
                scoreboard.AddRawScore(points);
                break;
            case ActionType.Grapple:
                points = GrapplePoints;
                scoreboard.AddScore(points);
                //scoreboard.IncrementCombo();
                break;
                // Add more cases as needed
        }
        //scoreboard.AddScore(points);
        //scoreboard.IncrementCombo();
    }
}
