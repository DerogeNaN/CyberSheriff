using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Scoreboard : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text rankText;
    public TMP_Text comboText;
    public Image decayBar;
    public GameObject scoreboardPanel; // Reference to the scoreboard panel to show/hide

    private int score;
    private int comboMultiplier;
    private string rank;

    private float decayTimer;
    public float decayInterval = 10f; // Time in seconds before rank decays
    private float decayBarMaxWidth;

    // Rank Colors
    public Color sRankColor = Color.yellow;
    public Color aRankColor = Color.red;
    public Color bRankColor = Color.magenta;
    public Color cRankColor = Color.blue;
    public Color dRankColor = Color.green;
    public Color eRankColor = Color.gray;

    private PulsateEffect rankTextPulsateEffect;
    private ScoreShake scoreTextShakeEffect;


    void Start()
    {
        score = 0;
        comboMultiplier = 1;
        rank = "E";
        decayTimer = decayInterval;
        decayBarMaxWidth = decayBar.rectTransform.sizeDelta.x;
        rankTextPulsateEffect = rankText.GetComponent<PulsateEffect>();
        scoreTextShakeEffect = scoreText.GetComponent<ScoreShake>();
        UpdateScoreboard();
    }

    void Update()
    {
        if (score > 0 || comboMultiplier > 1)
        {
            // Update the decay timer
            decayTimer -= Time.deltaTime;

            // Update the decay bar
            decayBar.rectTransform.sizeDelta = new Vector2((decayTimer / decayInterval) * decayBarMaxWidth, decayBar.rectTransform.sizeDelta.y);

            // Check if the timer has run out
            if (decayTimer <= 0)
            {
                DecayRank();
                decayTimer = decayInterval; // Reset the timer
            }
        }
        else
        {
            HideScoreboard();
        }
    }
    void UpdateScoreboard()
    {
        if (score > 0 || comboMultiplier > 1)
        {
            ShowScoreboard();
            scoreText.text = "" + score;
            rankText.text = rank;
            rankText.color = GetRankColor(rank); // Set the rank text color
            comboText.text = comboMultiplier + "x";
        }
        else
        {
            HideScoreboard();
        }
    }

    public void AddScore(int amount)
    {
        score += amount * comboMultiplier;
        decayTimer = decayInterval; // Reset the timer when score is added
        UpdateRank();
        UpdateScoreboard();
    }
    public void AddRawScore(int amount)
    {
        score += amount;
        decayTimer = decayInterval; // Reset the timer when score is added
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
    public void ResetScore()
    {
        score = 0;
        UpdateScoreboard();
    }
    public void ScoreDeduction()
    {
        score = score/2;
    }



    void UpdateRank()
    {
        string previousRank = rank;

        if (score > 10000)
            rank = "S";
        else if (score > 5000)
            rank = "A";
        else if (score > 2000)
            rank = "B";
        else if (score > 1000)
            rank = "C";
        else if (score > 500)
            rank = "D";
        else
            rank = "E";
        if (previousRank != rank)
        {
            rankTextPulsateEffect.StartPulsateAndShake();
        }
    }

    void DecayRank()
    {
        switch (rank)
        {
            case "S":
                rank = "A";
                ScoreDeduction();
                break;
            case "A":
                rank = "B";
                ScoreDeduction();
                break;
            case "B":
                rank = "C";
                ScoreDeduction();
                break;
            case "C":
                rank = "D";
                ScoreDeduction();
                break;
            case "D":
                rank = "E";
                ScoreDeduction();
                break;
            case "E":
                ResetScore();
                break;
                
        }
        ResetCombo();
        //ScoreDeduction();
        scoreTextShakeEffect.StartScorePulsateAndShake();
        UpdateScoreboard();
        
    }
    Color GetRankColor(string rank)
    {
        switch (rank)
        {
            case "S":
                return sRankColor;
            case "A":
                return aRankColor;
            case "B":
                return bRankColor;
            case "C":
                return cRankColor;
            case "D":
                return dRankColor;
            case "E":
                return eRankColor;
            default:
                return Color.white;
        }
    }
    void ShowScoreboard()
    {
        scoreboardPanel.SetActive(true);
    }

    void HideScoreboard()
    {
        scoreboardPanel.SetActive(false);
    }
}
