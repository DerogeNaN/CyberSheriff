using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;

    [SerializeField] TMP_Text healthUI;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyHitbox")
        {
            EnemyHitbox hitbox = other.GetComponent<EnemyHitbox>();

            if (hitbox)
            {
                TakeDamage(hitbox.damage, 0);
                Destroy(hitbox.gameObject);
            }
        }
    }

    public void TakeDamage(int damage, int damageType)
    {
        Debug.Log(health + " before");

        health -= damage;

        IsDestroyed();
        UpdateHealthUI();

        Debug.Log(health + " after");

    }

    public void IsDestroyed()
    {
        Debug.Log("checking hp");
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
