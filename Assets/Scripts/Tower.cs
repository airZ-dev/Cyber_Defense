using UnityEngine;
using System;

[Serializable]
public class Tower
{
    public string name;
    public int cost;
    public GameObject prefab;
    public float x_pos;
    public float y_pos;
    public Tower(string _name, int _cost, GameObject _prefab, float x = 0.0f, float y = 0.0f)
    {
        name = _name;
        cost = _cost;
        prefab = _prefab;
        x_pos = x;
        y_pos = y;
    }
}
