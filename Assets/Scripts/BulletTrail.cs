using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{

    public float liveTime= 0.1f;

    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("Bullet");
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject,0.1f);
    }

}
