using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance {  get; private set; }

    [Header("Regerences")]
    [SerializeField] private Tower[] towers;
    //[SerializeField] private GameObject[] TowerPrefabs;
    private int selectedTower = -1;

    private void Awake()
    {
        Instance = this;
    }

    public Tower getSelectedTower()
    {
        if (selectedTower == -1)
            return null;
        return towers[selectedTower];
    }

    public void setSelectedTower(int _selectedTower)
    {
        if (_selectedTower < 0 || towers.Length <= _selectedTower)
        {
            Debug.Log("error1");
            return;
        }
        selectedTower = _selectedTower;
    }
    

    

}
