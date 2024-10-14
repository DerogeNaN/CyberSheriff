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

    private float score;
    private float comboMultiplier;
    private string rank;

    //Internal Scorebar Progression
    private int rankPointsMax = 100;
    private int rankPointsCurrent = 0;

    [Header("Rank up Balancing")] // these values control how much your decay bar jumps up when you score points.
    [SerializeField] private int minorAction = 10;
    [SerializeField] private int midAction = 20;
    [SerializeField] private int majorAction = 50;

    [Header("Score Bonus On Rank Decay")]
    [SerializeField] private int eRankBonus = 5;
    [SerializeField] private int dankBonus = 50;
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
    [SerializeField] private float eRankHandCap = 1.0f;
    [SerializeField] private float dRankHandCap = 0.9f;
    [SerializeField] private float cRankHandCap = 0.8f;
    [SerializeField] private float bRankHandCap = 0.7f;
    [SerializeField] private float aRankHandCap = 0.6f;
    [SerializeField] private float sRankHandCap = 0.5f;


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
            scoreShadowText.text = "" + score;
            rankText.text = rank;
            rankShadowText.text = rank;
            rankText.color = GetRankColor(rank); // Set the rank text color

            comboText.text = comboMultiplier + "x";
            comboShadowText.text = comboMultiplier + "x";
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

    public void IncrementMuiltiplier()
    {
        comboMultiplier++;
        UpdateScoreboard();
    }

    public void ResetMuiltiplier()
    {
        comboMultiplier = eRankMulti;
        UpdateScoreboard();
    }
    public void ResetScore()
    {
        score = 0;
        UpdateScoreboard();
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
                scoreTextShakeEffect.StartScorePulsateAndShake();
                Debug.Log("S to A");
                break;
            case "A":
                rank = "B";

                scoreTextShakeEffect.StartScorePulsateAndShake();
                Debug.Log("A to B");
                break;
            case "B":
                rank = "C";

                scoreTextShakeEffect.StartScorePulsateAndShake();
                Debug.Log("B to C");
                break;
            case "C":
                rank = "D";

                scoreTextShakeEffect.StartScorePulsateAndShake();
                Debug.Log("C to D");
                break;
            case "D":
                rank = "E";

                scoreTextShakeEffect.StartScorePulsateAndShake();
                Debug.Log("D to E");
                break;
            case "E":
                //ResetScore();
                Debug.Log("Reset Score");
                break;
                
        }
        ResetMuiltiplier();
        //ScoreDeduction();
        //scoreTextShakeEffect.StartScorePulsateAndShake();
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
