using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int fps = 60;


    private void Awake()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = fps;
    }
}
