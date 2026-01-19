using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLogic : MonoBehaviour
{
    [Header("Hazzard Settings")]
    public float fadeInDelay = 0.5f;
    public float fadeOutDelay = 0.5f;
    public float damageAmount = 10f;
    private Transform playerPos;

    FadeToBlack fade;
    PlayerController player;
    Animator anim;
    PlayerHealth healthMessage;

    void Start()
    {
        fade = FindAnyObjectByType<FadeToBlack>();
        healthMessage = FindAnyObjectByType<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim = other.GetComponent<Animator>();
            anim.SetBool("IsJumping", false);
            anim.SetInteger("WalkX", 0);
            player = other.GetComponent<PlayerController>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                if(health.currentHealth >= 11f)
                {
                    StartCoroutine(TeleportAfterDelay(playerPos, player));
                }
                playerPos = other.transform;
                other.GetComponent<PlayerHealth>().TakeDamage(damageAmount);

                //Should inform the variable that the player has been damaged by a trap
                //other.GetComponent<PlayerHealth>().trapDamaged = true;

                if (player.rb != null)
                {
                    player.rb.velocity = Vector3.zero;
                }
            }
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.GetComponent<EnemyHealth>() != null)
            {
                other.GetComponent<EnemyHealth>().health -= 100f;
            }
        }
    }

    private IEnumerator TeleportAfterDelay(Transform playerPos, PlayerController player)
    {
        if (player.rb != null)
        {
            player.rb.velocity = Vector3.zero;
        }
        player.canMove = false;
        fade.FadeOut();

        yield return new WaitForSeconds(fadeOutDelay);
        StartCoroutine(HandleFadeIn());
    }
    private IEnumerator HandleFadeIn()
    {
        if (player.rb != null)
        {
            player.rb.velocity = Vector3.zero;
        }
        fade.FadeIn();

        //healthMessage.trapDamaged = false;

        playerPos.position = player.lastGroundedPosition;

        yield return new WaitForSeconds(fadeInDelay);
        player.canMove = true;
    }
}
