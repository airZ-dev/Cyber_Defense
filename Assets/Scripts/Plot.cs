using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class Plot : MonoBehaviour
{
    [Header("Настройки")]
    public Color hoverColor = Color.yellow;
    public Color clickColor = Color.red;
    public bool enableTileSelection = true;

    [Header("Ссылки")]
    public Tilemap tilemap;

    [Header("Игнорируемые UI теги")]
    public string[] ignoreUITags = { };

    private Vector3Int lastHoveredCell;
    private Camera mainCamera;
    private Dictionary<Vector3Int, GameObject> towers;
    // События для внешних скриптов
    public Action<Vector3Int> OnTileHovered;
    public Action<Vector3Int> OnTileClicked;

    void Start()
    {
        mainCamera = Camera.main;
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }
        towers = BuildManager.Instance.dict;
    }

    void Update()
    {
        if (!enableTileSelection)
            return;
        // Проверяем не над UI ли курсор
        if (IsPointerOverUIElement())
            return;

        HandleTileSelection();
    }

    // Расширенная проверка UI элементов
    bool IsPointerOverUIElement()
    {
        // Базовая проверка EventSystem
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Дополнительная проверка через Raycast
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Проверяем все результаты Raycast
            foreach (RaycastResult result in results)
            {
                // Игнорируем элементы с определенными тегами
                if (result.gameObject != null)
                {
                    bool shouldIgnore = false;
                    foreach (string tag in ignoreUITags)
                    {
                        if (result.gameObject.CompareTag(tag))
                        {
                            shouldIgnore = true;
                            break;
                        }
                    }

                    if (!shouldIgnore)
                        return true;
                }
            }
        }

        return false;
    }

    void HandleTileSelection()
    {
        if (!IsMouseInViewport())
            return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        if (mouseWorldPos == Vector3.zero)
            return;

        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
        towers = BuildManager.Instance.dict;
        if (tilemap.HasTile(cellPos))
        {
            if (cellPos != lastHoveredCell)
            {
                //OnTileHover(cellPos);
                lastHoveredCell = cellPos;
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnTileClick(cellPos);
            }
        }
        else
        {
            if (tilemap.HasTile(lastHoveredCell))
            {
                if (towers.ContainsKey(lastHoveredCell)) towers[lastHoveredCell].GetComponent<basic_turret>().HideRange();
                ResetTileColor(lastHoveredCell);
                lastHoveredCell = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            }
        }
    }

    bool IsMouseInViewport()
    {
        Vector3 mousePos = Input.mousePosition;
        return mousePos.x >= 0 && mousePos.x <= Screen.width &&
               mousePos.y >= 0 && mousePos.y <= Screen.height;
    }

    Vector3 GetMouseWorldPosition()
    {
        if (!IsMouseInViewport())
            return Vector3.zero;

        try
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            return worldPos;
        }
        catch (System.Exception)
        {
            return Vector3.zero;
        }
    }

    /*void OnTileHover(Vector3Int cellPos)
    {
        if (tilemap.HasTile(lastHoveredCell))
        {
            ResetTileColor(lastHoveredCell);
        }

        tilemap.SetColor(cellPos, hoverColor);
        OnTileHovered?.Invoke(cellPos);
    }*/

    void OnTileClick(Vector3Int cellPos)
    {
        /*if (!WaveManager.instance.IsGameStarted)
        {
            Debug.Log("Сначала начните волну!");
            return;
        }*/

        if (towers.ContainsKey(cellPos))
        {
            TowerSelectionUI UpdateUI = FindFirstObjectByType<TowerSelectionUI>();
            if (UpdateUI != null)
            {
                towers[cellPos].GetComponent<basic_turret>().ShowRange();
                Debug.Log("Меню апдейта");
                //selectionUI.ShowUpdatePanel(cellPos);
                return;
            }
        }
        
        TowerSelectionUI selectionUI = FindFirstObjectByType<TowerSelectionUI>();
        if (selectionUI != null)
        {
            selectionUI.ShowSelectionPanel(cellPos);

        }
        else
        {
            Debug.LogError("TowerSelectionUI не найден на сцене!");
        }
    }

    void ResetTileColor(Vector3Int cellPos)
    {
        tilemap.SetColor(cellPos, Color.white);
    }

    // Публичные методы для управления
    public void EnableTileSelection() => enableTileSelection = true;
    public void DisableTileSelection() => enableTileSelection = false;
    public void ResetAllTileColors() => tilemap.color = Color.white;
    public Vector3Int GetCurrentHoveredTile() => lastHoveredCell;
    public bool IsOverTile() => tilemap.HasTile(lastHoveredCell);
}





