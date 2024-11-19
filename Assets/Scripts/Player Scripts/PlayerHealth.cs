using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;

    [SerializeField][Tooltip("If true the player can't take damage during a dash")]
    private bool invincibleDashing = false;

    public GameObject healthUI1;
    public GameObject healthUI2;
    public GameObject healthUI3;

    void Start()
    {
        UpdateHealthUI();   
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(10, 0);
            Debug.Log("I took damage");
        }
        UpdateHealthUI();
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "EnemyHitbox")
    //    {
    //        EnemyHitbox hitbox = other.GetComponent<EnemyHitbox>();

    //        if (hitbox && hitbox.active)
    //        {
    //            TakeDamage(hitbox.damage, 0);
    //            hitbox.active = false;
    //        }
    //    }
    //}

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
        if (health < 75)
        {
            healthUI1.SetActive(true);
            if (health <= 50)
            {
                healthUI2.SetActive(true);

                if (health <= 25) healthUI3.SetActive(true);
            }
        }

        else
        {
             healthUI1.SetActive(false);
             healthUI2.SetActive(false);
             healthUI3.SetActive(false);
        }
    }
}
