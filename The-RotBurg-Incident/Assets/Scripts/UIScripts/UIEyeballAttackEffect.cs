using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEyeballAttackEffect : MonoBehaviour
{
    [Header("References")]
    public Image overlayImage; 

    [Header("Fade Duration")]
    public float fadeDuration = 1.5f;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        SetAlpha(0f);
        overlayImage.gameObject.SetActive(false);
    }

    public void ActivateOverlay()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        overlayImage.gameObject.SetActive(true);
        SetAlpha(1f); 

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        overlayImage.gameObject.SetActive(false);
    }

    void SetAlpha(float alpha)
    {
        Color color = overlayImage.color;
        color.a = alpha;
        overlayImage.color = color;
    }
}
