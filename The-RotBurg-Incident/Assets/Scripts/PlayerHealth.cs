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
    HealthBarSlider healthBar;
    PlayerController playerController;
    EnemySpawnerManager enemySpawnerManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBar = FindAnyObjectByType<HealthBarSlider>();
        playerController = FindAnyObjectByType<PlayerController>();
        enemySpawnerManager = FindAnyObjectByType<EnemySpawnerManager>();

        ResetHealthFull();
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
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
        healthBar.SetMaxHealth(maxHealth);
    }
}
