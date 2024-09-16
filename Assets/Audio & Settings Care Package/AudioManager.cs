using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    //Place Script on Empty Object

    // For Adding Audio Anywhere in any script use the commented scripts below

    // for Sounds:
    // AudioManager.instance.PlaySound("Name");;

    // for Music:
    // AudioManager.instance.PlayTrack("Name");;  cuts into the next track
    // AudioManager.instance.PlayNext("Name");; cross fades into the next track

    // for Ambience:
    // AudioManager.instance.PlayClip("Name");;
    // AudioManager.instance.PlayNextClip("Name");;


    public AudioMixer mixer;
    public static AudioManager instance;

    //SFX
    public SoundMaster[] sounds;

    //Music
    public MusicMaster[] tracks;
    public MusicMaster currentTrackPlaying;
    public MusicMaster nextTrackPlaying;
    public float trackFadeTime;
    private float trackFadeTimeElapsed = 0;
    private bool isTrackFading = false;

    //Ambience
    public AmbienceMaster currentClipPlaying;
    public AmbienceMaster nextClipPlaying;
    public AmbienceMaster[] clips;
    public float clipFadeTime;
    private float clipFadeTimeElapsed = 0;
    private bool isClipFading = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);

        foreach (MusicMaster t in tracks)
        {
            t.source = gameObject.AddComponent<AudioSource>();
            t.source.clip = t.track;
            t.source.volume = t.volume;
            t.source.loop = t.loop;
            t.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        }

        foreach (AmbienceMaster c in clips)
        {
            c.source = gameObject.AddComponent<AudioSource>();
            c.source.clip = c.track;
            c.source.volume = c.volume;
            c.source.loop = true;
            c.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Ambience")[0];
        }

        foreach (SoundMaster s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("SoundFX")[0];
        }

    }
    void Update()
    {
        if (isTrackFading)
        {
            FadeTrack();
            if (currentTrackPlaying != null)
            {
                currentTrackPlaying.source.volume = currentTrackPlaying.volume;
            }
            else if (currentTrackPlaying == null)
            {
                return;
            }

            if (nextTrackPlaying != null)
            {
                nextTrackPlaying.source.volume = nextTrackPlaying.volume;
            }
            else if (nextTrackPlaying == null)
            {
                return;
            }
        }

        if (!currentTrackPlaying.source.isPlaying)
        {
            currentTrackPlaying = null;
            PlayTrack("MainMenu");
        }

        if (isClipFading)
        {
            FadeClip();
            if (currentClipPlaying != null)
            {
                currentClipPlaying.source.volume = currentClipPlaying.volume;
            }
            else if (currentClipPlaying == null)
            {
                return;
            }


            if (nextClipPlaying != null)
            {
                nextClipPlaying.source.volume = nextClipPlaying.volume;
            }
            else if (nextClipPlaying == null)
            {
                return;
            }
        }
    }

    public void PlaySound(string name)
    {
        SoundMaster s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void PlayTrack(string name)
    {
        if (currentTrackPlaying != null)
        {
            currentTrackPlaying.volume = 0;
        }

        MusicMaster t = Array.Find(tracks, sound => sound.name == name);
        currentTrackPlaying = t;

        currentTrackPlaying.volume = 1;
        t.source.Play();
    }

    public void PlayClip(string name)
    {
        if (currentClipPlaying != null)
        {
            currentClipPlaying.volume = 0;
        }

        AmbienceMaster c = Array.Find(clips, sound => sound.name == name);
        currentClipPlaying = c;

        currentClipPlaying.volume = 1;
        c.source.Play();
    }



    public void PlayNextClip(string name)
    {
        AmbienceMaster c = Array.Find(clips, sound => sound.name == name);
        nextClipPlaying = c;

        clipFadeTimeElapsed = 0;
        isClipFading = true;

        nextClipPlaying.source.Play();
    }

    public void PlayNext(string name)
    {
        MusicMaster t = Array.Find(tracks, sound => sound.name == name);
        if (currentTrackPlaying != t)
        {
            nextTrackPlaying = t;

            trackFadeTimeElapsed = 0;
            isTrackFading = true;

            nextTrackPlaying.source.Play();
        }
    }

    private void FadeTrack()
    {
        if (trackFadeTimeElapsed < trackFadeTime)
        {
            currentTrackPlaying.volume = Mathf.Lerp(1, 0, trackFadeTimeElapsed / trackFadeTime);
            nextTrackPlaying.volume = Mathf.Lerp(0, 1, trackFadeTimeElapsed / trackFadeTime);

            trackFadeTimeElapsed += Time.deltaTime;
        }
        else
        {
            currentTrackPlaying.volume = 0;
            nextTrackPlaying.volume = 1;

            currentTrackPlaying = nextTrackPlaying;
            nextTrackPlaying = null;

            isTrackFading = false;
        }
    }

    private void FadeClip()
    {
        if (clipFadeTimeElapsed < clipFadeTime)
        {
            currentClipPlaying.volume = Mathf.Lerp(1, 0, clipFadeTimeElapsed / clipFadeTime);
            nextClipPlaying.volume = Mathf.Lerp(0, 1, clipFadeTimeElapsed / clipFadeTime);

            clipFadeTimeElapsed += Time.deltaTime;
        }
        else
        {
            currentClipPlaying.volume = 0;
            nextClipPlaying.volume = 1;

            //currentClipPlaying.source.Stop();

            currentClipPlaying = nextClipPlaying;
            nextClipPlaying = null;

            isClipFading = false;
        }
    }



}



