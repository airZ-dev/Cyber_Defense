using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class TowerSelectionUI : MonoBehaviour
{

    public static TowerSelectionUI instance { get; private set; }
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
    private float spread;
    private int cntPellet;
    private bool isNotFreeze;
    private ToggleGroup radios;
    private TargetStrategy targetStrategy;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI[] updatePanelTexts;
    [SerializeField] private Image imageUpdate;
    [SerializeField] private Sprite[] imagesRequire;
    [SerializeField] private GameObject radioField;

    void Start()
    {
        mainCamera = Camera.main;
        instance = this;
        selectionPanel.SetActive(false);
        updatePanel.SetActive(false);
        radioField.SetActive(false);
        if (TowerButtons != null)
        {
            if(TowerButtons.Length == 1)
            {
                TowerButtons[0].onClick.AddListener(() => OnTowerSelected(0));
            }else if (TowerButtons.Length == 2)
            {
                TowerButtons[0].onClick.AddListener(() => OnTowerSelected(0));
                TowerButtons[1].onClick.AddListener(() => OnTowerSelected(1));
            }
            else if (TowerButtons.Length == 3)
            {

                TowerButtons[0].onClick.AddListener(() => OnTowerSelected(0));
                TowerButtons[1].onClick.AddListener(() => OnTowerSelected(1));
                TowerButtons[2].onClick.AddListener(() => OnTowerSelected(2));
            }

        }
        if (updateButton != null)
            updateButton.onClick.AddListener(() => OnClickButtonUpdate());
        foreach (var x in exitBtn)
            if (x != null)
                x.onClick.AddListener(() => HideUpdatewPanel());
        radios = radioField.GetComponent<ToggleGroup>();
        
    }

    public void OnChanged(int i)
    {
        if (tw.GetComponent<Tower>().Strategy == (TargetStrategy)i)
            return;
        tw.GetComponent<Tower>().Strategy = (TargetStrategy) i;

        basic_turret bt = tw.GetComponent<basic_turret>();
        if (bt != null) bt.isChanged = true;

        ShotgunTurret st = tw.GetComponent<ShotgunTurret>();
        if (st != null) st.isChanged = true;
    }


    public void ShowSelectionPanel(Vector3Int plotPos)
    {
        HideUpdatewPanel();
        targetPlotPosition = plotPos;
        selectionPanel.SetActive(true);
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

            ShotgunTurret st = tw.GetComponent<ShotgunTurret>();
            if (st != null) st.HideRange();
        }
        radioField.SetActive(false);

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
            targetStrategy = bt.GetComponent<Tower>().Strategy;
        }

        isNotFreeze = true;
        FreezeTurret ft = tw.GetComponent<FreezeTurret>();
        if (ft != null)
        {
            dmg = ft.Damage;          // не используется, но оставляем для совместимости
            spd = ft.SpeedOfSpawn;    // не используется
            range = ft.Range;
            freezeFactor = ft.FreezeFactor;
            isNotFreeze = false;
        }

        ShotgunTurret st = tw.GetComponent<ShotgunTurret>();
        if (st != null)
        {
            cntPellet = st.CounrOfBullets;
            dmg = st.Damage;
            spread = st.Spread;
            range = st.Range;
            spd = st.SpeedOfSpawn;
            targetStrategy = st.GetComponent<Tower>().Strategy;
        }
        //реализовать для других турелей
        // else { }
        Tower x = tw?.GetComponent<Tower>();
        if (x != null && x.costSell == 0)
        {
            x.costSell = x.buyCost / 2;
        }
        if (isNotFreeze) {
            radioField.SetActive(true);
            int ind = (int)tw.GetComponent<Tower>().Strategy;
            Toggle[] tt = radioField.GetComponentsInChildren<Toggle>(true);
            for (int i = 0; i < tt.Length; i++)
            {
                tt[i].SetIsOnWithoutNotify(false);
                if(i == ind)
                {
                    tt[i].SetIsOnWithoutNotify(true);
                }
            }
        }

        updatePanel.SetActive(true);
    }

    public void OnClickButtonUpdate()
    {
        if (LevelManager.instance.currency < tw.GetComponent<Tower>().currCost)
        {
            AudioManager.Instance?.ErrorSound();
            return;
        }
        if (!tw.GetComponent<Tower>().isPosToUpgrade(currLvl + 1))
        {
            AudioManager.Instance?.ErrorSound();
            return;
        }
        LevelManager.instance.currency -= tw.GetComponent<Tower>().currCost;
        tw.GetComponent<Tower>().costSell += tw.GetComponent<Tower>().currCost / 2;
        // Увеличение характеристик в зависимости от типа
        basic_turret bt = tw.GetComponent<basic_turret>();
        if (bt != null)
        {
            dmg += 3;
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
            freezeFactor -= 0.04f; //увеличиваем уровень заморозки на 4% при прокачке
            currLvl += 1;
            tw.GetComponent<Tower>().currCost += 20;
            tw.GetComponent<Tower>().currentLevel = currLvl;

            ft.ChangeRange(range);
            ft.FreezeFactor = freezeFactor;
            // damage и speedOfSpawn не используются
        }

        ShotgunTurret st = tw.GetComponent<ShotgunTurret>();
        if (st != null)
        {
            range += 0.5f;
            spread -= spread * 0.1f;
            cntPellet++;
            currLvl++;
            tw.GetComponent<Tower>().currCost += 20;
            tw.GetComponent<Tower>().currentLevel = currLvl;

            st.ChangeRange(range);
            st.CounrOfBullets = cntPellet;
            st.Damage = dmg;
            st.Spread = spread;
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

        if (tw == null) return;
        if (tw?.GetComponent<basic_turret>() != null)
        {
            if (imagesRequire != null)
                imageUpdate.GetComponent<Image>().sprite = imagesRequire[0];
            updatePanelTexts[0].text = "Скорость - " + string.Format("{0:f2}", 1 / spd) + " в сек.";
            updatePanelTexts[1].text = "Радиус - " + range;
            updatePanelTexts[2].text = "Урон - " + dmg;
            if (tw?.GetComponent<Tower>().maxLvl ==  currLvl)
                updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + " - ";
            else
                updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + "цена: " + tw.GetComponent<Tower>().currCost;
        }

        if (tw?.GetComponent<FreezeTurret>() != null)
        {
            if (imagesRequire != null)
                imageUpdate.GetComponent<Image>().sprite = imagesRequire[1];
            updatePanelTexts[0].text = "Скорость - " + string.Format("{0:f2}", 1 / spd) + " в сек."; // если нужно, можно хранить rotationSpeed
            updatePanelTexts[1].text = "Радиус - " + range;
            updatePanelTexts[2].text = $"Заморозка - {((1 - freezeFactor) * 100):F0}%"; // чем меньше freezeFactor, тем сильнее эффект
            if (tw?.GetComponent<Tower>().maxLvl == currLvl)
                updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + " - ";
            else
                updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + "цена: " + tw.GetComponent<Tower>().currCost;
        }


        if (tw?.GetComponent<ShotgunTurret>() != null)
        {
            if (imagesRequire != null)
                imageUpdate.GetComponent<Image>().sprite = imagesRequire[2];
            updatePanelTexts[0].text = "Скорость - " + string.Format("{0:f2}", 1 / spd) + " в сек.";
            updatePanelTexts[1].text = "Радиус - " + range;
            updatePanelTexts[2].text = "Урон - " + dmg + "\nТочность - " + spread + "\nКоличество - " + cntPellet;
            if (tw?.GetComponent<Tower>().maxLvl == currLvl)
                updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + " - ";
            else
                updatePanelTexts[3].text = currLvl + "/" + tw.GetComponent<Tower>().maxLvl + " уровень\n" + "цена: " + tw.GetComponent<Tower>().currCost;
        }
        SellMenu.instance?.setTower(tw);
    }

    void OnTowerSelected(int towerIndex)
    {
        //Debug.Log($"Выбрана башня с индексом: {towerIndex}");
        BuildManager.Instance.setSelectedTower(towerIndex);

        if (LevelManager.instance.currency >= BuildManager.Instance.getSelectedTower().buyCost)
        {
            BuildTower(towerIndex);
            LevelManager.instance.currency -= BuildManager.Instance.getSelectedTower().buyCost;
            HideSelectionPanel();
        }
        else
        {
            AudioManager.Instance?.ErrorSound();
            //BuildManager.Instance.setSelectedTower(-1);
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
        AudioManager.Instance?.PlayBuildTower();
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