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
        public Sprite emoji;               // Optional image for emoji
        public bool animateEmoji;          // If true, emoji will pulse or bounce
    }

    [Header("Chat Settings")]
    public RectTransform contentParent;   // Where messages appear (e.g. ScrollView Content)
    public ScrollRect scrollRect;
    public GameObject messagePrefab;      // Prefab with TextMeshProUGUI + optional Image
    public List<ChatEntry> startingMessages = new List<ChatEntry>();
    public float messageLifetime = 5f;    // How long messages stay before fading
    public float fadeDuration = 1f;
    public float spawnDelay = 2f;         // Time between each starting message
    public bool autoStart = true;
    public bool chatVisible = true;

    private float timer;
    private int nextIndex;

    void Start()
    {
        if (autoStart)
            StartCoroutine(PlayStartingMessages());
    }

    void Update()
    {
        // Toggle chat visibility with a key (optional)
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
            Debug.LogError("Missing messagePrefab or contentParent!");
            return;
        }

        GameObject newMsg = Instantiate(messagePrefab, contentParent);
        newMsg.transform.SetAsLastSibling();

        // --- Setup visuals ---
        TextMeshProUGUI text = newMsg.GetComponentInChildren<TextMeshProUGUI>();
        Image img = newMsg.GetComponentInChildren<Image>();

        if (text) text.text = entry.messageText;
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

        //  OPTION B — dynamic spacing tweak based on message length
        HorizontalLayoutGroup layout = newMsg.GetComponent<HorizontalLayoutGroup>();
        if (layout != null && text != null)
        {
            // Shorter messages get tighter spacing between emoji and text
            if (text.text.Length < 10)
                layout.spacing = 2f;
            else if (text.text.Length < 30)
                layout.spacing = 4f;
            else
                layout.spacing = 6f;
        }

        // Force layout rebuild so spacing applies immediately
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
        Canvas.ForceUpdateCanvases();

        // Fade & scroll like before
        StartCoroutine(FadeAndDestroy(newMsg));
        if (scrollRect != null)
            StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        // Wait 1 frame so the layout system updates first
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
            float scale = 1f + Mathf.Sin(t * 3f) * 0.1f;  // gentle bounce
            emoji.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
    }
}
