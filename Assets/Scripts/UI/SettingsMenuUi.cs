using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;
using Unity.Mathematics;

public class SettingsMenuUi : MonoBehaviour
{
    public OptionsMenuState menuState = OptionsMenuState.main;

    [Header("Menu Objects (Make sure these are applied)")]
    public GameObject optionsMainMenu;
    public GameObject screenMenu;
    public GameObject soundMenu;
    public GameObject controlsMenu;
    public GameObject optionsPrefab;

    [Header("Required scripts")]
    public PauseMenu PauseMenuScript;
    public MouseLook lookingscript;
    public CameraJiggle jiggleScript;

    [Header("Main Menu UI Elements(Needs to be set)")]

    public Button soundButton;
    public Button screenButton;
    public Button controlsButton;
    public Button returnToMenu;
    [Header("Sound Menu UI Elements(Needs To Be Set)")]

    public Slider masterSlider;
    public Slider SFXSlider;
    public Slider musicSlider;
    public TMP_Text masterText;
    public TMP_Text musicText;
    public TMP_Text SFXText;

    [Header("Screen Menu UI  Elements(Needs To Be Set)")]

    public Slider FOVSlider;
    public TMP_Text FOVText;
    public TMP_Dropdown ScreenModeDropDown;

    [Header("Controls Ui Elements(Needs To Be Set)")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityText;

    public enum OptionsMenuState
    {
        main,
        Screen,
        sound,
        controls,
    }

    // Start is called before the first frame update
    void Start()
    {
        masterSlider.value = SoundManager2.Instance.masterVolume;
        musicSlider.value = SoundManager2.Instance.masterMusicVolume;
        SFXSlider.value = SoundManager2.Instance.masterSfxVolume;
        masterText.text = (Mathf.Floor(SoundManager2.Instance.masterVolume * 100)).ToString();
        musicText.text = (Mathf.Floor(SoundManager2.Instance.masterMusicVolume * 100)).ToString();
        SFXText.text = (Mathf.Floor(SoundManager2.Instance.masterSfxVolume * 100)).ToString();
        FOVSlider.value = jiggleScript.GetDefaultFov();
        FOVText.text = jiggleScript.GetDefaultFov().ToString("F2");

        sensitivitySlider.value = lookingscript.GetMouseSense();
        sensitivityText.text = math.remap(0, 20, 0, 2, lookingscript.GetMouseSense()).ToString("F2");
        controlsButton.onClick.AddListener(delegate { SetState(OptionsMenuState.controls); });
        soundButton.onClick.AddListener(delegate { SetState(OptionsMenuState.sound); });
        returnToMenu.onClick.AddListener(delegate { SetState(OptionsMenuState.main); });
        screenButton.onClick.AddListener(delegate { SetState(OptionsMenuState.Screen); });

    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    public void SetState(OptionsMenuState state)
    {
        if (menuState == OptionsMenuState.main && state == OptionsMenuState.main)
        {
            PauseMenuScript.ReturnToMenuButton();
        }
        this.menuState = state;
    }

    public void UpdateState()
    {
        switch (menuState)
        {
            case OptionsMenuState.main:
                ActivateMenu(0);

                break;

            case OptionsMenuState.Screen:
                ActivateMenu(1);

                break;

            case OptionsMenuState.sound:
                ActivateMenu(2);

                break;

            case OptionsMenuState.controls:
                ActivateMenu(3);
                
                break;
        }
    }


    public void ActivateMenu(int i)
    {
        switch (i)
        {
            case 0://main 
                optionsMainMenu.SetActive(true);
                optionsPrefab.SetActive(false);

                screenMenu.SetActive(false);
                soundMenu.SetActive(false);
                controlsMenu.SetActive(false);
                break;

            case 1://screen
                optionsMainMenu.SetActive(false);
                optionsPrefab.SetActive(true);

                screenMenu.SetActive(true);
                soundMenu.SetActive(false);
                controlsMenu.SetActive(false);
                break;


            case 2://sound
                optionsMainMenu.SetActive(false);
                optionsPrefab.SetActive(true);

                screenMenu.SetActive(false);
                soundMenu.SetActive(true);
                controlsMenu.SetActive(false);
                break;


            case 3://controls
                optionsMainMenu.SetActive(false);
                optionsPrefab.SetActive(true);

                screenMenu.SetActive(false);
                soundMenu.SetActive(false);
                controlsMenu.SetActive(true);
                break;
        }

    }

    public void MasterVolumeSlider()
    {
        SoundManager2.Instance.AdjustMasterVolume(masterSlider.value);
        masterText.text = (Mathf.Floor(SoundManager2.Instance.masterVolume * 100)).ToString();
    }

    public void MusicVolumeSlider()
    {
        SoundManager2.Instance.AdjustMusicVolume(musicSlider.value);
        musicText.text = (Mathf.Floor(SoundManager2.Instance.masterMusicVolume * 100)).ToString();
    }

    public void SFXVolumeSlider()
    {
        SoundManager2.Instance.AdjustSFXVolume(SFXSlider.value);
        SFXText.text = (Mathf.Floor(SoundManager2.Instance.masterSfxVolume * 100)).ToString();
    }

    public void SenseSlider()
    {
        lookingscript.SetMouseSense(sensitivitySlider.value * 20);
        sensitivityText.text = math.remap(0, 20, 0, 2, lookingscript.GetMouseSense()).ToString("F2");
    }

    public void FOVSliderFunc() 
    {
        jiggleScript.SetDefaultFov(math.clamp(60,110, FOVSlider.value * 110));
        FOVText.text = jiggleScript.GetDefaultFov().ToString("F2");
    }
}
