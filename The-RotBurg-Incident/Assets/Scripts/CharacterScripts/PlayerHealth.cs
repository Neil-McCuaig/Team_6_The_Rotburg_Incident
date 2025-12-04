using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public float invincibilityDuration = 2f;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;
    private GameObject playerArm;
    private SpriteRenderer armRender;
    PlayerController playerController;
    
    StreamChat chat;
    public Sprite hurtEmote;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerArm = GameObject.Find("Arm_Sprite");
        armRender = playerArm.GetComponent<SpriteRenderer>();
        playerController = FindAnyObjectByType<PlayerController>();
        chat = FindAnyObjectByType<StreamChat>();

        ResetHealthFull();
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }

        CameraManager.instance.ScreenShake(20f, 1f);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            chat.AddMessage("[CareTaker] OMG f's in the chat to pay repects he got wrecked", hurtEmote, true);
            playerController.Die();
        }
        else if(spriteRenderer != null) 
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < invincibilityDuration)
        {
            float newAlpha = spriteRenderer.color.a == 1f ? 0f : 1f;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            armRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        // Reset to fully visible
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        isInvincible = false;
    }

    public void ResetHealthFull()
    {
        currentHealth = maxHealth;
    }
}
