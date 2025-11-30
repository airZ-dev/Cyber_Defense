using UnityEngine;
using UnityEngine.UI;

public class TowerSelectionUI : MonoBehaviour
{
    public GameObject selectionPanel;
    public Button basicTowerButton;

    private Vector3Int targetPlotPosition;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        selectionPanel.SetActive(false);

        if (basicTowerButton != null)
            basicTowerButton.onClick.AddListener(() => OnTowerSelected(0));
    }

    void Update()
    {
        if (selectionPanel.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                selectionPanel.GetComponent<RectTransform>(),
                Input.mousePosition,
                null))
            {
                HideSelectionPanel();
            }
        }
    }

    public void ShowSelectionPanel(Vector3Int plotPos)
    {
        targetPlotPosition = plotPos;

        Vector3 worldPos = GetCorrectPlotWorldPosition(plotPos);
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        selectionPanel.transform.position = screenPos + new Vector3(120, 0, 0);
        selectionPanel.SetActive(true);
    }

    public void HideSelectionPanel()
    {
        selectionPanel.SetActive(false);
    }

    void OnTowerSelected(int towerIndex)
    {
        Debug.Log($"Выбрана башня с индексом: {towerIndex}");

        BuildManager.Instance.setSelectedTower(towerIndex);

        BuildTower(towerIndex);

        HideSelectionPanel();
    }

    void BuildTower(int towerIndex)
    {
        Tower selectedTower = BuildManager.Instance.getSelectedTower();
        if (selectedTower == null)
        {
            return;
        }

        Vector3 worldPos = GetCorrectPlotWorldPosition(targetPlotPosition);

        GameObject tower = Instantiate(selectedTower.prefab, worldPos, Quaternion.identity);
    }

    Vector3 GetCorrectPlotWorldPosition(Vector3Int plotPos)
    {
        Plot plot = FindFirstObjectByType<Plot>();
        if (plot != null && plot.tilemap != null)
        {
            Vector3 worldPos = plot.tilemap.GetCellCenterWorld(plotPos);
            return worldPos;
        }

        return new Vector3(plotPos.x + 0.5f, plotPos.y + 0.5f, 0);
    }
}