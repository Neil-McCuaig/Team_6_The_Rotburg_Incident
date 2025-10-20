using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject audioSettingsUI;

    public bool isPaused = false;
    private bool inAudioSettings = false;
    PlayerController playerController;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
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
        pauseMenuUI.SetActive(true);
        audioSettingsUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        inAudioSettings = false;
    }

    public void Resume()
    {
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
}
