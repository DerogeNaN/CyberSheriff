using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PH_Push : MonoBehaviour
{
    public float pushForce = 1000f;
    public float pushRange = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Push();
        }
    }

    void Push()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, pushRange))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = (hit.point - transform.position).normalized;
                rb.AddForce(pushDirection * pushForce);
            }
        }
    }
}
