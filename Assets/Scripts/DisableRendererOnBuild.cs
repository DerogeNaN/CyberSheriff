using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRendererOnBuild : MonoBehaviour
{
    void Awake()
    {
        // Check if the application is NOT running in the Unity Editor
        if (!Application.isEditor)
        {
            // Disable the MeshRenderer
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
    }
}
