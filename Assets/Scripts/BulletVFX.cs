using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletVFX : MonoBehaviour
{
    [SerializeField]
    float lifeTime = 30;
    private void Update()
    {
        Destroy(gameObject,lifeTime);
    }
}
