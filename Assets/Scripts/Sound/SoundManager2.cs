using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class SoundManager2 : MonoBehaviour
{
    public static SoundManager2 Instance;

    [Range(0f, 1f)] public float masterVolume = 1;

    [Range(0f, 1f)] public float masterMusicVolume = 1;

    [Range(0f, 1f)] public float masterSfxVolume = 1;



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

    private MusicMaster currentMusic;
    private MusicMaster nextMusic;
    private bool isMusicFading;
    private float fadeDuration = 2f;
    private float fadeTimer;

    private void Awake()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterValue",1);
        masterMusicVolume = PlayerPrefs.GetFloat("MasterMusicValue", 1);
        masterSfxVolume = PlayerPrefs.GetFloat("MasterSfxValue", 1);

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (SceneManager.GetActiveScene().buildIndex == 0) PlayMusic("Main Menu");
        else if (SceneManager.GetActiveScene().buildIndex == 2) PlayMusic("Tutorial");
        else PlayMusic("Gameplay Track 1");
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

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Movement.playerMovement.GetComponent<PlayerHealth>().health <= 0) return;

            if (currentMusic.source.time >= currentMusic.source.clip.length)
            {
                if (currentMusic.name == "Gameplay Track 1")
                {
                    PlayMusic("Gameplay Track 2");
                }
                else if (currentMusic.name == "Gameplay Track 2")
                {
                    PlayMusic("Gameplay Track 3");
                }

                else
                {
                    PlayMusic("Gameplay Track 1");
                }
            }
        }

        //enSured deletion 
        foreach (AudioSource aus in gameObject.GetComponentsInChildren<AudioSource>())
        {
            if (aus.GetComponent<AudioSource>())
            {
                if (aus.GetComponent<AudioSource>().time >= aus.GetComponent<AudioSource>().clip.length)
                {
                    Destroy(GetComponent<AudioSource>());
                }
            }
        }

        SourceVolumeUpdate();

    }

    public void PlaySound(string name, Transform targetObject = null, bool Overlap = true, bool shouldLoop = false)
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
            AudioSource newSource = (targetObject == null) ? gameObject.AddComponent<AudioSource>() : targetObject.GetComponent<AudioSource>();

            if (newSource.gameObject == gameObject && newSource.isPlaying)
                newSource = newSource.gameObject.AddComponent<AudioSource>();

            newSource.clip = clipToPlay;
            newSource.volume = sound.volume * masterSfxVolume * masterVolume;
            newSource.pitch = sound.pitch;
            newSource.loop = sound.loop;


            //Debug.Log("Source volume is :" + newSource.volume);
            // Set 3D sound properties if Local3D
            newSource.spatialBlend = (sound.soundType == SoundType.Local3D) ? 1.0f : 0.0f;

            sound.source = newSource;
            sound.source.Play();
            //Debug.Log($"Now adding sound:\"{sound.source.clip.name}\" to \" {sound.source.name}\"");
            // Clean up after the clip has finished playing

            if (targetObject == null)
                Destroy(sound.source, clipToPlay.length);
        }
    }

    public void SourceVolumeUpdate()
    {
        foreach (MusicMaster music in musicTracks)
        {
            if (music.source)
            {
                music.source.volume = music.volume * masterMusicVolume * masterVolume;
            }
        }

        foreach (SoundMaster sound in sounds)
        {
            if (sound.source)
            {
                sound.source.volume = sound.volume * masterSfxVolume * masterVolume;
            }
        }
    }



    public void StopSound(string soundName, Transform targetObject = null)
    {
        SoundMaster sound = sounds.Find(s => s.name == soundName);

        if (sound != null && targetObject == null && sound.source)
        {
            sound.source.Stop();
            Destroy(sound.source);
        }
        else if (sound != null && targetObject != null)
        {
            AudioSource localAudioSource = targetObject.GetComponent<AudioSource>();
            if (localAudioSource != null)
            {
                localAudioSource.Stop();
            }
        }

    }

    public void StopMusic(string musicName, Transform targetObject = null)
    {
        MusicMaster music = musicTracks.Find(s => s.name == musicName);

        if (music != null && targetObject == null && music.source)
        {
            music.source.Stop();
            Destroy(music.source);
        }
        else if (music != null && targetObject != null)
        {
            AudioSource localAudioSource = targetObject.GetComponent<AudioSource>();
            if (localAudioSource != null)
            {
                localAudioSource.Stop();
            }
        }

    }

    public void PlayMusic(string name, bool fade = false, bool shouldLoop = false)
    {

        MusicMaster music = musicTracks.Find(m => m.name == name);
        if (music == null) return;
        else
        {
            if (currentMusic != null)
                if (music.track == currentMusic.track) return;

            music.source = gameObject.AddComponent<AudioSource>();
            music.source.clip = music.track;
            music.source.volume = music.volume * masterMusicVolume * masterVolume;
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
            currentMusic.source.volume = music.volume * masterMusicVolume * masterVolume;
            currentMusic.source.Play();
        }

    }

    public void PlayAmbience(string name, Transform targetObject = null)
    {
        AmbienceMaster ambience = ambienceClips.Find(s => s.name == name);
        if (ambience != null && ambience.tracks.Length > 0)
        {
            // Select a random clip from the array of clips
            AudioClip trackToPlay = ambience.tracks[UnityEngine.Random.Range(0, ambience.tracks.Length)];

            // Create a new AudioSource for this specific instance
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = trackToPlay;
            newSource.volume = ambience.volume;
            newSource.pitch = ambience.pitch;

            // Set 3D sound properties if Local3D
            newSource.spatialBlend = (ambience.soundType == SoundType.Local3D) ? 1.0f : 0.0f;

            ambience.source = newSource;
            ambience.source.Play();

            // Clean up after the clip has finished playing
            Destroy(ambience.source, trackToPlay.length);
        }
    }

    public void AdjustMasterVolume(float volume)
    {
        masterVolume = volume;
    }

    public void AdjustMusicVolume(float volume)
    {
        masterMusicVolume = volume;
    }


    public void AdjustSFXVolume(float volume)
    {
        masterSfxVolume = volume;
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