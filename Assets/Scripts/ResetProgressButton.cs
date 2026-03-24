using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ResetProgressButton : MonoBehaviour
{
    [SerializeField] private Button resetButton;
    [SerializeField] private MainMenu mainMenu;

    private void Start()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetProgress);
        }
    }

    private void ResetProgress()
    {
        SaveSystem.ResetProgress();
        if (mainMenu != null)
            mainMenu.ResetProgress();
    }
}