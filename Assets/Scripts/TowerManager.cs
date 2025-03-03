using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerManager : MonoBehaviour
{
    // A List so I could remove all towers easier
    private List<Tower> towers;

    // Variables set with inspector
    [SerializeField] private List<TowerData> towerDatas;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Field grid;
    [SerializeField] private TMP_Text purchaseCost;
    [SerializeField] private TMP_Text selectedTower;

    public GameObject indicator; // Range indicator object
    public TowerType SelectedType { get; private set; }

    public void Awake()
    {
        towers = new();

        // Default the tower type selection to the basic tower
        Select(TowerType.Basic);
    }

    // Selects which tower to place down
    public void Select(TowerType type)
    {
        TowerData data = GetDataFromType(type);

        purchaseCost.SetText($"${data.cost}");
        selectedTower.SetText(type.ToString());

        SelectedType = type;

        // Visualises what the range would look like
        indicator.transform.localScale = new(
            data.range * 2,
            data.range * 2
        );
    }

    // Purchases the tower if the player has sufficient cash, and places it at the selected tile
    public void Purchase()
    {
        TowerData data = GetDataFromType(SelectedType);
        Base endBase = grid.EndBase;

        if (endBase.Cash - data.cost < 0)
            return;

        endBase.AddCash(-data.cost);

        // Creates a tower object and places it at the tile
        GameObject towerObject = Instantiate(towerPrefab);
        Tower tower = towerObject.GetComponent<Tower>();
        tower.Initialise(data);

        grid.SelectedTile.PlaceTower(tower);
        towers.Add(tower);
        grid.SelectTile(grid.SelectedTile); // Deselect the current selected tile
    }

    public TowerData GetDataFromType(TowerType type)
    {
        // Returns the data, where its type matches the given type
        return towerDatas.Find(data => data.type == type);
    }

    public void ClearAllTowers()
    {
        // Iterate through all towers and destroys it
        foreach (Tower tower in towers) {
            Destroy(tower.gameObject);
        }

        // Destroying the towers don't remove it from the list, so we need to clear it too
        towers.Clear();
    }
}