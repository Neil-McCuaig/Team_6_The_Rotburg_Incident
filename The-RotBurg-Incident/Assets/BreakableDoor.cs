using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableDoor : MonoBehaviour
{
    private EnemyHealth health;
    private Animator anim;
    private Collider2D col2D;  

    void Start()
    {
        health = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>();
        col2D = GetComponent<Collider2D>();
    }
    void Update()
    {
        if (health.health < health.currentHealth)
        {
            anim.SetTrigger("Attacked");
        }
    }
    public void DisableCollider()
    {
        col2D.enabled = false;
    }
}
