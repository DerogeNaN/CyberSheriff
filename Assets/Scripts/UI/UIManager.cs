using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public GameManager gm;
    public UniversalRendererData rendererData;
    public int HealthEffectBlitIndex;
    public int DashEffectBlitIndex;

    enum PlayerHealthState
    {
        High,
        Med,
        Low
    }

    [SerializeField]
    PlayerHealthState playerHealthstate;

    private void Start()
    {
        playerHealthstate = PlayerHealthState.High;
        gm = GetComponent<GameManager>();
    }

    void Update()
    {
        if (gm.playerHealth.health > 50)
            playerHealthstate = PlayerHealthState.High;
        else if (gm.playerHealth.health < 50 && gm.playerHealth.health > 30)
            playerHealthstate = PlayerHealthState.Med;
        else if (gm.playerHealth.health < 30)
            playerHealthstate = PlayerHealthState.Low;

        // Press the "B" key to change the Blit material
        if (playerHealthstate == PlayerHealthState.Low)
        {
            ActivateBlitz(rendererData, HealthEffectBlitIndex, true);
        }
        else 
        {
            ActivateBlitz(rendererData, HealthEffectBlitIndex, false);

        }

        if (gm.playerMovement.isDashing)
        {
            ActivateBlitz(rendererData, DashEffectBlitIndex, true);
        }
        else
        {
            ActivateBlitz(rendererData, DashEffectBlitIndex, false);
        }
    }


    public void ActivateBlitz(UniversalRendererData rendererData, int index, bool active)
    {
        if (rendererData == null || index < 0 || index >= rendererData.rendererFeatures.Count)
        {
            Debug.LogError("Invalid Renderer Data or Feature Index.");
            return;
        }

        ScriptableRendererFeature feature = rendererData.rendererFeatures[index];
        // Cast it to your custom Blit renderer feature
        if (feature is Cyan.Blit blitFeature)
        {
            blitFeature.SetActive(active);
        }
        else
        {
            Debug.LogError("The feature at this index is not of type Blit.");
        }
    }

    private void OnApplicationQuit()
    {
        if (rendererData == null || HealthEffectBlitIndex < 0 || HealthEffectBlitIndex >= rendererData.rendererFeatures.Count)
        {
            Debug.LogError("Invalid Renderer Data or Feature Index.");
            return;
        }
        ScriptableRendererFeature feature = rendererData.rendererFeatures[HealthEffectBlitIndex];
        // Cast it to your custom Blit renderer feature
        if (feature is Cyan.Blit blitFeature)
        {
            blitFeature.SetActive(false);
        }
        else
        {
            Debug.LogError("The feature at this index is not of type Blit.");
        }


        if (rendererData == null || HealthEffectBlitIndex < 0 || HealthEffectBlitIndex >= rendererData.rendererFeatures.Count)
        {
            Debug.LogError("Invalid Renderer Data or Feature Index.");
            return;
        }
        ScriptableRendererFeature feature1 = rendererData.rendererFeatures[HealthEffectBlitIndex];
        // Cast it to your custom Blit renderer feature
        if (feature1 is Cyan.Blit blitFeature1)
        {
            blitFeature1.SetActive(false);
        }
        else
        {
            Debug.LogError("The feature at this index is not of type Blit.");
        }
    }

}
