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
        if (chatCooldownCurrentTime > 0 && chat.currentListIndex != chat.defaultChatlist)
        {
            chatCooldownCurrentTime -= Time.deltaTime;
        }
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
        ExsplodeEnemyController popper = other.GetComponent<ExsplodeEnemyController>();
        WeepingAngelBehavior angel = other.GetComponent<WeepingAngelBehavior>();

        //At the moment, if a lot of enemies are in one area, it spawns a fuckton of messages. This needs fixed.
        if (pouncer != null && chat.currentListIndex != 11)
        {
            //&& readyToSwitchChats == true
            chat.SwitchMessageList(11);
            chat.messageLifetime = 2.5f;
            chat.spawnDelay = 1f;
            //Only ticks down a fraction of a sec.
            //chatCooldownCurrentTime -= Time.deltaTime;
        }
        if (flyer != null && chat.currentListIndex != 12)
        {
            chat.SwitchMessageList(12);
            chat.messageLifetime = 2.5f;
            chat.spawnDelay = 1f;
            //chatCooldownCurrentTime -= Time.deltaTime;
        }
        if (popper != null && chat.currentListIndex != 13)
        {
            chat.SwitchMessageList(13);
            chat.messageLifetime = 2.5f;
            chat.spawnDelay = 1f;
        }
        if (angel != null && chat.currentListIndex != 15)
        {
            chat.SwitchMessageList(15);
            chat.messageLifetime = 2f;
            chat.spawnDelay = 1.2f;
        }

    }
}
