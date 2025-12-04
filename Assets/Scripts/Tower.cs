using UnityEngine;
using System;

[Serializable]
public class Tower : MonoBehaviour
{
    //сделать его род классом с вирт методами для остальных турелей
    [Header("Settings")]
    [SerializeField] private int MaxLevel = 10;
    [SerializeField] private int CostOfUpg = 30;
    [SerializeField] private int CostOfBuy = 45;

    public GameObject prefab;

    public int currentLevel = 1;
    public int maxLvl { get { return MaxLevel; } }
    public int currCost { get { return CostOfUpg; } set { if (value > CostOfUpg) { CostOfUpg = value; } } }
    public int butCost { get { return CostOfBuy; } }

    public bool isPosToUpgrade(int cntlevel)
    {
        return cntlevel <= MaxLevel;
    }

    public Tower(GameObject gm)
    {
        prefab = gm;
    }
}
