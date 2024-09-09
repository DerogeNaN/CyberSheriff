using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletVFX : MonoBehaviour
{
    public float speed = 200;

    private void Update()
    {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        Destroy(gameObject,5);

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
