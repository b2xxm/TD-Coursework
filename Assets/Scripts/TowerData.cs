using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObjects/TowerData")]
public class TowerData : ScriptableObject
{
    public TowerType type;
    public int cost;

    public Color color;
    // <note> add other properties
}
