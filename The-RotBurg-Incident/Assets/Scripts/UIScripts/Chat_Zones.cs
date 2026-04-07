using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_Zones : MonoBehaviour
{
    StreamChat chat;

    public int chatZoneNumber;

    // Start is called before the first frame update
    void Start()
    {
        chat = FindAnyObjectByType<StreamChat>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //What this SHOULD do is make it so that upon entering a Chatzone, the default chat will be switched to a certain
    //Chatlog. The intent is Whenever something like an enemy is not being observed, the chat will return to that 
    //Default index.
    public void OnTriggerEnter2D(Collider2D other)
    {
        //PlayerController player = FindAnyObjectByType<PlayerController>();

        if (other.CompareTag("Player"))
        {
            chat.SwitchMessageList(chatZoneNumber);
            chat.SetDefaultChatlist();
        }
    }
}
