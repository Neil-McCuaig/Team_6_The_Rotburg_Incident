using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5;
    private float currentHealth;

    public float invincibilityDuration = 2f;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;
    HealthBarSlider healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBar = FindAnyObjectByType<HealthBarSlider>();
        healthBar.SetMaxHealth(maxHealth);
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
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        SceneManager.LoadScene("SampleScene");
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}
