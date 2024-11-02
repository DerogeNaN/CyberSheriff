using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Scoreboard : MonoBehaviour
{
    [Header ("Scoreboard visuals")]
    public TMP_Text scoreText;
    public TMP_Text scoreShadowText;
    public TMP_Text rankText;
    public TMP_Text rankShadowText;
    public TMP_Text comboText;
    public TMP_Text comboShadowText;
    public RawImage decayBar;
    public GameObject scoreboardPanel; // Reference to the scoreboard panel to show/hide

    public TMP_Text debugRankValue; // temporary Debug value for rank tracking.

    private float score;
    private float comboMultiplier;
    private string rank;

    //Internal Scorebar Progression
    private int rankPointsMax = 100;
    private int rankPointsCurrent = 0;

    [Header("Rank up Balancing")] // these values control how much your decay bar jumps up when you score points.
    

    [Header("Score Bonus On Rank Decay")]
    [SerializeField] private int eRankBonus = 5;
    [SerializeField] private int dRankBonus = 50;
    [SerializeField] private int cRankBonus = 200;
    [SerializeField] private int bRankBonus = 500;
    [SerializeField] private int aRankBonus = 1000;
    [SerializeField] private int sRankBonus = 2000;

    [Header("Score Multipliers")]
    [SerializeField] private float eRankMulti = 0.5f;
    [SerializeField] private float dRankMulti = 1.0f;
    [SerializeField] private float cRankMulti = 1.1f;
    [SerializeField] private float bRankMulti = 1.3f;
    [SerializeField] private float aRankMulti = 1.6f;
    [SerializeField] private float sRankMulti = 1.7f;

    [Header("Rank Handycap")]
    [SerializeField] private float eRankHandicap = 1.0f;
    [SerializeField] private float dRankHandicap = 0.8f;
    [SerializeField] private float cRankHandicap = 0.6f;
    [SerializeField] private float bRankHandicap = 0.4f;
    [SerializeField] private float aRankHandicap = 0.2f;
    [SerializeField] private float sRankHandicap = 0.1f;


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
        comboMultiplier = eRankMulti;
        rank = "E";
        decayTimer = decayInterval;
        decayBarMaxWidth = decayBar.rectTransform.sizeDelta.x;
      

        rankTextPulsateEffect = scoreboardPanel.GetComponentInChildren<PulsateEffect>();
        scoreTextShakeEffect = scoreboardPanel.GetComponentInChildren<ScoreShake>();


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
            //decayBar.color = GetRankColor(rank);
        }
        else
        {
          // HideScoreboard();
        }
    }
    void UpdateScoreboard()
    {
        if (score > 0 || comboMultiplier > 1)
        {
            ShowScoreboard();
            scoreText.text = "" + score;
            scoreShadowText.text = "" + score;
            rankText.text = rank;
            rankShadowText.text = rank;
            rankText.color = GetRankColor(rank); // Set the rank text color

            comboText.text = comboMultiplier + "x";
            comboShadowText.text = comboMultiplier + "x";
            debugRankValue.text = "rankpoints" + rankPointsCurrent;
        }
        else
        {
           //HideScoreboard();
        }
    }

    public void AddScore(int amount)
    {
        /*score += amount * comboMultiplier;
        decayTimer = decayInterval; // Reset the timer when score is added
        UpdateRank();
        UpdateScoreboard();*/
        int adjustedPoints = Mathf.RoundToInt(amount * comboMultiplier);
        score += adjustedPoints;

        //UpdateRankPoints(minorAction); // You can pass midAction or majorAction based on the action type
        decayTimer = decayInterval; // Reset decay bar
        UpdateScoreboard();
    }
    /*public void AddRawScore(int amount)
    {
        score += amount;
        decayTimer = decayInterval; // Reset the timer when score is added
        UpdateRank();
        UpdateScoreboard();
    }*/

    public void ResetMultiplier() // general multiplier reset
    {
        comboMultiplier = eRankMulti;
        UpdateScoreboard();
    }
    public void ResetScore()  //For level resetting.
    {
        score = 0;
        UpdateScoreboard();
    }

    public void UpdateRankPoints(int actionPoints)
    {
        rankPointsCurrent += Mathf.RoundToInt(actionPoints * GetRankHandicap());

        if (rankPointsCurrent >= rankPointsMax)
        {
            PromoteRank();
        }

        UpdateScoreboard();
    }

    void PromoteRank()
    {
        switch (rank)
        {
            case "E": rank = "D"; comboMultiplier = dRankMulti; break;
            case "D": rank = "C"; comboMultiplier = cRankMulti; break;
            case "C": rank = "B"; comboMultiplier = bRankMulti; break;
            case "B": rank = "A"; comboMultiplier = aRankMulti; break;
            case "A": rank = "S"; comboMultiplier = sRankMulti; break;
            case "S":
                comboMultiplier += 0.1f; // Increase multiplier further
                break;
        }

        rankPointsCurrent = 0; // Reset rank points  // THIS MIGHT BE A PROBLEM
        rankTextPulsateEffect.StartPulsateAndShake();
    }
    void DecayRank()
    {
        // Regardless of rank, reset to E and award the bonus for the current rank
        AwardRankBonus();            // Award bonus based on the current rank
        rank = "E";                  // Reset to E rank
        rankPointsCurrent = 0;        // Reset rank points
        ResetMultiplier();  
        // Reset multiplier to E rank's multiplier
        rankTextPulsateEffect.StartPulsateAndShake();  // Visual effect on rank change
        UpdateScoreboard();
    }
    void AwardRankBonus()
    {
        switch (rank)
        {
            case "S": score += sRankBonus; break;
            case "A": score += aRankBonus; break;
            case "B": score += bRankBonus; break;
            case "C": score += cRankBonus; break;
            case "D": score += dRankBonus; break;
            case "E": score += eRankBonus; break;
        }
    }

    float GetRankHandicap()
    {
        switch (rank)
        {
            case "S": return sRankHandicap;
            case "A": return aRankHandicap;
            case "B": return bRankHandicap;
            case "C": return cRankHandicap;
            case "D": return dRankHandicap;
            case "E": return eRankHandicap;
            default: return 1.0f;
        }
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
