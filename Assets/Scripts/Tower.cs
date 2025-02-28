using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerType type;
    public SpriteRenderer spriteRenderer;
}

public enum TowerType
{
    Basic,
    Sniper,
    Cannon
}