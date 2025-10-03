using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float currentHealth;
    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < currentHealth && !isDead)
        {
            currentHealth = health;
            anim.SetTrigger("Attacked");
        }
        if (health <= 0)
        {
            isDead = true;
            anim.SetBool("IsDead", true);
        }
    }
}
