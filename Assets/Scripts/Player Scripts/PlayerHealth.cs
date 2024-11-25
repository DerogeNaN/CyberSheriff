using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;

    [SerializeField][Tooltip("If true the player can't take damage during a dash")]
    private bool invincibleDashing = false;

    public GameObject healthUI1;
    public GameObject healthUI2;
    public GameObject healthUI3;
    public Slider healthSlider;

    void Start()
    {
        UpdateHealthUI();   
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(10, 0);
        }
        UpdateHealthUI();
    }


    public void TakeDamage(int damage, int damageType)
    {
        if (Movement.playerMovement.isDashing && invincibleDashing) return;
        health -= damage;

        IsDestroyed();
       
    }

    public void IsDestroyed()
    {
        if (health <= 0)
        {
            Debug.Log("you died");
            // THIS NEEDS TO BE CHANGED BACK TO 0 HEALTH
            WaveManager.waveManagerInstance.LoseCondition();

            //Movement.playerMovement.playerInputActions.Disable();
        }
    }

    void UpdateHealthUI()
    {
        healthSlider.value = health;

        if (health < 75)
        {
            healthUI1.SetActive(true);
            if (health <= 50)
            {
                healthUI2.SetActive(true);

                if (health <= 25) healthUI3.SetActive(true);
            }
        }

        if (health > 25)
        {
            healthUI3.SetActive(false);
            if (health > 50)
            {
                healthUI2.SetActive(false);

                if (health > 75) healthUI1.SetActive(false);
            }
        }
    }
}
