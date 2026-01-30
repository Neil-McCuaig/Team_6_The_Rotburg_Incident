using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeballsOverlayBackground : MonoBehaviour
{
    [Header("References")]
    public GameObject overlayBackground;
    public Transform[] spawnPoints;
    public Transform player;
    private List<GameObject> activeOverlays = new List<GameObject>();

    [Header("ACTIVATE OVERLAY")]
    public bool activateOverlay;

    private float overlayWidth;
    private float overlayHeight;
    private bool isActive;

    private void Start()
    {
        SpriteRenderer sr = overlayBackground.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            return;
        }

        overlayWidth = sr.bounds.size.x;
        overlayHeight = sr.bounds.size.y;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !activateOverlay)
        {
            return;
        }
        if (activeOverlays.Count > 0)
        {
            return;
        }
        for (int i = 0; i < Mathf.Min(6, spawnPoints.Length); i++)
        {
            GameObject overlay = Instantiate(overlayBackground, spawnPoints[i].position, spawnPoints[i].rotation);
            activeOverlays.Add(overlay);
        }

        isActive = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !activateOverlay)
        {
            return;
        }
        foreach (GameObject overlay in activeOverlays)
        {
            Destroy(overlay);
        }

        activeOverlays.Clear();
        isActive = false;
    }

    private void Update()
    {
        if (!isActive || activeOverlays.Count < 6)
        {
            return;
        }
        float leftEdge = float.MaxValue;
        float rightEdge = float.MinValue;
        float topEdge = float.MinValue;
        float bottomEdge = float.MaxValue;

        foreach (GameObject overlays in activeOverlays)
        {
            Vector3 position = overlays.transform.position;

            leftEdge = Mathf.Min(leftEdge, position.x);
            rightEdge = Mathf.Max(rightEdge, position.x);
            topEdge = Mathf.Max(topEdge, position.y);
            bottomEdge = Mathf.Min(bottomEdge, position.y);
        }

        if (player.position.x - leftEdge > overlayWidth)
        {
            foreach (GameObject overlays in activeOverlays)
            {
                if (Mathf.Abs(overlays.transform.position.x - leftEdge) < 0.01f)
                {
                    overlays.transform.position += Vector3.right * overlayWidth * 3;
                }
            }
        }

        if (rightEdge - player.position.x > overlayWidth)
        {
            foreach (GameObject overlays in activeOverlays)
            {
                if (Mathf.Abs(overlays.transform.position.x - rightEdge) < 0.01f)
                {
                    overlays.transform.position += Vector3.left * overlayWidth * 3;
                }
            }
        }

        if (player.position.y - bottomEdge > overlayHeight)
        {
            foreach (GameObject overlays in activeOverlays)
            {
                if (Mathf.Abs(overlays.transform.position.y - bottomEdge) < 0.01f)
                {
                    overlays.transform.position += Vector3.up * overlayHeight * 2;
                }
            }
        }

        if (topEdge - player.position.y > overlayHeight)
        {
            foreach (GameObject overlays in activeOverlays)
            {
                if (Mathf.Abs(overlays.transform.position.y - topEdge) < 0.01f)
                {
                    overlays.transform.position += Vector3.down * overlayHeight * 2;
                }
            }
        }
    }
}
