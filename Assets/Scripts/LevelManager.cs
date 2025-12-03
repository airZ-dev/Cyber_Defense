using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    public Transform startPoint;
    public Transform[] path;

    public bool isStop;
    private int currentHP;

    [SerializeField] public int currency;
    [SerializeField] private int baseHP;
    [SerializeField] private Image _hp;

    public int CurrentHP { get { return currentHP; } }

    private void Awake()
    {
        instance = this;
        currentHP= baseHP;
    }

    private void Start()
    {
        if(currency == 0)
        {
            currency = 48*2;
        }
        isStop = true;
    }
    public void takeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        //_hp.fillAmount = currentHP * 1.0f / baseHP;
        if(currentHP == 0)
        {
            DEATH();
            return;
        }
    }
    private void DEATH()
    {
        if (WinOrLossMenu.instance != null)
            WinOrLossMenu.instance.winOrLoseWindowShow(false);
    }

    public void WIN()
    {
        if (WinOrLossMenu.instance != null)
            WinOrLossMenu.instance.winOrLoseWindowShow(true);
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }
        else
        {
            Debug.Log("No Money");
            return false;
        }
    }
}
