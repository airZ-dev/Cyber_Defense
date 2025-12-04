using UnityEngine;

public class CameraZoomMovement : MonoBehaviour
{

    [Header("Настройки зума")]
    public float zoomSpeed = 2.0f;
    public float minZoom = 2.0f;
    public float maxZoom = 10.0f;
    public float initialZoom = 5.0f;



    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = initialZoom;
        }
    }

    void Update()
    {
        if (LevelManager.instance.isActive)
        {
            HandleZoom();
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0 && cam != null)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }
}