using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Defaults")]
    public float defaultDuration = 0.25f;
    public float defaultMagnitude = 0.25f;

    Vector3 originalPos;
    Coroutine shakeRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        originalPos = transform.localPosition;
    }

    void OnDisable()
    {
        transform.localPosition = originalPos;
    }
    public void Shake()
    {
        Shake(defaultMagnitude, defaultDuration);
    }
    public void Shake(float magnitude, float duration)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(DoShake(magnitude, duration));
    }

    IEnumerator DoShake(float magnitude, float duration)
    {
        float elapsed = 0f;
        originalPos = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percentComplete = elapsed / duration;
            // optional: ease out so shake decreases toward the end
            float damper = 1f - Mathf.Clamp01(percentComplete);

            float x = (Random.value * 2f - 1f) * magnitude * damper;
            float y = (Random.value * 2f - 1f) * magnitude * damper;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            yield return null;
        }

        transform.localPosition = originalPos;
        shakeRoutine = null;
    }
}
