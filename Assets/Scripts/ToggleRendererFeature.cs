using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToggleRendererFeature : MonoBehaviour
{
    // Reference the Universal Renderer Data
    public UniversalRendererData rendererData;

    // The index of the feature you want to toggle
    public int featureIndex;

    private bool featureEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        // Toggle the specified Renderer Feature
        ToggleFeature(rendererData, featureIndex, false); // Disable it initially
    }

    // Update is called once per frame
    void Update()
    {
        // Press the "T" key to toggle the renderer feature on/off
        if (Input.GetKeyDown(KeyCode.T))
        {
            featureEnabled = !featureEnabled;
            ToggleFeature(rendererData, featureIndex, featureEnabled);
            Debug.Log("Renderer Feature Toggled: " + featureEnabled);
        }
    }

    public void ToggleFeature(UniversalRendererData rendererData, int index, bool isEnabled)
    {
        if (rendererData == null || index < 0 || index >= rendererData.rendererFeatures.Count)
        {
            Debug.LogError("Invalid Renderer Data or Feature Index.");
            return;
        }
        // Access the feature at the given index
        ScriptableRendererFeature feature = rendererData.rendererFeatures[index];

        // Enable or disable the feature
        feature.SetActive(isEnabled);
    }
}
