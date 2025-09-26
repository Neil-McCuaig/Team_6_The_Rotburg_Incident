using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLogic : MonoBehaviour
{
    public GameObject teleportPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnCollisionEnter2D(Collision2D other)
    //{
        //if (other.gameObject.CompareTag("Player"))
        //{
            //Debug.Log("You stepped on spikes!");
            //FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);
        //}
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("You stepped on spikes!");
            FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);
            
        }
    }
}
