using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100;
    public TMP_Text debugText;

    public delegate void EnemyKillEvent();
    public static event EnemyKillEvent enemyKill;

    private void Update()
    {
        if (debugText)
        {
            debugText.text = "HP: " + health.ToString();
        }
    }

    public void TakeDamage(int damage, int damageType)
    {
        health -= damage;
        //Debug.Log("hit:" + gameObject.name + " damage:" + damage + " type:" + damageType);

        // destroys this gameobject if health <= 0
        IsDestroyed();
    }

    public void IsDestroyed()
    {
        if (health <= 0)
        {
            health = 0;
            //Debug.Log(gameObject.name + " was destroyed");
            Destroy(gameObject);
            if (WaveManager.waveManagerInstance != null) WaveManager.waveManagerInstance.enemiesRemaining--;
        }
    }

    private void OnDestroy()
    {
        enemyKill();
    }
}
