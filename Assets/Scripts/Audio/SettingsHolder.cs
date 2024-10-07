using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHolder : MonoBehaviour
{
    //public CameraController camera;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundFXSlider;
    public Slider ambienceSlider;
    public Slider brightnessSlider;

    public Toggle fullscreenToggle;
    public Toggle invertCameraToggle;
    public Toggle tutorialToggle;

    public TMP_Dropdown resolutionDropdown;
}
