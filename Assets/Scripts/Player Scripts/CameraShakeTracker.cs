using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeTracker : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    public float cameraOffset = 0.0f;

    void Update()
    {
        cam.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.05f, transform.position.z);
        cam.transform.position -= cam.transform.forward * cameraOffset;
        cam.transform.rotation = transform.rotation;
    }
}
