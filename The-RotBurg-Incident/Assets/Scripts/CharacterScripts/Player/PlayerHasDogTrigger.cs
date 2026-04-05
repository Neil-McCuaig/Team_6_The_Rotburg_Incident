using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHasDogTrigger : MonoBehaviour
{
    PlayerController player;
    public bool givePlayerDog;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (player != null)
            {
                if (givePlayerDog)
                {
                    player.EnableDog();
                    player.hasDog = true;
                }
                else
                {
                    player.DisableDog();
                    player.hasDog = false;
                }
            }
        }
    }
}
