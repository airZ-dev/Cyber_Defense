using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button resumeButton;

    private bool isPaused = false;

    private void Start()
    {
        if (settingsButton != null)
            settingsButton.onClick.AddListener(TogglePauseMenu);

        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMainMenu);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (settingsPanel != null)
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
        isPaused = true;
        Time.timeScale = 0f;
        settingsPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        settingsPanel.SetActive(false);
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
