using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public bool isPaused = false;
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    [Header("Settings Sub-Menus")]
    public GameObject audioSubMenu;
    public GameObject controlsSubMenu;

    private bool inSettingsMenu = false;
    PlayerController playerController;
    HoverLogic hoverLogic;

    [Header("Cursor Settings")]
    public Texture2D cursorCamTexture;
    public Texture2D cursorFingerTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        Cursor.SetCursor(cursorCamTexture, hotspot, cursorMode);
        playerController = FindAnyObjectByType<PlayerController>();
        hoverLogic = FindAnyObjectByType<HoverLogic>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inSettingsMenu)
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
        if (inSettingsMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();
        }
    }

    public void PauseGame()
    {
        Cursor.SetCursor(cursorFingerTexture, hotspot, cursorMode);
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        inSettingsMenu = false;
    }

    public void Resume()
    {
        Cursor.SetCursor(cursorCamTexture, hotspot, cursorMode);
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        inSettingsMenu = false;
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        inSettingsMenu = true;
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        inSettingsMenu = false;
    }

    public void AudioSubMenu()
    {
        DisableSubMenus();
        audioSubMenu.SetActive(true);
    }
    public void ControlsSubMenu()
    {
        DisableSubMenus();
        controlsSubMenu.SetActive(true);
    }
    private void DisableSubMenus()
    {
        audioSubMenu.SetActive(false);
        controlsSubMenu.SetActive(false);
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
        if (playerController != null && !playerController.isDead)
        {
            playerController.Die();
        }
    }

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }


}
