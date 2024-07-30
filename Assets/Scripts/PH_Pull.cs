using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PH_Pull : MonoBehaviour
{
    public float hookSpeed = 20f;
    public float hookRange = 50f;
    public float hookForce = 50f;
    public LineRenderer hookLine;
    public Transform hookOrigin;

    private bool isHooked = false;
    private Vector3 hookTarget;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHooked)
            {
                DetachHook();
            }
            else
            {
                FireHook();
            }
        }

        if (isHooked)
        {
            HookPull();
        }
    }

    void FireHook()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, hookRange))
        {
            if (hit.collider.CompareTag("PullPlayer"))
            {
                hookTarget = hit.point;
                isHooked = true;
                hookLine.enabled = true;
                hookLine.SetPosition(0, hookOrigin.position);
                hookLine.SetPosition(1, hookTarget);
            }
        }
    }

    void HookPull()
    {
        Vector3 direction = (hookTarget - transform.position).normalized;
        rb.AddForce(direction * hookForce);

        hookLine.SetPosition(0, hookOrigin.position);
        hookLine.SetPosition(1, hookTarget);

        if (Vector3.Distance(transform.position, hookTarget) < 2f)
        {
            DetachHook();
        }
    }

    void DetachHook()
    {
        isHooked = false;
        hookLine.enabled = false;
    }
}
