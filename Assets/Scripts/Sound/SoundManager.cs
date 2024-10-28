using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public enum SoundCategory { Master, SFX, Music, UI }
    public enum SoundType { Global2D, Local3D }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip[] clips;
        public float volume = 1f;
        public bool loop = false;
        public SoundCategory category;
        public SoundType soundType = SoundType.Global2D; // Determines if sound is 2D or 3D
    }

    public List<Sound> sounds = new List<Sound>();
    private Dictionary<string, AudioSource> globalSounds = new Dictionary<string, AudioSource>();

    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float musicVolume = 1f;
    private float uiVolume = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        // Initialize global (2D) sounds
        foreach (var sound in sounds)
        {
            if (sound.soundType == SoundType.Global2D)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.volume = sound.volume;
                audioSource.loop = sound.loop;
                audioSource.spatialBlend = 0f; // 2D sound
                globalSounds[sound.name] = audioSource;
            }
        }
    }

    public void PlaySound(string soundName, Transform targetObject = null)
    {
        var sound = sounds.Find(s => s.name == soundName);
        if (sound == null) return;

        AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];

        if (sound.soundType == SoundType.Global2D)
        {
            if (globalSounds.ContainsKey(soundName))
            {
                globalSounds[soundName].clip = clip;
                globalSounds[soundName].volume = sound.volume * GetCategoryVolume(sound.category);
                globalSounds[soundName].Play();
            }
        }
        else if (sound.soundType == SoundType.Local3D && targetObject != null)
        {
            // Create or find a 3D audio source on the target object
            AudioSource localAudioSource = targetObject.GetComponent<AudioSource>();
            if (localAudioSource == null)
            {
                localAudioSource = targetObject.gameObject.AddComponent<AudioSource>();
                localAudioSource.spatialBlend = 1f; // Fully 3D sound
            }

            localAudioSource.clip = clip;
            localAudioSource.volume = sound.volume * GetCategoryVolume(sound.category);
            localAudioSource.loop = sound.loop;
            localAudioSource.Play();
        }
    }

    public void StopSound(string soundName, Transform targetObject = null)
    {
        if (globalSounds.ContainsKey(soundName) && targetObject == null)
        {
            globalSounds[soundName].Stop();
        }
        else if (targetObject != null)
        {
            AudioSource localAudioSource = targetObject.GetComponent<AudioSource>();
            if (localAudioSource != null)
            {
                localAudioSource.Stop();
            }
        }
    }

    public void SetVolume(SoundCategory category, float volume)
    {
        // Set category volumes and apply them to currently playing sounds
        switch (category)
        {
            case SoundCategory.Master: masterVolume = volume; break;
            case SoundCategory.SFX: sfxVolume = volume; break;
            case SoundCategory.Music: musicVolume = volume; break;
            case SoundCategory.UI: uiVolume = volume; break;
        }

        foreach (var sound in sounds)
        {
            if (globalSounds.ContainsKey(sound.name) && globalSounds[sound.name].isPlaying)
            {
                globalSounds[sound.name].volume = sound.volume * GetCategoryVolume(sound.category);
            }
        }
    }

    private float GetCategoryVolume(SoundCategory category)
    {
        return category switch
        {
            SoundCategory.Master => masterVolume,
            SoundCategory.SFX => sfxVolume * masterVolume,
            SoundCategory.Music => musicVolume * masterVolume,
            SoundCategory.UI => uiVolume * masterVolume,
            _ => 1f
        };
    }
}