using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class TowerSelectionUI : MonoBehaviour
{
    public GameObject selectionPanel;
    public Button[] TowerButtons;
    public GameObject updatePanel;
    public Button updateButton;
    public Button[] exitBtn;

    private Vector3Int targetPlotPosition;
    private Camera mainCamera;

    private int dmg;
    private float spd;
    private float range;
    private int currLvl;
    private GameObject tw;
    private float freezeFactor;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI[] updatePanelTexts;
    void Start()
    {
        mainCamera = Camera.main;
        selectionPanel.SetActive(false);
        updatePanel.SetActive(false);
        if (TowerButtons != null)
        {
            TowerButtons[0].onClick.AddListener(() => OnTowerSelected(0));
            TowerButtons[1].onClick.AddListener(() => OnTowerSelected(1));

        }
        if (updateButton != null)
            updateButton.onClick.AddListener(() => OnClickButtonUpdate());
        foreach(var x in exitBtn)
            if (x != null)
                x.onClick.AddListener(() => HideUpdatewPanel());
    }

    /*void Update()
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
    }*/

    public void ShowSelectionPanel(Vector3Int plotPos)
    {
        HideUpdatewPanel();
        targetPlotPosition = plotPos;
        selectionPanel.SetActive(true);
        //Debug.Log(selectionPanel.activeSelf);
    }

    public void HideSelectionPanel()
    {
        selectionPanel.SetActive(false);
    }
    public void HideUpdatewPanel()
    {

        updatePanel.SetActive(false);
        foreach (var t in updatePanelTexts)
            t.text = "";
        dmg = 0;
        spd = 0f;
        range = 0f;
        currLvl = 0;
        ///реализовать для всех башен
        if (tw != null)
        {
            basic_turret bt = tw.GetComponent<basic_turret>();
            if (bt != null) bt.HideRange();

            FreezeTurret ft = tw.GetComponent<FreezeTurret>();
            if (ft != null) ft.HideRange();
        }


    }
    public void ShowUpdatePanel(Vector3Int cellPos)
    {
        HideSelectionPanel();
        tw = BuildManager.Instance.dict[cellPos];
        currLvl = tw.GetComponent<Tower>().currentLevel;
        basic_turret bt = tw.GetComponent<basic_turret>();
        if (bt != null)
        {
            dmg = bt.Damage;
            spd = bt.SpeedOfSpawn;
            range = bt.Range;
        }

        FreezeTurret ft = tw.GetComponent<FreezeTurret>();
        if (ft!=null)
        {
            dmg = ft.Damage;          // не используется, но оставляем для совместимости
            spd = ft.SpeedOfSpawn;    // не используется
            range = ft.Range;
            freezeFactor = ft.FreezeFactor;
        }
        //реализовать для других турелей
        // else { }

        updatePanel.SetActive(true);
    }

    public void OnClickButtonUpdate()
    {
        if (LevelManager.instance.currency < tw.GetComponent<Tower>().currCost)
        {

            return;
        }
        if (!tw.GetComponent<Tower>().isPosToUpgrade(currLvl + 1))
        {
            return;
        }
        LevelManager.instance.currency -= tw.GetComponent<Tower>().currCost;

        // Увеличение характеристик в зависимости от типа
        basic_turret bt = tw.GetComponent<basic_turret>();
        if (bt != null)
        {
            dmg += 1;
            spd -= 0.01f; // скорострельность увеличивается
            range += 0.5f;
            currLvl += 1;
            tw.GetComponent<Tower>().currCost += 20;
            tw.GetComponent<Tower>().currentLevel = currLvl;

            bt.Damage = dmg;
            bt.SpeedOfSpawn = spd;
            bt.ChangeRange(range);
        }

        FreezeTurret ft = tw.GetComponent<FreezeTurret>();
        if (ft != null)
        {
            // Для замораживающей турели улучшаем радиус и силу заморозки
            range += 0.5f;
            freezeFactor = Mathf.Max(0, freezeFactor - 0.1f); // приближаем к 0 (сильнее мороз)
            currLvl += 1;
            tw.GetComponent<Tower>().currCost += 20;
            tw.GetComponent<Tower>().currentLevel = currLvl;

            ft.ChangeRange(range);
            ft.FreezeFactor = freezeFactor;
            // damage и speedOfSpawn не используются
        }
    }

    private void OnGUI()
    {
        if (updatePanelTexts == null || updatePanelTexts.Length == 0)
        {
            return;
        }
        foreach (var t in updatePanelTexts)
            t.text = "";
        if (tw?.GetComponent<basic_turret>() != null)
        {
            updatePanelTexts[0].text = "Скорость - " + string.Format("{0:f2}", 1 / spd) + " в сек.";
            updatePanelTexts[1].text = "Радиус - " + range;
            updatePanelTexts[2].text = "Урон - " + dmg;
            updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + "цена: " + tw.GetComponent<Tower>().currCost;
        }

        if (tw?.GetComponent<FreezeTurret>() != null)
        {
            updatePanelTexts[0].text = "Скорость - " + string.Format("{0:f2}", 1 / spd) + " в сек."; // если нужно, можно хранить rotationSpeed
            updatePanelTexts[1].text = "Радиус - " + range;
            updatePanelTexts[2].text = "Заморозка - " + ((1 - freezeFactor) * 100) + "%"; // чем меньше freezeFactor, тем сильнее эффект
            updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + "цена: " + tw.GetComponent<Tower>().currCost;
        }
    }

    void OnTowerSelected(int towerIndex)
    {
        //Debug.Log($"Выбрана башня с индексом: {towerIndex}");
        BuildManager.Instance.setSelectedTower(towerIndex);

        if (LevelManager.instance.currency >= BuildManager.Instance.getSelectedTower().butCost)
        {
            BuildTower(towerIndex);
            LevelManager.instance.currency -= BuildManager.Instance.getSelectedTower().butCost;
            HideSelectionPanel();
        }
        else
        {
            BuildManager.Instance.setSelectedTower(-1);
        }
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
        BuildManager.Instance.dict[targetPlotPosition] = tower;
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