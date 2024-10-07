using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AmbienceMaster
{
    public string name;
    public AudioClip track;

    [Range(0f, 1f)]
    public float volume;

    [HideInInspector]
    public AudioSource source;
}

