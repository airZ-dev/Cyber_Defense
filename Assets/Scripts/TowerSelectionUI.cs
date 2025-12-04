using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class TowerSelectionUI : MonoBehaviour
{
    public GameObject selectionPanel;
    public Button basicTowerButton;
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

    [Header("References")]
    [SerializeField] private TextMeshProUGUI[] updatePanelTexts;
    void Start()
    {
        mainCamera = Camera.main;
        selectionPanel.SetActive(false);
        updatePanel.SetActive(false);
        if (basicTowerButton != null)
            basicTowerButton.onClick.AddListener(() => OnTowerSelected(0));
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
            if (tw.GetComponent<basic_turret>() != null)
            {
                tw.GetComponent<basic_turret>().HideRange();
            }
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
        dmg += 1;
        spd -= 0.01f;
        currLvl += 1;
        tw.GetComponent<Tower>().currCost += 20;
        tw.GetComponent<Tower>().currentLevel = currLvl;
        basic_turret bt = tw.GetComponent<basic_turret>();
        if (bt != null)
        {
            range += 0.5f;
            bt.ChangeRange(range);
            bt.Damage = dmg;
            bt.SpeedOfSpawn = spd;
        }
        //для других переделать



    }

    private void OnGUI()
    {
        if (updatePanelTexts == null || updatePanelTexts.Length == 0)
        {
            return;
        }
        if (tw == null)
        {
            return;
        }
        updatePanelTexts[0].text = "Скорость - " + string.Format("{0:f2}", 1/spd) + " в сек.";
        updatePanelTexts[1].text = "Радиус - " + range;
        updatePanelTexts[2].text = "Урон - " + dmg;
        updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + "цена: " + tw.GetComponent<Tower>().currCost;
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