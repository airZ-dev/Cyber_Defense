using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    public Transform startPoint;
    public Transform[] path;

    public bool isActive;
    private int currentHP;

    [SerializeField] public int currency;
    [SerializeField] private int baseHP;
    [SerializeField] private TextMeshProUGUI _hps;
    [SerializeField] private TextMeshProUGUI _cur;
    [SerializeField] private TextMeshProUGUI _wav;

    private void OnGUI()
    {
        if (_hps != null)
            _hps.text = $"{currentHP}";
        if (_cur != null)
            _cur.text = $"{currency}";
        if (_wav != null)
        {
            _wav.text = $"{WaveManager.instance.current_wave + 1}|{WaveManager.instance.max_wave}";

        }
    }

    public int CurrentHP { get { return currentHP; } }

    private void Awake()
    {
        instance = this;
        currentHP = baseHP;
    }

    private void Start()
    {
        if (currency == 0)
        {
            Console.WriteLine("error: currency is zero!");
        }
        isActive = true;
    }
    public void takeDamage(int amount)
    {
        AudioManager.Instance?.PlayHitBase();
        currentHP -= amount;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        //_hp.fillAmount = currentHP * 1.0f / baseHP;
        if (currentHP == 0)
        {
            DEATH();
            return;
        }
    }
    private void DEATH()
    {
        if (WinOrLossMenu.instance != null)
        {
            AudioManager.Instance?.PlayGameOver();
            AudioManager.Instance?.PauseMusic(true);
            WinOrLossMenu.instance.winOrLoseWindowShow(false);
        }

    }

    public void WIN()
    {
        if (WinOrLossMenu.instance != null)
        {
            AudioManager.Instance?.PlayVictory();
            AudioManager.Instance?.PauseMusic(true);
            WinOrLossMenu.instance.winOrLoseWindowShow(true);
        }
        
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
        AudioManager.Instance?.PlayCurrency();
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
            //Debug.Log("No Money");
            return false;
        }
    }
}
