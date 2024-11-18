using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.SceneManagement;
using UnityEngine.Animations;

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
        [Range(0f, 2f)] public float volume = 1f;
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
        // Initialize sources for music
        //foreach (var m in musicTracks)
        //{
        //    m.source = gameObject.AddComponent<AudioSource>();
        //    m.source.clip = m.track;
        //    m.source.volume = m.volume;
        //    m.source.loop = m.loop; // Preserve looping option
        //}

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

        foreach (AudioSource aus in gameObject.GetComponentsInChildren<AudioSource>())
        {
            if (aus.GetComponent<AudioSource>())
            {
                if (!aus.GetComponent<AudioSource>().isPlaying)
                {
                    Destroy(GetComponent<AudioSource>());
                }
            }
        }
    }

    public void PlaySound(string name, Transform targetObject = null, bool Overlap = true)
    {
        SoundMaster sound = sounds.Find(s => s.name == name);
        if (sound != null && sound.clips.Length > 0)
        {
            // Select a random clip from the array of clips
            AudioClip clipToPlay = sound.clips[UnityEngine.Random.Range(0, sound.clips.Length)];

            foreach (AudioSource audioSource in GetComponentsInChildren<AudioSource>())
            {
                if (audioSource.clip == clipToPlay)
                {
                    if (!Overlap)
                    {
                        Debug.Log($"attempt to add sound duplicate : \"{audioSource.clip.name} \" to \"{((targetObject != null) ? targetObject.name : gameObject.name)} \"  has been cancelled");
                        return;
                    }

                }

            }

            // Create a new AudioSource for this specific instance
            AudioSource tempSource = (targetObject == null) ? gameObject.AddComponent<AudioSource>() : targetObject.GetComponent<AudioSource>();

            if (tempSource.gameObject == gameObject && tempSource.isPlaying)
                tempSource = tempSource.gameObject.AddComponent<AudioSource>();

            tempSource.clip = clipToPlay;
            tempSource.volume = sound.volume;
            tempSource.pitch = sound.pitch;

            // Set 3D sound properties if Local3D
            tempSource.spatialBlend = (sound.soundType == SoundType.Local3D) ? 1.0f : 0.0f;

            tempSource.Play();
            Debug.Log($"Now adding sound:\"{tempSource.clip.name}\" to \" {tempSource.name}\"");
            // Clean up after the clip has finished playing

            if (targetObject == null)
                Destroy(tempSource, clipToPlay.length);
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

    public void PlayMusic(string name, bool fade = false)
    {

        MusicMaster music = musicTracks.Find(m => m.name == name);
        if (music == null) return;
        else
        {
            if (currentMusic != null)
                if (music.track == currentMusic.track) return;

            music.source = gameObject.AddComponent<AudioSource>();
            music.source.clip = music.track;
            music.source.volume = music.volume;
            music.source.loop = music.loop; // Preserve looping option
        }

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

    public void PlayAmbience(string name, Transform targetObject = null)
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