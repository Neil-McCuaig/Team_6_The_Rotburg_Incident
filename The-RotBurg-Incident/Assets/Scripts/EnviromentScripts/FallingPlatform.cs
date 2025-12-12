using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public float shakeDelay = 0.2f;   
    public float fallDelay = 1f;      

    [Header("Respawn Settings")]
    public bool respawn;
    public float respawnTime = 3f;    

    private Collider2D col2D;
    private Animator anim;
    private bool triggered = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        col2D = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!triggered && (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy")))
        {
            triggered = true;
            Invoke(nameof(PlayShake), shakeDelay);
            Invoke(nameof(PlayFall), fallDelay);
        }
    }

    void PlayShake()
    {
        anim.SetTrigger("shake");
    }

    void PlayFall()
    {
        anim.SetTrigger("fall");
        col2D.enabled = false;

        if (respawn)
        {
            Invoke(nameof(RespawnPlatform), respawnTime);
        }
    }

    void RespawnPlatform()
    {
        anim.SetTrigger("reset");
        triggered = false;
    }

    public void ResetCollider()
    {
        col2D.enabled = true;
    }
}
