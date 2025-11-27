using UnityEngine;

public class CameraDragMovement : MonoBehaviour
{
    [Header("Настройки перемещения")]
    public float dragSpeed = 1.0f;
    public bool invertDrag = false;

    [Header("Ограничения перемещения перемещения")]
    public bool useBounds = false;
    public float minX, maxX, minY, maxY;


    private Vector3 dragOrigin;
    private Vector3 cameraStartPosition;
    private bool isDragging = false;

    void Update()
    {
        if (LevelManager.instance.isStop)
        {
            // Начало перемещения при нажатии ЛКМ
            if (Input.GetMouseButtonDown(0))
            {
                StartDrag();
            }

            // Перемещение камеры при удержании ЛКМ
            if (Input.GetMouseButton(0) && isDragging)
            {
                PerformDrag();
            }

            // Завершение перемещения при отпускании ЛКМ
            if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }
    }

    void LateUpdate()
    {
        if (useBounds)
        {
            // Ограничиваем позицию камеры в заданных границах
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
            transform.position = clampedPosition;
        }
    }

    void StartDrag()
    {
        isDragging = true;
        dragOrigin = Input.mousePosition;
        cameraStartPosition = transform.position;
    }

    void PerformDrag()
    {
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 difference = Camera.main.ScreenToWorldPoint(currentMousePos) -
                           Camera.main.ScreenToWorldPoint(dragOrigin);

        // Инвертируем направление если нужно
        if (invertDrag)
        {
            difference = -difference;
        }

        transform.position = cameraStartPosition - difference * dragSpeed;
    }

    void EndDrag()
    {
        isDragging = false;
    }
   
}