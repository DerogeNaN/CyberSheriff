using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;

    [SerializeField] TMP_Text healthUI;

    void Start()
    {
        UpdateHealthUI();   
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyHitbox")
        {
            EnemyHitbox hitbox = other.GetComponent<EnemyHitbox>();

            if (hitbox && hitbox.active)
            {
                TakeDamage(hitbox.damage, 0);
                Destroy(hitbox.gameObject);
            }
        }
    }

    public void TakeDamage(int damage, int damageType)
    {
        health -= damage;

        IsDestroyed();
        UpdateHealthUI();
    }

    public void IsDestroyed()
    {
        if (health <= 0)
        {
            Debug.Log("you died");
            health = 0;

            Movement.playerMovement.playerInputActions.Disable();
        }
    }

    void UpdateHealthUI()
    {
        healthUI.text = "hp: " + health;
    }

}