/* [Header("Настройки")]
 public Color hoverColor = Color.yellow;
 public Color clickColor = Color.red;

 [Header("Ссылки")]
 public Tilemap tilemap;

 private Vector3Int lastHoveredCell;
 private Camera mainCamera;

 void Start()
 {
     mainCamera = Camera.main;

     if (tilemap == null)
     {
         tilemap = GetComponent<Tilemap>();
     }
 }

 void Update()
 {
     HandleTileSelection();
 }

 void HandleTileSelection()
 {
     // Получаем позицию мыши в мировых координатах
     Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
     mouseWorldPos.z = 0;

     // Конвертируем в позицию ячейки тайлмапа
     Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

     // Проверяем есть ли тайл в этой ячейке
     if (tilemap.HasTile(cellPos))
     {
         // Наведение на тайл
         if (cellPos != lastHoveredCell)
         {
             OnTileHover(cellPos);
             lastHoveredCell = cellPos;
         }

         // Клик по тайлу
         if (Input.GetMouseButtonDown(0))
         {
             OnTileClick(cellPos);
         }
     }
     else
     {
         // Сброс при уходе с тайла
         if (tilemap.HasTile(lastHoveredCell))
         {
             ResetTileColor(lastHoveredCell);
             lastHoveredCell = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
         }
     }
 }

 void OnTileHover(Vector3Int cellPos)
 {
     Debug.Log($"Наведение на тайл: {cellPos}");

     // Восстанавливаем цвет предыдущего тайла
     if (tilemap.HasTile(lastHoveredCell))
     {
         ResetTileColor(lastHoveredCell);
     }

     // Подсвечиваем текущий тайл
     tilemap.SetColor(cellPos, hoverColor);
 }

 void OnTileClick(Vector3Int cellPos)
 {
     Debug.Log($"Клик по тайлу: {cellPos}");

     // Меняем цвет при клике
     tilemap.SetColor(cellPos, clickColor);

     // Получаем мировые координаты центра тайла
     Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
     Debug.Log($"Мировые координаты: {worldPos}");

     // Здесь можно добавить логику:
     // - Построить башню
     // - Выбрать тайл
     // - Активировать способность
 }

 void ResetTileColor(Vector3Int cellPos)
 {
     // Возвращаем оригинальный цвет
     tilemap.SetColor(cellPos, Color.white);
 }

 // Метод для сброса всех цветов (можно вызвать извне)
 public void ResetAllTileColors()
 {
     tilemap.color = Color.white;
 }

 // Метод для получения тайла под мышью
 public Vector3Int GetTileUnderMouse()
 {
     Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
     mouseWorldPos.z = 0;
     return tilemap.WorldToCell(mouseWorldPos);
 }

 // Метод для проверки есть ли тайл под мышью
 public bool HasTileUnderMouse()
 {
     return tilemap.HasTile(GetTileUnderMouse());
 }
}*/

/*[Header("settings")]
[SerializeField] private Color hoverColor;
[SerializeField] private float thickness = 0.1f;

[Header("References")]
[SerializeField] private Tilemap tm;
[SerializeField] private Material material;

private GameObject tower;
private Color basicColor;
private Camera maincamera;




private void Start()
{
    maincamera = Camera.main;
    tm = GetComponent<Tilemap>();
    ShaderHightlight();

}
private void ShaderHightlight()
{
    Shader shader = Shader.Find("Sprites/shaders/Ourline");
    if (shader == null)
    {
        Debug.Log("error3");
    }
    material = new Material(shader);
    material.SetColor("_OutlineColor", hoverColor);
    material.SetFloat("_OutlineThickness", thickness);
}

private void Update()
{
    HandleTileSelection();
}

void HandleTileSelection()
{
    Vector3 mousePos = maincamera.ScreenToWorldPoint(Input.mousePosition);
    Vector3Int cellPos = tm.WorldToCell(mousePos);
    if (tm.HasTile(cellPos))
    {
        if(cellPos != )
    }
}
private void OnMouseEnter()
{
    sr.color = hoverColor;
}

private void OnMouseExit()
{
    sr.color = basicColor;
}
private void OnMouseDown()
{
    if (LayerMask.GetMask() == 0)
    {
        Debug.Log(":sd");
    }
    if (tower != null)
    {
        ErrorAnimation();
        return;
    }
    Tower towerBuilder = BuildManager.Instance.getSelectedTower();
    if (towerBuilder == null)
    {
        ErrorAnimation();
        return;
    }
    if (towerBuilder.cost > LevelManager.instance.currency)
    {
        ErrorAnimation();
        return;
    }
    LevelManager.instance.SpendCurrency(towerBuilder.cost);
    tower = Instantiate(towerBuilder.prefab, new Vector3(transform.position.x, transform.position.y, 1), Quaternion.identity);
}
private void ErrorAnimation()
{
    sr.color = Color.red;
}*/

