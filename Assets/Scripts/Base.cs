using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private int health;

    public int Health { get { return health; } }

    public void TakeDamage(int amount)
    {
        health = Mathf.Max(health - amount, 0);

        if (health == 0) {
            // stop schedule
            // stop completely
        }
    }
}
