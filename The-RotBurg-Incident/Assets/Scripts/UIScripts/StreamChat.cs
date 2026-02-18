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

    [System.Serializable]
    public class ChatMessageList
    {
        public string listName; // Helpful label in Inspector
        public List<ChatEntry> messages = new List<ChatEntry>();
    }

    [Header("References")]
    public RectTransform contentParent;
    public ScrollRect scrollRect;
    public GameObject messagePrefab;

    [Header("Message Lists")]
    public List<ChatMessageList> messageLists = new List<ChatMessageList>();
    public int currentListIndex = 0;

    [Header("Chat Settings")]
    public float messageLifetime = 5f;
    public float fadeDuration = 1f;
    public float spawnDelay = 2f;
    public int maxMessages;
    public bool repeatMessages = false;

    [Header("Bool Checks")]
    public bool autoStart = true;
    public bool chatVisible = true;

    private int nextIndex;
    private Coroutine playRoutine;

    void Start()
    {
        if (autoStart && messageLists.Count > 0)
        {
            PlayCurrentList();
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

    public void PlayCurrentList()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }
        nextIndex = 0;
        playRoutine = StartCoroutine(PlayMessages(messageLists[currentListIndex]));
    }

    IEnumerator PlayMessages(ChatMessageList list)
    {
        while (nextIndex < list.messages.Count)
        {
            if (nextIndex == (maxMessages - 1) && repeatMessages)
            {
                nextIndex = 0;
            }

            CreateMessage(list.messages[nextIndex]);
            nextIndex++;

            yield return new WaitForSeconds(spawnDelay);
        }
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
                {
                    StartCoroutine(AnimateEmoji(img.rectTransform));
                }
            }
            else
            {
                img.enabled = false;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent);
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
    public void SetChatTimings(float lifetime, float fade, float delay)
    {
        messageLifetime = Mathf.Max(0f, lifetime);
        fadeDuration = Mathf.Max(0f, fade);
        spawnDelay = Mathf.Max(0f, delay);
    }
    public void SwitchMessageList(int newIndex)
    {
        if (newIndex < 0 || newIndex >= messageLists.Count)
        {
            return;
        }
        currentListIndex = newIndex;
        PlayCurrentList();
    }
    public void AddMessage(string text, Sprite emoji = null, bool animate = false)
    {
        ChatEntry entry = new ChatEntry { messageText = text, emoji = emoji, animateEmoji = animate };

        CreateMessage(entry);
    }
}
