using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerManager : MonoBehaviour
{
    private List<Tower> towers;

    [SerializeField] private List<TowerData> towerDatas;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Field grid;
    [SerializeField] private TMP_Text purchaseCost;
    [SerializeField] private TMP_Text selectedTower;

    public GameObject indicator;
    public TowerType SelectedType { get; private set; }

    public void Awake()
    {
        towers = new();

        Select(TowerType.Basic);
    }

    public void Select(TowerType type)
    {
        TowerData data = GetDataFromType(type);

        purchaseCost.SetText($"${data.cost}");
        selectedTower.SetText(type.ToString());

        SelectedType = type;

        indicator.transform.localScale = new(
            data.range * 2,
            data.range * 2
        );
    }

    public void Purchase()
    {
        TowerData data = GetDataFromType(SelectedType);
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

        towers.Clear();
    }
}