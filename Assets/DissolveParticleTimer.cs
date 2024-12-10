using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveParticleTimer : MonoBehaviour
{
    public float timer = 1.0f;
    public ParticleSystem particles;
    bool played = false;

    void Update()
    {
        timer -= Time.deltaTime;

        if (!played && !particles.isPlaying && timer <= 0)
        {
            played = true;
            particles.Play();
        }
    }
}
