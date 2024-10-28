using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float speed;

    private float despawnTime = 5.0f;
    private Vector3 direction;

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        despawnTime -= Time.deltaTime;
        if (despawnTime <= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, 0);
            Destroy(gameObject);
        }
        else if (!other.gameObject.CompareTag("EnemyHitbox"))
        {
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector3 target)
    {
        direction = Vector3.Normalize(target - transform.position);
    }
}
