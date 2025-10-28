using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public Image fadeImage;            
    public float fadeDuration = 1f;    

    public void FadeIn() => StartCoroutine(Fade(1f, 0f)); 
    public void FadeOut() => StartCoroutine(Fade(0f, 1f));

    private void Awake()
    {
        gameObject.SetActive(true);
        FadeIn();
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, to);
    }
}
