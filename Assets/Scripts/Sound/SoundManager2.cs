using UnityEngine;
using System.Collections.Generic;
using System;

public class SoundManager2 : MonoBehaviour
{
    public static SoundManager2 Instance;

    public enum SoundCategory { Master, SFX, Music, UI }
    public enum SoundType { Global2D, Local3D }

    [System.Serializable]
    public class SoundMaster
    {
        public string name;
        public SoundType soundType; // Added SoundType
        public AudioClip[] clips; // Changed back to array for consistency
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop; // Added looping option
        [HideInInspector] public AudioSource source;
    }

    [System.Serializable]
    public class MusicMaster
    {
        public string name;
        public AudioClip track;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop; // Ensure looping option for music
        [HideInInspector] public AudioSource source;
    }

    [System.Serializable]
    public class AmbienceMaster
    {
        public string name;
        public SoundType soundType; // Added SoundType
        public AudioClip[] tracks;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop; // Ensure looping option for ambience
        [HideInInspector] public AudioSource source;
    }

    public List<SoundMaster> sounds = new List<SoundMaster>();
    public List<MusicMaster> musicTracks = new List<MusicMaster>();
    public List<AmbienceMaster> ambienceClips = new List<AmbienceMaster>();
    private Dictionary<string, AudioSource> globalSounds = new Dictionary<string, AudioSource>(); // Added global sounds dictionary

    private MusicMaster currentMusic;
    private MusicMaster nextMusic;
    private bool isMusicFading;
    private float fadeDuration = 2f;
    private float fadeTimer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        // Initialize sources for sounds
        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            if (s.soundType == SoundType.Global2D)
            {
                globalSounds[s.name] = s.source; // Track global sounds
            }
        }

        // Initialize sources for music
        foreach (var m in musicTracks)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.track;
            m.source.volume = m.volume;
            m.source.loop = m.loop; // Preserve looping option
        }

        // Initialize sources for ambience
        foreach (var a in ambienceClips)
        {
            a.source = gameObject.AddComponent<AudioSource>();
            a.source.volume = a.volume;
            a.source.pitch = a.pitch;
            if (a.soundType == SoundType.Global2D)
            {
                globalSounds[a.name] = a.source; // Track global sounds
            }
        }
    }

    private void Update()
    {
        if (isMusicFading)
        {
            Debug.Log($"Fading from {currentMusic.name} to {nextMusic?.name}");

            FadeMusic();
            if (currentMusic != null)
            {
                currentMusic.source.volume = currentMusic.volume;
            }
            else if (currentMusic == null)
            {
                return;
            }

            if (nextMusic != null)
            {
                nextMusic.source.volume = nextMusic.volume;
            }
            else if (nextMusic == null)
            {
                return;
            }
        }
    }

    public void PlaySound(string name, Transform targetObject = null)
    {
        SoundMaster sound = sounds.Find(s => s.name == name);
        if (sound != null && sound.clips.Length > 0)
        {
            // Select a random clip from the array of clips
            AudioClip clipToPlay = sound.clips[UnityEngine.Random.Range(0, sound.clips.Length)];

            // Create a new AudioSource for this specific instance
            AudioSource tempSource = gameObject.AddComponent<AudioSource>();
            tempSource.clip = clipToPlay;
            tempSource.volume = sound.volume;
            tempSource.pitch = sound.pitch;

            // Set 3D sound properties if Local3D
            tempSource.spatialBlend = (sound.soundType == SoundType.Local3D) ? 1.0f : 0.0f;

            tempSource.Play();

            // Clean up after the clip has finished playing
            Destroy(tempSource, clipToPlay.length);
        }
    }

  /*  public void StopSound(string name)
    {
        if (globalSounds.ContainsKey(name))
        {
            globalSounds[name].Stop(); // Stop global sound
        }
    }*/
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

    public void PlayMusic(string name, bool fade = false)
    {
        MusicMaster music = musicTracks.Find(m => m.name == name);
        if (music == null) return;

        if (fade && currentMusic != null)
        {
            nextMusic = music;
            fadeTimer = 0;
            isMusicFading = true;
        }
        else
        {
            if (currentMusic != null) currentMusic.source.Stop();
            currentMusic = music;
            currentMusic.source.volume = currentMusic.volume;
            currentMusic.source.Play();
        }
    }

    public void PlayAmbience(string name,Transform targetObject = null)
    {
       /* AmbienceMaster ambience = ambienceClips.Find(a => a.name == name);
        if (ambience != null) ambience.source.Play();*/

        AmbienceMaster ambience = ambienceClips.Find(s => s.name == name);
        if (ambience != null && ambience.tracks.Length > 0)
        {
            // Select a random clip from the array of clips
            AudioClip trackToPlay = ambience.tracks[UnityEngine.Random.Range(0, ambience.tracks.Length)];

            // Create a new AudioSource for this specific instance
            AudioSource tempSource = gameObject.AddComponent<AudioSource>();
            tempSource.clip = trackToPlay;
            tempSource.volume = ambience.volume;
            tempSource.pitch = ambience.pitch;

            // Set 3D sound properties if Local3D
            tempSource.spatialBlend = (ambience.soundType == SoundType.Local3D) ? 1.0f : 0.0f;

            tempSource.Play();

            // Clean up after the clip has finished playing
            Destroy(tempSource, trackToPlay.length);
        }
    }

    private void FadeMusic()
    {
        if (fadeTimer < fadeDuration)
        {
            float fadeProgress = fadeTimer / fadeDuration;
            currentMusic.source.volume = Mathf.Lerp(currentMusic.volume, 0, fadeProgress);
            nextMusic.source.volume = Mathf.Lerp(0, nextMusic.volume, fadeProgress);
            fadeTimer += Time.deltaTime;
        }
        else
        {
            currentMusic.source.Stop();
            currentMusic = nextMusic;
            nextMusic = null;
            currentMusic?.source.Play(); // Play the new current track
            isMusicFading = false;
        }
    }
}