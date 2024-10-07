using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreShake : MonoBehaviour
{
    public float pulsateDuration = 1f; // Duration of one pulsate cycle
    public float pulsateScale = -1.4f;  // Scale factor for pulsating

    public float shakeDuration = 0.5f; // Duration of the shake
    public float shakeMagnitude = 0.1f; // Magnitude of the shake

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool isPulsating = false;

    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    public void StartScorePulsateAndShake()
    {
        if (!isPulsating)
        {
            StartCoroutine(ScorePulsateAndShake());
        }
    }

    private IEnumerator ScorePulsateAndShake()
    {
        isPulsating = true;
        float pulsateTimer = 0f;
        float shakeTimer = 0f;

        while (pulsateTimer < pulsateDuration || shakeTimer < shakeDuration)
        {
            if (pulsateTimer < pulsateDuration)
            {
                float scale = Mathf.Lerp(1f, pulsateScale, Mathf.PingPong(pulsateTimer * 2 / pulsateDuration, 1));
                transform.localScale = originalScale * scale;
                pulsateTimer += Time.deltaTime;
            }

            if (shakeTimer < shakeDuration)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                transform.localPosition = originalPosition + shakeOffset;
                shakeTimer += Time.deltaTime;
            }
            else
            {
                transform.localPosition = originalPosition;
            }

            yield return null;
        }

        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
        isPulsating = false;
    }
}
