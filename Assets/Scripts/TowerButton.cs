using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private Image image;

    public TowerType type;

    public void Awake()
    {
        TowerData data = towerManager.GetDataFromType(type);
        image.color = data.color;
    }

    public void Select()
    {
        towerManager.Select(type);
    }
}
