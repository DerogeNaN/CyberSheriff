using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUi : MonoBehaviour
{
    
    public Slider SFXSlider;
    public Slider musicSlider;




    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = SoundManager2.Instance.masterMusicVolume;
        SFXSlider.value = SoundManager2.Instance.masterSfxVolume;
    }



    // Update is called once per frame
    void Update()
    {
        
    }

  

    public void MusicVolumeSlider ()
    {
        SoundManager2.Instance.AdjustMusicVolume(musicSlider.value);
    }


    public void SFXVolumeSlider()
    {
        SoundManager2.Instance.AdjustSFXVolume(SFXSlider.value);
    }
}
