using UnityEngine;

// Creates a scriptable object, makes designing tower data intuitive and easy
[CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObjects/TowerData")]
public class TowerData : ScriptableObject
{
    public TowerType type;
    public int cost;
    public int attack;
    public float range;
    public float fireRate;

    public Color color;
}
