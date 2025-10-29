using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StreamChat : MonoBehaviour
{
    public static StreamChat Instance;

    [Header("Chat UI References")]
    public GameObject chatPanel;               
    public ScrollRect scrollRect;              
    public TextMeshProUGUI chatText; 
    RealTimeClock time;

    [Header("Settings")]
    public int maxMessages = 50;               
    public KeyCode toggleKey = KeyCode.C;      
    public float messageInterval = 3f;         

    [Header("Starting Messages")]
    [TextArea(2, 10)]
    public List<string> startingMessages = new List<string>();

    private List<string> messages = new List<string>();
    private bool chatVisible = true;
    private float messageTimer = 0f;
    private int nextMessageIndex = 0;

    void Start()
    {
        time = FindAnyObjectByType<RealTimeClock>();
        chatPanel.SetActive(chatVisible);
        chatText.text = "";

        if (startingMessages.Count > 0)
        {
            AddMessage(startingMessages[0]);
            nextMessageIndex = 1;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            chatVisible = !chatVisible;
            chatPanel.SetActive(chatVisible);
        }
        messageTimer += Time.deltaTime;
        if (messageTimer >= messageInterval && nextMessageIndex < startingMessages.Count)
        {
            AddMessage(startingMessages[nextMessageIndex]);
            nextMessageIndex++;
            messageTimer = 0f;
        }
    }
    public void AddMessage(string message)
    {
        messages.Add(message);
        if (messages.Count > maxMessages)
        {
            messages.RemoveAt(0);
        }

        chatText.text = string.Join("\n" + time.formattedTime + ": ", messages);
        ScrollToBottom();
    }
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
    public void PostMessage(string message)
    {
        AddMessage(message);
    }
}
