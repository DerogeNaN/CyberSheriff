using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] EnemyHitbox hitbox;

    private void Update()
    {
        if (!hitbox.active)
        {
            Destroy(gameObject);
        }
    }
}
