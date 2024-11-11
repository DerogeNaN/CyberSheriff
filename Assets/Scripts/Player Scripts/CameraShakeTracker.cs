using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeTracker : MonoBehaviour
{
    [SerializeField] private GameObject camera;

    void Update()
    {
        camera.transform.position = transform.position;
        camera.transform.rotation = transform.rotation;
    }
}
