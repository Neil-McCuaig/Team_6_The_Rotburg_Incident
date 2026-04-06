using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    public bool isPaused = false;
    private bool inSettingsMenu = false;

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject quitCheck;
    public GameObject menuCheck;

    [Header("Settings Sub-Menus")]
    public GameObject videoSubMenu;
    public GameObject audioSubMenu;
    public GameObject controlsSubMenu;

    [HideInInspector][Header("References")]
    PlayerController playerController;
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [Header("Cursor Settings")]
    public Texture2D cursorCamTexture;
    public Texture2D cursorFingerTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("Event System GameObjects")]
    public GameObject pausedFirstButton;
    public GameObject settingsFirstButton;
    public GameObject settingsClosedButton;
    public GameObject videoSubMenuFirstButton;
    public GameObject audioSubMenuFirstButton;
    public GameObject audioSubMenuClosedButton;
    public GameObject menuQuitCheckFirstButton;
    public GameObject menuQuitCheckClosedButton;
    public GameObject gameQuitCheckFirstButton;
    public GameObject gameQuitCheckClosedButton;

    private void Start()
    {
        Cursor.SetCursor(cursorCamTexture, hotspot, cursorMode);
        playerController = FindAnyObjectByType<PlayerController>();

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution (int  resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inSettingsMenu)
        {
            if (isPaused)
            {
                Resume();
                SoundManager.instance.PlaySound(SoundManager.instance.uiBack);

            }
            else
            {
                PauseGame();
                SoundManager.instance.PlaySound(SoundManager.instance.uiBack);
            }
        }
        if (inSettingsMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();
            SoundManager.instance.PlaySound(SoundManager.instance.uiBack);
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

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pausedFirstButton);
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
        DisableSubMenus();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsFirstButton);
        SoundManager.instance.PlaySound(SoundManager.instance.uiOk);
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        inSettingsMenu = false;
        DisableSubMenus();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsClosedButton);
        SoundManager.instance.PlaySound(SoundManager.instance.uiBack);
    }
    public void AudioSubMenu()
    {
        DisableSubMenus();
        audioSubMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(audioSubMenuFirstButton);
        SoundManager.instance.PlaySound(SoundManager.instance.uiOk);
    }
    public void CloseAudioSubMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(audioSubMenuClosedButton);
        SoundManager.instance.PlaySound(SoundManager.instance.uiBack);
    }
    public void VideoSubMenu()
    {
        DisableSubMenus();
        videoSubMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(videoSubMenuFirstButton);
        SoundManager.instance.PlaySound(SoundManager.instance.uiOk);
    }
    public void ControlsSubMenu()
    {
        DisableSubMenus();
        controlsSubMenu.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.instance.uiOk);
    }
    private void DisableSubMenus()
    {
        videoSubMenu.SetActive(false);
        audioSubMenu.SetActive(false);
        controlsSubMenu.SetActive(false);
    }
    public void SetFullScreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void OpenQuitCheck()
    {
        quitCheck.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameQuitCheckFirstButton);
    }
    public void CloseQuitCheck()
    {
        quitCheck.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameQuitCheckClosedButton);
    }
    public void OpenMenuCheck()
    {
        menuCheck.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuQuitCheckFirstButton);
        SoundManager.instance.PlaySound(SoundManager.instance.uiOk);
    }
    public void CloseMenuCheck()
    {
        menuCheck.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuQuitCheckClosedButton);
        SoundManager.instance.PlaySound(SoundManager.instance.uiBack);
    }
    public void QuitGame()
    {
        Application.Quit();
        SoundManager.instance.PlaySound(SoundManager.instance.uiOk);
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        SoundManager.instance.PlaySound(SoundManager.instance.uiOk);
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

    public void HoverMouse()
    {
        GameObject audioSubMenu;
        SoundManager.instance.PlaySound(SoundManager.instance.uiHover);
    }

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
