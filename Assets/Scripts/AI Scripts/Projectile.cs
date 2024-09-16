using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    [SerializeField] EnemyHitbox hitbox;

    private void Start()
    {
        // should already be normalised but just in case
        Vector3.Normalize(direction);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (!hitbox.active)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
