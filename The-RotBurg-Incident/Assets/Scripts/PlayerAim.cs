using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("Arm Settings")]
    public Transform arm; 
    public bool flipArmLeft = true; 
    public SpriteRenderer spriteRender;

    public GameObject stunEffect;
    public Transform cameraFlash;
    public Transform flashLeft;
    public Transform flashRight;
    private bool canFlash = true;
    public SpriteRenderer effectRender;

    void Update()
    {
        AimAtMouse();
        if (Input.GetMouseButtonDown(0) && canFlash) // Left mouse button
        {
            ActivateFlash();
        }
    }

    void AimAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 direction = mousePos - arm.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        arm.rotation = Quaternion.Euler(0f, 0f, angle);

        if (flipArmLeft)
        {
            if (angle > 90 || angle < -90)
            {
                spriteRender.flipY = true;
                cameraFlash.position = flashLeft.position;
            }
            else
            {
                spriteRender.flipY = false;
                cameraFlash.position = flashRight.position;
            }
        }
    }

    void ActivateFlash()
    {
        canFlash = false;
        stunEffect.SetActive(true);
        Color originalColor = effectRender.color;
        effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        StartCoroutine(DelayedAction());
    }

    IEnumerator DelayedAction()
    {
        float elapsed = 0f;
        Color originalColor = effectRender.color;

        while (elapsed < 1f)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / 1f);
            effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        stunEffect.SetActive(false);
        canFlash = true;
    }
}
