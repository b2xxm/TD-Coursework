using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerType type;
}

public enum TowerType
{
    Basic,
    Sniper,
    Cannon
}