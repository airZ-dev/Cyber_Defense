using UnityEngine;
using System;


public enum TargetStrategy
{
    First = 0,  // самый старый (наибольший прогресс по пути)
    Closest = 1,   // ближайший по расстоянию
    Last = 2       // самый новый (наименьший прогресс)
}

[Serializable]
public class Tower : MonoBehaviour
{
    //сделать его род классом с вирт методами для остальных турелей
    [Header("Settings")]
    [SerializeField] private int MaxLevel = 10;
    [SerializeField] private int CostOfUpg = 30;
    [SerializeField] private int CostOfBuy = 45;
    [SerializeField] private TargetStrategy str = TargetStrategy.Closest;

    private int CostOfSell = 0;

    public GameObject prefab;

    public int currentLevel = 1;
    public int maxLvl { get { return MaxLevel; } }
    public int currCost { get { return CostOfUpg; } set { if (value > CostOfUpg) { CostOfUpg = value; } } }
    public int buyCost { get { return CostOfBuy; } }

    public int costSell { get { return CostOfSell; } set { if (value > 0) { CostOfSell = value; } } }

    public TargetStrategy Strategy { get { return str; } set { str = value; } }

    public bool isPosToUpgrade(int cntlevel)
    {
        return cntlevel <= MaxLevel;
    }

    public Tower(GameObject gm)
    {
        prefab = gm;
    }
}
