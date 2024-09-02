using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPromptScript : MonoBehaviour
{
    public GameObject hookPromptPrefab;
    public float hookRange = 50f;

    private GameObject currentPrompt;

    void Update()
    {
        CheckHookable();
    }

    void CheckHookable()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, hookRange))
        {
            if (hit.collider.CompareTag("PullPlayer"))
            {
                if (currentPrompt == null)
                {
                    currentPrompt = Instantiate(hookPromptPrefab, hit.point, Quaternion.identity);
                    currentPrompt.transform.SetParent(hit.collider.transform);
                }
                else
                {
                    currentPrompt.transform.position = hit.point;
                }
            }
            else
            {
                DestroyCurrentPrompt();
            }
        }
        else
        {
            DestroyCurrentPrompt();
        }
    }

    void DestroyCurrentPrompt()
    {
        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
        }
    }
}
