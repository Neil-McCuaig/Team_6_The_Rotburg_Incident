using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            health = health - tempTakeDamage;
            Debug.Log(health);
            if (health <= 0)
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        }
    }
}
