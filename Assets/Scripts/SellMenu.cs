using System.Reflection.Emit;
using TMPro;
using UnityEngine;

public class SellMenu : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private GameObject sellPanel;
    [SerializeField] private TextMeshProUGUI lbl;

    public static SellMenu instance { get; private set; }
    private GameObject towerToSell;
    private void Awake()
    {
        instance = this;
        sellPanel.SetActive(false);
    }

    public void setTower(GameObject g)
    {
        towerToSell = g;
    }

    public void ShowSellMenu()
    {
        Time.timeScale = 0.0f;
        AudioManager.Instance?.PlayDefaultButtonClick();
        LevelManager.instance.isActive = false;
        if (towerToSell?.GetComponent<Tower>() != null)
        {
            lbl.text = $"Продажа за {towerToSell.GetComponent<Tower>().costSell}";
        }
        sellPanel.SetActive(true);

    }

    public void OnYesButton()
    {
        AudioManager.Instance?.PlayCurrency();
        LevelManager lm = LevelManager.instance;
        lm.currency = lm.currency + towerToSell.GetComponent<Tower>().costSell;
        Time.timeScale = 1.0f;
        TowerSelectionUI.instance?.HideUpdatewPanel();
        Plot.instance?.RemoveTower(towerToSell);
        if (towerToSell != null)
        {
            if (towerToSell.GetComponent<FreezeTurret>() != null)
                towerToSell.GetComponent<FreezeTurret>().OnDestroyThis();
            else
                Destroy(towerToSell);
        }

        sellPanel.SetActive(false);
        LevelManager.instance.isActive = true;

    }
    public void OnNoButton()
    {
        AudioManager.Instance?.PlayBackButtonClick();
        Time.timeScale = 1.0f;
        sellPanel.SetActive(false);
        LevelManager.instance.isActive = true;
    }

}
