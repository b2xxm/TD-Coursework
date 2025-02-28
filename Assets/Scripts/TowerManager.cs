using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private List<TowerData> towerDatas;
    [SerializeField] private TMP_Text purchaseCost;
    [SerializeField] private TMP_Text selectedTower;

    public void Select(TowerType type)
    {
        TowerData data = GetDataFromType(type);

        purchaseCost.SetText($"${data.cost}");
        selectedTower.SetText(type.ToString());
    }

    public TowerData GetDataFromType(TowerType type)
    {
        return towerDatas.Find(data => data.type == type);
    }
}
