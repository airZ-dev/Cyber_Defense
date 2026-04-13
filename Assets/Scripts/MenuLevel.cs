using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button backButtonMenu;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button resumeButton;

    private bool isPaused = false;

    private void Start()
    {
        if (menuButton != null)
            menuButton.onClick.AddListener(TogglePauseMenu);

        if (backButton != null)
            backButton.onClick.AddListener(GoToMainMenu);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if(backButtonMenu != null)
            backButtonMenu.onClick.AddListener(QuickSettings);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void QuickSettings()
    {
       settingsPanel.SetActive(false);
    }

    public void TogglePauseMenu()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        LevelManager.instance.isActive = false;
        Time.timeScale = 0f;
        menuPanel.SetActive(true);
        AudioManager.Instance?.PauseMusic(true);
    }

    public void ResumeGame()
    {
        LevelManager.instance.isActive = true;
        Time.timeScale = 1f;
        menuPanel.SetActive(false);
        AudioManager.Instance?.PauseMusic(false);
    }

    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }


    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }
}
