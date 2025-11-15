using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLogic : MonoBehaviour
{
    public GameObject teleportPoint;

    private Transform destination;

    //public GameObject fadeToBlack;

    FadeToBlack fade;

    // Start is called before the first frame update
    void Start()
    {
        destination = teleportPoint.transform;

        fade = FindAnyObjectByType<FadeToBlack>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("You stepped on spikes!");
            FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);
            //fade.GetComponent<FadeToBlack>.FadeIn;

            other.transform.position = new Vector2(destination.position.x, destination.position.y);
        }
        else if(other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyHealth>().health = 0;
        }
    }
}
