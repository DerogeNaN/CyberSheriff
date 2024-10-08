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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(10, 0);
            Debug.Log("I took damage");
        }
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
        health -= damage;

        IsDestroyed();
        UpdateHealthUI();
    }

    public void IsDestroyed()
    {
        if (health <= 0)
        {
            Debug.Log("you died");
            // THIS NEEDS TO BE CHANGED BACK TO 0 HEALTH
            health = 100;
            Movement.playerMovement.transform.position = Movement.playerMovement.respawnPos.position;

            //Movement.playerMovement.playerInputActions.Disable();
        }
    }

    void UpdateHealthUI()
    {
        healthUI.text = "hp: " + health;
    }

   
}
