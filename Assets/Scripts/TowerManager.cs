using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerManager : MonoBehaviour
{
    private Dictionary<Tower, Tile> occupation;

    [SerializeField] private List<TowerData> towerDatas;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Field grid;
    [SerializeField] private TMP_Text purchaseCost;
    [SerializeField] private TMP_Text selectedTower;

    private TowerType? selectedType;

    public void Awake()
    {
        occupation = new();
    }

    public void Select(TowerType type)
    {
        TowerData data = GetDataFromType(type);

        purchaseCost.SetText($"${data.cost}");
        selectedTower.SetText(type.ToString());

        selectedType = type;
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

        tower.type = type;
        tower.spriteRenderer.color = data.color;

        grid.SelectedTile.PlaceTower(tower);
        occupation.Add(tower, grid.SelectedTile);
    }

    public TowerData GetDataFromType(TowerType type)
    {
        return towerDatas.Find(data => data.type == type);
    }
}
