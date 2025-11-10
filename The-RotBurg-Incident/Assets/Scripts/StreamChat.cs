using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StreamChat : MonoBehaviour
{
    [System.Serializable]
    public class ChatEntry
    {
        [TextArea(1, 3)] public string messageText;
        public Sprite emoji;               
        public bool animateEmoji;         
    }

    [Header("References")]
    public RectTransform contentParent;  
    public ScrollRect scrollRect;
    public GameObject messagePrefab;      
    public List<ChatEntry> startingMessages = new List<ChatEntry>();
    
    [Header("Chat Settings")]
    public float messageLifetime = 5f;    
    public float fadeDuration = 1f;
    public float spawnDelay = 2f;
    private int nextIndex;
    public int maxMessages;

    [Header("Bool Checks")]
    public bool autoStart = true;
    public bool chatVisible = true;
    public bool repeatMessages;

    void Start()
    {
        if (autoStart)
        {
            StartCoroutine(PlayStartingMessages());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            chatVisible = !chatVisible;
            contentParent.gameObject.SetActive(chatVisible);
        }
    }

    IEnumerator PlayStartingMessages()
    {
        while (nextIndex < startingMessages.Count)
        {
            if (nextIndex == (maxMessages - 1) && repeatMessages)
            {
                nextIndex = 0;
            }
            CreateMessage(startingMessages[nextIndex]);
            nextIndex++;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void AddMessage(string text, Sprite emoji = null, bool animate = false)
    {
        ChatEntry entry = new ChatEntry { messageText = text, emoji = emoji, animateEmoji = animate };
        CreateMessage(entry);
    }

    private void CreateMessage(ChatEntry entry)
    {
        if (!messagePrefab || !contentParent)
        {
            return;
        }

        GameObject newMsg = Instantiate(messagePrefab, contentParent);
        newMsg.transform.SetAsLastSibling();
        TextMeshProUGUI text = newMsg.GetComponentInChildren<TextMeshProUGUI>();
        Image img = newMsg.GetComponentInChildren<Image>();

        if (text)
        {
            text.text = entry.messageText;
        }
        if (img)
        {
            if (entry.emoji)
            {
                img.sprite = entry.emoji;
                img.enabled = true;
                if (entry.animateEmoji)
                    StartCoroutine(AnimateEmoji(img.rectTransform));
            }
            else
            {
                img.enabled = false;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
        Canvas.ForceUpdateCanvases();
        StartCoroutine(FadeAndDestroy(newMsg));

        if (scrollRect != null)
        {
            StartCoroutine(ScrollToBottom());
        }
    }

    IEnumerator ScrollToBottom()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0f;
    }

    IEnumerator FadeAndDestroy(GameObject obj)
    {
        CanvasGroup cg = obj.AddComponent<CanvasGroup>();
        yield return new WaitForSeconds(messageLifetime);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        Destroy(obj);
    }

    IEnumerator AnimateEmoji(RectTransform emoji)
    {
        float t = 0f;
        while (emoji != null)
        {
            t += Time.deltaTime * 2f;
            float scale = 1f + Mathf.Sin(t * 3f) * 0.1f;  
            emoji.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
    }
}
