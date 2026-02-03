using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallMonitorBehavior : MonoBehaviour, MonnsterActivation
{
    [Header("Overlay Settings")]
    public GameObject overlayPrefab;
    public Transform[] spawnPoints;
    public Transform player;
    private List<GameObject> activeOverlays = new List<GameObject>();
    private List<Animator> overlayAnims = new List<Animator>();
    private float overlayWidth;
    private float overlayHeight;

    private bool isActive;

    [Header("State Management")]
    public float greenLightDuration = 60f;
    public float greenLightStartUp = 2.5f;
    public float redLightDuration = 4f;
    public float redLightStartUp = 0.6f;
    bool greenLightBegun;
    bool redLightBegun;
    bool playerCaught;

    [HideInInspector]
    [Header("Animation Settings")]
    private bool beginStateCycle;
    private float animationTimer;
    private int eyeIndex;

    [Header("Gameplay References")]
    PlayerController playerController;
    public float damageAmount;

    private enum State { GreenLight, RedLight }
    private State currentState = State.GreenLight;

    private float EyeStepTime => greenLightDuration / 6f;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        SpriteRenderer sr = overlayPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            overlayWidth = sr.bounds.size.x;
            overlayHeight = sr.bounds.size.y;
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        if (activeOverlays.Count == 0)
        {
            SpawnOverlays();
        }
        OverlayGrid();

        switch (currentState)
        {
            case State.GreenLight:
            {
                HandleGreenLight();
                break;
            }

            case State.RedLight:
            { 
                HandleRedLight();
                break;
            }
        }
    }

    public void SetActiveState(bool value)
    {
        if (isActive == value)
        {
            return;
        }
        isActive = value;

        if (!isActive)
        {
            ClearOverlays();
            ResetCycle();
        }
    }

    void SpawnOverlays()
    {
        for (int i = 0; i < Mathf.Min(6, spawnPoints.Length); i++)
        {
            GameObject overlay = Instantiate(overlayPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            activeOverlays.Add(overlay);

            Animator anim = overlay.GetComponent<Animator>();
            if (anim != null)
            {
                overlayAnims.Add(anim);
            }
        }
    }

    void ClearOverlays()
    {
        foreach (var overlay in activeOverlays)
        {
            if (overlay != null)
            {
                Destroy(overlay);
            }
        }
        activeOverlays.Clear();
        overlayAnims.Clear();
    }

    void OverlayGrid()
    {
        if (activeOverlays.Count < 6)
        {
            return;
        }

        float leftEdge = float.MaxValue;
        float rightEdge = float.MinValue;
        float topEdge = float.MinValue;
        float bottomEdge = float.MaxValue;

        foreach (GameObject overlay in activeOverlays)
        {
            Vector3 pos = overlay.transform.position;

            leftEdge = Mathf.Min(leftEdge, pos.x);
            rightEdge = Mathf.Max(rightEdge, pos.x);
            topEdge = Mathf.Max(topEdge, pos.y);
            bottomEdge = Mathf.Min(bottomEdge, pos.y);
        }

        if (player.position.x - leftEdge > overlayWidth)
        {
            ShiftColumn(leftEdge, Vector3.right * overlayWidth * 3);
        }
        if (rightEdge - player.position.x > overlayWidth)
        {
            ShiftColumn(rightEdge, Vector3.left * overlayWidth * 3);
        }
        if (player.position.y - bottomEdge > overlayHeight)
        {
            ShiftRow(bottomEdge, Vector3.up * overlayHeight * 2);
        }
        if (topEdge - player.position.y > overlayHeight)
        {
            ShiftRow(topEdge, Vector3.down * overlayHeight * 2);
        }
    }

    void ShiftColumn(float edge, Vector3 offset)
    {
        foreach (var overlay in activeOverlays)
        {
            if (Mathf.Abs(overlay.transform.position.x - edge) < 0.01f)
            {
                overlay.transform.position += offset;
            }
        }
    }

    void ShiftRow(float edge, Vector3 offset)
    {
        foreach (var overlay in activeOverlays)
        {
            if (Mathf.Abs(overlay.transform.position.y - edge) < 0.01f)
            {
                overlay.transform.position += offset;
            }
        }
    }

    void HandleGreenLight()
    {
        if (!greenLightBegun)
        {
            greenLightBegun = true;
            foreach (var locker in LockerInteraction.allLockers)
            {
                locker.UnSealLockers();
            }
            animationTimer = 0f;
            eyeIndex = 0;

            StartCoroutine(GreenLightStartup());
        }

        if (!beginStateCycle)
        {
            return;
        }

        animationTimer += Time.deltaTime;
        if (animationTimer >= EyeStepTime && eyeIndex < 6)
        {
            animationTimer = 0f;
            eyeIndex++;
            TriggerAll("NextState");
        }
    }

    IEnumerator GreenLightStartup()
    {
        yield return new WaitForSeconds(greenLightStartUp);
        TriggerAll("StartStates");
        beginStateCycle = true;

        yield return new WaitForSeconds(greenLightDuration);
        beginStateCycle = false;
        playerCaught = !playerController.inLocker;
        SetBoolAll("PlayerCaught", playerCaught);

        yield return new WaitForSeconds(redLightStartUp);
        TriggerAll("NextState");
        currentState = State.RedLight;
        greenLightBegun = false;
    }

    void HandleRedLight()
    {
        if (!redLightBegun)
        {
            redLightBegun = true;
            foreach (var locker in LockerInteraction.allLockers)
            {
                locker.SealAllLockers();
            }
            if (playerCaught)
            {
                FindAnyObjectByType<PlayerHealth>().TakeDamage(damageAmount);
            }
            StartCoroutine(RedLightDuration());
        }
    }

    IEnumerator RedLightDuration()
    {
        yield return new WaitForSeconds(redLightDuration);
        TriggerAll("ResetStates");
        currentState = State.GreenLight;
        redLightBegun = false;
    }

    void TriggerAll(string triggerName)
    {
        foreach (var anim in overlayAnims)
        {
            if (anim != null)
            {
                anim.SetTrigger(triggerName);
            }
        }
    }

    void SetBoolAll(string boolName, bool value)
    {
        foreach (var anim in overlayAnims)
        {
            if (anim != null)
            {
                anim.SetBool(boolName, value);
            }
        }
    }

    void ResetCycle()
    {
        greenLightBegun = false;
        redLightBegun = false;
        beginStateCycle = false;
        animationTimer = 0f;
        eyeIndex = 0;
        currentState = State.GreenLight;
    }
}

