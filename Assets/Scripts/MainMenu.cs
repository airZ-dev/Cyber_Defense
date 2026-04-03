using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private GameObject[] levelLockIcons;
    [SerializeField] private int[] levelBuildIndexes;

    private int maxUnlockedLevel = 1;

    private void Start()
    {
        maxUnlockedLevel = SaveSystem.LoadProgress();

        UpdateLevelButtonsUI();
    }

    private void UpdateLevelButtonsUI()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;

            if (levelNumber <= maxUnlockedLevel)
            {
                levelButtons[i].interactable = true;
                if (levelLockIcons != null && i < levelLockIcons.Length && levelLockIcons[i] != null)
                    levelLockIcons[i].SetActive(false);
            }
            else
            {
                levelButtons[i].interactable = false;
                if (levelLockIcons != null && i < levelLockIcons.Length && levelLockIcons[i] != null)
                    levelLockIcons[i].SetActive(true);
            }
        }
    }

    public void PlayLevel(int level)
    {
        if (level <= maxUnlockedLevel)
            SceneManager.LoadSceneAsync(levelBuildIndexes[level]);
    }

    public void ResetProgress()
    {
        SaveSystem.ResetProgress();
        maxUnlockedLevel = 1;
        UpdateLevelButtonsUI();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
