using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public int damage;
    public float activeTime;

    private float time;

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= activeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, 0);
            Destroy(gameObject);
        }
    }
}
