using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Myles_SettingsManager : MonoBehaviour
{ 

    /*
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundFXSlider;
    public Slider ambienceSlider;

    public Dropdown resolutionDropdown;
    Resolution[] resolutions;

    const string mixer_Master = "Master";
    const string mixer_Music = "Music";
    const string mixer_SoundFX = "SoundFX";
    const string mixer_Ambience = "Ambience";


    // Sound Sliders

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        soundFXSlider.onValueChanged.AddListener(SetSoundFXVolume);
        ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);

    }

    public void SetMasterVolume (float value)
    {
        audioMixer.SetFloat("mixer_Master", Mathf.Log10(value) * 20);
    }
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("mixer_Music", Mathf.Log10(value) * 20); 
    }
    public void SetSoundFXVolume(float value)
    {
        audioMixer.SetFloat("mixer_SoundFX", Mathf.Log10(value) * 20);
    }
    public void SetAmbienceVolume(float value)
    {
        audioMixer.SetFloat("mixer_Ambience", Mathf.Log10(value) * 20);
    }

    // Fullscreen and Resolution

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void ToggleFullscreen(bool isWindowed)
    {
        Screen.fullScreen = !isWindowed;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    */
}
    