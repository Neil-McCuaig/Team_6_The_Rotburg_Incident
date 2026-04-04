using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerController;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;

    public float invincibilityDuration = 2f;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;
    private GameObject playerArm;
    private SpriteRenderer armRender;
    PlayerController playerController;
    ViewerStats viewersStats;
    
    StreamChat chat;
    public Sprite hurtEmote;

    private SpriteRenderer weepingSprite;

    public float chatDamageTime;
    public float chatDamageCurrentTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerArm = GameObject.Find("Arm_Sprite");
        armRender = playerArm.GetComponent<SpriteRenderer>();
        playerController = FindAnyObjectByType<PlayerController>();
        chat = FindAnyObjectByType<StreamChat>();
        viewersStats = FindAnyObjectByType<ViewerStats>();

        ResetHealthFull();

        chatDamageTime = 5f;
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        if (isInvincible)
        {
            return;
        }

        CameraManager.instance.ScreenShake(20f, 1f);
        currentHealth -= damage;

        SoundManager.instance.PlaySound(SoundManager.instance.playerHurt);
        SoundManager.instance.PlaySound(SoundManager.instance.playerHurtSquish);
        SoundManager.instance.PlaySound(SoundManager.instance.playerHurtSplash);

        //Does not reset to the default
        //chat.SwitchMessageList(2);
        //chat.messageLifetime = 1f;
        //chat.spawnDelay = 0.5f;

        if (currentHealth <= 0)
        {
            Die(attacker);
            //chat.AddMessage("[CareTaker] OMG f's in the chat to pay repects he got wrecked", hurtEmote, true);
            chat.SwitchMessageList(1);
            chat.messageLifetime = 1f;
            chat.spawnDelay = 0.25f;
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

        //Does not reset to the default without the area below the while loop having chat.SwitchMessageList(0)
        //chat.SwitchMessageList(2);
        //chat.messageLifetime = 1f;
        //chat.spawnDelay = 0.4f;

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
        //Does not last for as long as I'd like, but technically works.
        //chat.SwitchMessageList(0);
    }

    private void Die(GameObject killer)
    {
        DeathType deathType = DeathType.Normal;

        if (killer != null && killer.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (killer.GetComponent<JumpingEnemyController>() != null)
            {
                deathType = DeathType.Pouncer;
                Destroy(killer);
            }
            else if (killer.GetComponent<FlyingEnemyController>() != null)
            {
                deathType = DeathType.Flyer;
                Destroy(killer);
            }
            else if (killer.GetComponent<ExsplodeEnemyController>() != null)
            {
                deathType = DeathType.Popper;
                Destroy(killer);
            }
            else if (killer.GetComponent<WeepingAngelBehavior>() != null)
            {
                deathType = DeathType.WeepingAngel;
                weepingSprite = killer.GetComponent<SpriteRenderer>();
                if (weepingSprite != null)
                {
                    weepingSprite.enabled = false;
                }
            }
            else if (killer.GetComponent<DamagePlayer>() != null) // Hall Monitor
            {
                deathType = DeathType.HallMonitor;
            }
        }

        playerController.Die(deathType);
    }

    public void ResetHealthFull()
    {
        maxHealth = viewersStats.maxPlayerHealth;
        currentHealth = maxHealth;
        chat.SwitchMessageList(0);
        chat.messageLifetime = 5f;
        chat.spawnDelay = 1f;
    }

    public void ResetWeepingSpriteRenderer()
    {
        weepingSprite.enabled = true;
    }
}
