using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5.0f, ~12, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(hit.point, hit.normal * 2, Color.cyan);
        }
        else Debug.Log("Raycast failing");

        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.magenta);
    }
}
