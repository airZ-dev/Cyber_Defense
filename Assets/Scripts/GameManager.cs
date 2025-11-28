using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int fps = 60;


    private void Awake()
    {
        Application.targetFrameRate = fps;
    }
}
