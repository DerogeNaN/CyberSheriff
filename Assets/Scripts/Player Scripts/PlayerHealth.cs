using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;

    public float vFXFlashTime = 0.0f;

    public float iFrameTime = 0.0f;
    private float lastHurtTime = 0.0f;

    public int fallDamage = 0;

    [SerializeField][Tooltip("If true the player can't take damage during a dash")]
    private bool invincibleDashing = false;

    public GameObject healthUI1;
    public GameObject healthUI2;
    public GameObject healthUI3;
    public GameObject hitFlash;
    public GameObject healFlash;
    public Slider healthSlider;

    void Start()
    {
        UpdateHealthUI();   
    }
    void Update()
    {
        UpdateHealthUI();
        if (Input.GetKeyDown(KeyCode.O)) { TakeDamage(10, 0); }
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        SoundManager2.Instance.PlaySound("Health Pickup");
        HealFlashEffect();
    }

    public void TakeDamage(int damage, int damageType)
    {
        if (lastHurtTime + iFrameTime < Time.time)
        {
            if (Movement.playerMovement.isDashing && invincibleDashing) return;
            if (health - damage > 0) SoundManager2.Instance.PlaySound("PlayerGettingHit");
            health -= damage;
            PlayHurtSound();
            lastHurtTime = Time.time;
            DamageFlashEffect();
            IsDestroyed();
        }
    }

    #region FlashVFX
    public void HealFlashEffect()
    {
        healFlash.SetActive(true);
        Invoke(nameof(ClearHealFlashEffect), vFXFlashTime);
    }
    
    private void ClearHealFlashEffect()
    {
        healFlash.SetActive(false);
    }

    public void DamageFlashEffect()
    {
        hitFlash.SetActive(true);
        Invoke(nameof(ClearDamageFlashEffect), vFXFlashTime);
    }

    private void ClearDamageFlashEffect()
    {
        hitFlash.SetActive(false);
    }
    #endregion

    public void IsDestroyed()
    {
        if (health <= 0)
        {
            SoundManager2.Instance.PlaySound("PlayerDeath");
            SoundManager2.Instance.StopMusic("Gameplay Track 1");
            SoundManager2.Instance.StopMusic("Gameplay Track 2");
            SoundManager2.Instance.StopMusic("Gameplay Track 3");
            WaveManager.waveManagerInstance.LoseCondition();
        }
    }

    void PlayHurtSound()
    {
        if (health >= 75)
        {
            SoundManager2.Instance.StopSound("WoundedHealth");
            SoundManager2.Instance.StopSound("InjuredHealth");
            SoundManager2.Instance.StopSound("CriticalHealth");
        }

        else if (health >= 50 && health < 75) 
        {
            SoundManager2.Instance.StopSound("WoundedHealth");
            SoundManager2.Instance.PlaySound("WoundedHealth");
            SoundManager2.Instance.StopSound("InjuredHealth");
            SoundManager2.Instance.StopSound("CriticalHealth");
        }

        else if (health >= 25 && health < 50)
        {
            SoundManager2.Instance.StopSound("InjuredHealth");
            SoundManager2.Instance.PlaySound("InjuredHealth");
            SoundManager2.Instance.StopSound("WoundedHealth");
            SoundManager2.Instance.StopSound("CriticalHealth");
        }

        else if (health < 25 && health > 0)
        {
            SoundManager2.Instance.StopSound("CriticalHealth");
            SoundManager2.Instance.PlaySound("CriticalHealth");
            SoundManager2.Instance.StopSound("WoundedHealth");
            SoundManager2.Instance.StopSound("InjuredHealth");
        }

        else
        {
            SoundManager2.Instance.StopSound("WoundedHealth");
            SoundManager2.Instance.StopSound("InjuredHealth");
            SoundManager2.Instance.StopSound("CriticalHealth");
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
