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

        if (isWin)
        {
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
    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
