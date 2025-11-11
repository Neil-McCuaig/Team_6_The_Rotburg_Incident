using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject audioSettingsUI;

    public bool isPaused = false;
    private bool inAudioSettings = false;
    PlayerController playerController;
    HoverLogic hoverLogic;

    [Header("Cursor Settings")]
    public Texture2D cursorCamTexture;
    public Texture2D cursorFingerTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("Powerups")]
    public GameObject doubleJumpUI;
    public GameObject doubleJumpUIText;

    private void Start()
    {
        Cursor.SetCursor(cursorCamTexture, hotspot, cursorMode);
        playerController = FindAnyObjectByType<PlayerController>();
        hoverLogic = FindAnyObjectByType<HoverLogic>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inAudioSettings)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                PauseGame();
            }
        }
        if (inAudioSettings && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAudioSettings();
        }
    }

    public void PauseGame()
    {
        Cursor.SetCursor(cursorFingerTexture, hotspot, cursorMode);
        pauseMenuUI.SetActive(true);
        doubleJumpUI.SetActive(false);
        audioSettingsUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        inAudioSettings = false;

        if (playerController.hasDoubleJump == true)
        {
            doubleJumpUI.SetActive(true);
            doubleJumpUIText.SetActive(false);

            if (hoverLogic.currentlyHovering == true)
            {
                doubleJumpUI.SetActive(true);
            }
        }
    }

    public void Resume()
    {
        Cursor.SetCursor(cursorCamTexture, hotspot, cursorMode);
        pauseMenuUI.SetActive(false);
        audioSettingsUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        inAudioSettings = false;
    }

    public void OpenAudioSettings()
    {
        pauseMenuUI.SetActive(false);
        audioSettingsUI.SetActive(true);
        inAudioSettings = true;
    }

    public void CloseAudioSettings()
    {
        audioSettingsUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        inAudioSettings = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        Resume();
        playerController.Die();
    }

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }


}
