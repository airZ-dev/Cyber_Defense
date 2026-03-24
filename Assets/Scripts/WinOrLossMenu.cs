using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLossMenu : MonoBehaviour
{
    public static WinOrLossMenu instance;

    public GameObject winPanel;
    public GameObject losePanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void winOrLoseWindowShow(bool isWin)
    {
        Time.timeScale = 0f;
        LevelManager.instance.isActive = false;
        if (isWin)
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            int maxUnlocked = SaveSystem.LoadProgress();

            if (currentLevel >= maxUnlocked)
            {
                SaveSystem.SaveProgress(currentLevel + 1);
            }

            winPanel.SetActive(true);
            losePanel.SetActive(false);
        }
        else
        {
            winPanel.SetActive(false);
            losePanel.SetActive(true);
        }
    }

    public void GoToMainMenu() => SceneManager.LoadScene(0);
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        int maxUnlocked = SaveSystem.LoadProgress();

        if (nextSceneIndex <= maxUnlocked && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }

        else if (nextSceneIndex > maxUnlocked)
        {
            Debug.Log("—ледующий уровень еще не разблокирован!");
            SceneManager.LoadScene(0);
        }

        else
        {
            Debug.Log("Ёто был последний уровень! ¬озврат в меню.");
            SceneManager.LoadScene(0);
        }
    }
}
