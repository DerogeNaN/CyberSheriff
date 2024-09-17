using Cyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class AccessRendererFeature : MonoBehaviour
{
    public UniversalRendererData rendererData;
    public int featureIndex;

    // New material to assign to the blit feature
    public Material dashMaterial;
    public Material bloodMaterial;
    public Material emptyMaterial;

    void Update()
    {
        // Press the "B" key to change the Blit material
        if (Input.GetKeyDown(KeyCode.B))
        {
            //ChangeMaterial(rendererData, featureIndex, bloodMaterial);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            //ChangeMaterial(rendererData, featureIndex, dashMaterial);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            //ChangeMaterial(rendererData, featureIndex, emptyMaterial);
        }

    }

    public void ChangeMaterial(UniversalRendererData rendererData, int index, Material material)
    {
        if (rendererData == null || index < 0 || index >= rendererData.rendererFeatures.Count)
        {
            Debug.LogError("Invalid Renderer Data or Feature Index.");
            return;
        }

        // Access the specific feature at the given index
        ScriptableRendererFeature feature = rendererData.rendererFeatures[index];

        // Cast it to your custom Blit renderer feature
        if (feature is Cyan.Blit blitFeature)
        {
            // Assign the new material to the Blit renderer feature
            blitFeature.settings.blitMaterial = material;
            Debug.Log("Blit Material changed to: " + material.name);

            // Recreate the pass to apply the new material immediately
            blitFeature.Create(); // Ensure the Blit pass gets recreated with the new material
        }
        else
        {
            Debug.LogError("The feature at this index is not of type Blit.");
        }
    }
}