using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_Vision : MonoBehaviour
{

    StreamChat chat;

    //JumpingEnemyController pouncer;

    public float chatCooldownTime;
    public float chatCooldownCurrentTime;
    //public bool readyToSwitchChats;

    // Start is called before the first frame update
    void Start()
    {
        chat = FindAnyObjectByType<StreamChat>();
        //pouncer = FindObjectsByType<JumpingEnemyController>(FindObjectsSortMode.None)[0];
        chatCooldownTime = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        //When the cooldown time hits zero, switch to the default chat index
        if (chatCooldownCurrentTime <= 0)
        {
            //readyToSwitchChats = true;
            chatCooldownCurrentTime = chatCooldownTime;
            chat.SwitchMessageList(chat.defaultChatlist);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.GetComponent<pouncer>())
        //{

        //}
        JumpingEnemyController pouncer = other.GetComponent<JumpingEnemyController>();
        FlyingEnemyController flyer = other.GetComponent<FlyingEnemyController>();

        if (pouncer != null)
        {
            //&& readyToSwitchChats == true
            chat.SwitchMessageList(5);
            chat.messageLifetime = 2.5f;
            chat.spawnDelay = 1f;
            chatCooldownCurrentTime -= Time.deltaTime;
        }
        if (flyer != null)
        {
            chat.SwitchMessageList(7);
            chat.messageLifetime = 2.5f;
            chat.spawnDelay = 1f;
            chatCooldownCurrentTime -= Time.deltaTime;
        }
    }
}
