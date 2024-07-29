using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Scoreboard : MonoBehaviour
{
    public Text scoreText;
    public Text rankText;
    public Text comboText;

    private int score;
    private int comboMultiplier;
    private string rank;

    void Start()
    {
        score = 0;
        comboMultiplier = 1;
        rank = "D";
        UpdateScoreboard();
    }

    void UpdateScoreboard()
    {
        scoreText.text = "Score: " + score;
        rankText.text = "Rank: " + rank;
        comboText.text = "Combo: " + comboMultiplier + "x";
    }

    public void AddScore(int amount)
    {
        score += amount * comboMultiplier;
        UpdateRank();
        UpdateScoreboard();
    }

    public void IncrementCombo()
    {
        comboMultiplier++;
        UpdateScoreboard();
    }

    public void ResetCombo()
    {
        comboMultiplier = 1;
        UpdateScoreboard();
    }

    void UpdateRank()
    {
        if (score > 10000)
            rank = "S";
        else if (score > 5000)
            rank = "A";
        else if (score > 2000)
            rank = "B";
        else if (score > 1000)
            rank = "C";
        else
            rank = "D";
    }
}
