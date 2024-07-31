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
    private bool isPullingObject = false;
    private Vector3 hookTarget;
    private Rigidbody rb;
    private Rigidbody targetRb;

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
            if (isPullingObject)
            {
                PullObject();
            }
            else
            {
                HookPull();
                
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DetachHook();
            }
        }
    }

    void FireHook()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, hookRange))
        {
            if (hit.collider.CompareTag("PullPlayer") || hit.collider.CompareTag("PullObject"))
            {
                hookTarget = hit.point;
                isHooked = true;
                hookLine.enabled = true;
                hookLine.SetPosition(0, hookOrigin.position);
                hookLine.SetPosition(1, hookTarget);

                if (hit.collider.CompareTag("PullObject"))
                {
                    isPullingObject = true;
                    targetRb = hit.collider.GetComponent<Rigidbody>();
                }
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

    void PullObject()
    {
        if (targetRb != null)
        {
            Vector3 direction = (transform.position - targetRb.position).normalized;
            targetRb.AddForce(direction * hookForce);

            hookLine.SetPosition(0, hookOrigin.position);
            hookLine.SetPosition(1, targetRb.position);

            if (Vector3.Distance(transform.position, targetRb.position) < 2f)
            {
                DetachHook();
            }
        }
    }

    void DetachHook()
    {
        isHooked = false;
        isPullingObject = false;
        targetRb = null;
        hookLine.enabled = false;
    }
}
