using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject UIObj;
    public GameObject mainMenu;

    [Header("Required scripts")]
    public PauseMenu pauseMenuScript;
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
        exit,
        Screen,
        sound,
        controls,
    }

    // Start is called before the first frame update
    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterValue");
        musicSlider.value = PlayerPrefs.GetFloat("MasterMusicValue");
        SFXSlider.value = PlayerPrefs.GetFloat("MasterSfxValue");

        masterText.text = (Mathf.Floor(PlayerPrefs.GetFloat("MasterValue") * 100)).ToString();
        musicText.text = (Mathf.Floor(PlayerPrefs.GetFloat("MasterMusicValue") * 100)).ToString();
        SFXText.text = (Mathf.Floor(PlayerPrefs.GetFloat("MasterSfxValue") * 100)).ToString();

        FOVSlider.value = PlayerPrefs.GetFloat("FOV");
        FOVText.text = PlayerPrefs.GetFloat("FOV").ToString("F2");

        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        sensitivityText.text = math.remap(0, 20, 0, 2, PlayerPrefs.GetFloat("Sensitivity")).ToString("F2");

        controlsButton.onClick.AddListener(delegate { SetState(OptionsMenuState.controls); });
        soundButton.onClick.AddListener(delegate { SetState(OptionsMenuState.sound); });
        returnToMenu.onClick.AddListener(delegate { SetState(OptionsMenuState.exit); });
        screenButton.onClick.AddListener(delegate { SetState(OptionsMenuState.Screen); });

    }

    // Update is called once per frame
    void Update() { UpdateState(menuState); }

    public void SetState(OptionsMenuState state)
    {
        if (menuState == OptionsMenuState.main && state == OptionsMenuState.main)
        {
            if (pauseMenuScript)
                pauseMenuScript.ReturnToMenuButton();
        }

        if (state is OptionsMenuState.exit)
            UpdateState(state);
        else
            this.menuState = state;
    }

    public void UpdateState(OptionsMenuState state)
    {
        switch (state)
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

            case OptionsMenuState.exit:
                ActivateMenu(4);
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
                UIObj.SetActive(true);
                if (mainMenu)
                    mainMenu.SetActive(false);

                screenMenu.SetActive(false);
                soundMenu.SetActive(false);
                controlsMenu.SetActive(false);

                returnToMenu.onClick.RemoveAllListeners();

                returnToMenu.onClick.AddListener(pauseMenuScript != null ? delegate { SetState(OptionsMenuState.main); } : delegate { SetState(OptionsMenuState.exit); });
                break;

            case 1://screen
                optionsMainMenu.SetActive(true);
                optionsPrefab.SetActive(true);
                UIObj.SetActive(false);
                if (mainMenu)
                    mainMenu.SetActive(false);



                screenMenu.SetActive(true);
                soundMenu.SetActive(false);
                controlsMenu.SetActive(false);

                returnToMenu.onClick.RemoveAllListeners();
                returnToMenu.onClick.AddListener(delegate { SetState(OptionsMenuState.main); });

                break;


            case 2://sound
                optionsMainMenu.SetActive(true);
                optionsPrefab.SetActive(true);
                UIObj.SetActive(false);
                if (mainMenu)
                    mainMenu.SetActive(false);


                screenMenu.SetActive(false);
                soundMenu.SetActive(true);
                controlsMenu.SetActive(false);


                returnToMenu.onClick.RemoveAllListeners();
                returnToMenu.onClick.AddListener(delegate { SetState(OptionsMenuState.main); });
                break;


            case 3://controls
                optionsMainMenu.SetActive(true);
                optionsPrefab.SetActive(true);
                UIObj.SetActive(false);
                if (mainMenu)
                    mainMenu.SetActive(false);

                screenMenu.SetActive(false);
                soundMenu.SetActive(false);
                controlsMenu.SetActive(true);

                returnToMenu.onClick.RemoveAllListeners();
                returnToMenu.onClick.AddListener(delegate { SetState(OptionsMenuState.main); });

                break;


            case 4://exit
                optionsMainMenu.SetActive(false);
                optionsPrefab.SetActive(false);
                UIObj.SetActive(false);
                if (mainMenu)
                    mainMenu.SetActive(true);

                screenMenu.SetActive(false);
                soundMenu.SetActive(false);
                controlsMenu.SetActive(false);
                break;
        }

    }

    public void MasterVolumeSlider()
    {
        if (SoundManager2.Instance)
            SoundManager2.Instance.AdjustMasterVolume(masterSlider.value);
        PlayerPrefs.SetFloat("MasterValue", SoundManager2.Instance.masterVolume);
        masterText.text = (Mathf.Floor(SoundManager2.Instance.masterVolume * 100)).ToString();
    }

    public void MusicVolumeSlider()
    {
        if (SoundManager2.Instance)
            SoundManager2.Instance.AdjustMusicVolume(musicSlider.value);
        PlayerPrefs.SetFloat("MasterMusicValue", SoundManager2.Instance.masterMusicVolume);
        musicText.text = (Mathf.Floor(SoundManager2.Instance.masterMusicVolume * 100)).ToString();
    }

    public void SFXVolumeSlider()
    {
        if (SoundManager2.Instance)
            SoundManager2.Instance.AdjustSFXVolume(SFXSlider.value);
        PlayerPrefs.SetFloat("MasterSfxValue", SoundManager2.Instance.masterSfxVolume);
        SFXText.text = (Mathf.Floor(SoundManager2.Instance.masterSfxVolume * 100)).ToString();
    }

    public void SenseSlider()
    {
        PlayerPrefs.SetFloat("Sensitivity", math.remap(0, 1, 0, 20, sensitivitySlider.value));
        if (lookingscript)
            lookingscript.mouseSens = PlayerPrefs.GetFloat("Sensitivity");
        sensitivityText.text = math.remap(0, 20, 0, 2, PlayerPrefs.GetFloat("Sensitivity")).ToString("F2");
    }

    public void FOVSliderFunc()
    {
        PlayerPrefs.SetFloat("FOV", math.remap(0, 1, 60, 110, FOVSlider.value));
        if (jiggleScript)
            jiggleScript.SetDefaultFov(PlayerPrefs.GetFloat("FOV"));
        FOVText.text = PlayerPrefs.GetFloat("FOV").ToString("F2");
    }
}
