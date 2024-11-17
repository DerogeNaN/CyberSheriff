using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeTracker : MonoBehaviour
{
    [SerializeField] private GameObject camera;

    void Update()
    {
        camera.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.05f, transform.position.z);
        camera.transform.rotation = transform.rotation;
    }
}
