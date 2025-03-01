using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerManager : MonoBehaviour
{
    private List<Tower> towers;
    private TowerType? selectedType;

    [SerializeField] private List<TowerData> towerDatas;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Field grid;
    [SerializeField] private TMP_Text purchaseCost;
    [SerializeField] private TMP_Text selectedTower;

    public GameObject indicator;

    public void Awake()
    {
        towers = new();
    }

    public void Select(TowerType type)
    {
        TowerData data = GetDataFromType(type);

        purchaseCost.SetText($"${data.cost}");
        selectedTower.SetText(type.ToString());

        selectedType = type;

        indicator.transform.localScale = new(
            data.range * 2,
            data.range * 2
        );
    }

    public void Purchase()
    {
        if (selectedType is not TowerType type)
            return;

        TowerData data = GetDataFromType(type);
        Base endBase = grid.EndBase;

        if (endBase.Cash - data.cost < 0)
            return;

        endBase.AddCash(-data.cost);

        GameObject towerObject = Instantiate(towerPrefab);
        Tower tower = towerObject.GetComponent<Tower>();
        tower.Initialise(data);

        grid.SelectedTile.PlaceTower(tower);
        towers.Add(tower);
        grid.SelectTile(grid.SelectedTile);
    }

    public TowerData GetDataFromType(TowerType type)
    {
        return towerDatas.Find(data => data.type == type);
    }

    public void ClearAllTowers()
    {
        foreach (Tower tower in towers) {
            Destroy(tower.gameObject);
        }

        Debug.Log(towers.Count);

        towers.Clear();
    }
}