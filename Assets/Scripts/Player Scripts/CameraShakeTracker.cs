using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeTracker : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    public float cameraOffset = 0.0f;

    void Update()
    {
        camera.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.05f, transform.position.z);
        camera.transform.position -= camera.transform.forward * cameraOffset;
        camera.transform.rotation = transform.rotation;
    }
}
