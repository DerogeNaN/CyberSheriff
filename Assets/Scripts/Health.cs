using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int damage, int damageType)
    {
        health -= damage;
        Debug.Log("hit:" + gameObject.name + " damage:" + damage + " type:" + damageType);

        // destroys this gameobject if health <= 0
        IsDestroyed();
    }

    public void IsDestroyed()
    {
        if (health <= 0)
        {
            Debug.Log(gameObject.name + " was destroyed");
            Destroy(gameObject);
        }
    }
}
