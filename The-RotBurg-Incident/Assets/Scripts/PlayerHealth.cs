using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int health;
    public int maxHealth;
    public int tempTakeDamage;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        health = maxHealth;
        tempTakeDamage = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 0) 
        { 
            //Goto the game over screen.
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Enemy)
        {
            health = health - tempTakeDamage;
        }
    }
}
