using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopUp : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject popupRoot;               
    public Image popupGraphic;               

    [Header("Settings")]  
    public float fadeDuration = 0.5f;
    private bool hasActivated = false;

    PlayerController playerController;

    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        SetGraphicAlpha(0f);
        popupRoot.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated)
        {
            return;
        }
        if (!other.CompareTag("Player"))
        {
            return;
        }
        hasActivated = true;
        playerController.canMove = false;
        StartCoroutine(ShowTutorial());
    }

    private IEnumerator ShowTutorial()
    {
        popupRoot.SetActive(true);

        yield return FadeGraphic(0f, 1f);

        yield return new WaitUntil(() => playerController.attackAction.WasPressedThisFrame());

        yield return FadeGraphic(1f, 0f);

        playerController.canMove = true;
        Destroy(gameObject);
    }

    private IEnumerator FadeGraphic(float start, float end)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, t / fadeDuration);
            SetGraphicAlpha(alpha);
            yield return null;
        }

        SetGraphicAlpha(end);
    }

    private void SetGraphicAlpha(float alpha)
    {
        if (popupGraphic == null)
        {
            return;
        }
        Color c = popupGraphic.color;
        c.a = alpha;
        popupGraphic.color = c;
    }
}
