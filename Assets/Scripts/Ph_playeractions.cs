using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ph_playeractions : MonoBehaviour
{
    // Start is called before the first frame update
    public Scoreboard scoreboard;

    void Start()
    {
        // Assume the scoreboard is assigned via the Inspector or found in the scene.
    }

    void Update()
    {
        // Example actions
        if (Input.GetKeyDown(KeyCode.K)) // Enemy defeated
        {
            scoreboard.AddScore(100);
            scoreboard.IncrementCombo();
        }

        if (Input.GetKeyDown(KeyCode.J)) // Player takes damage
        {
            scoreboard.ResetCombo();
        }
    }
}
