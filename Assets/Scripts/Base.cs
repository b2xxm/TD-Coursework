using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Base : MonoBehaviour
{
    private int health;

    [SerializeField] private int maxHealth;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text display;

    public int Health {
        get => health;
        private set {
            if (health == value)
                return;

            health = value;

            slider.value = (float) health / maxHealth;
            display.SetText($"{health} / {maxHealth}");
        }
    }

    public void Awake()
    {
        ResetBase();
    }

    public void TakeDamage(int amount)
    {
        Health = Mathf.Max(Health - amount, 0);

        if (Health == 0) {
            // stop schedule
            // stop completely
        }
    }

    public void ResetBase()
    {
        Health = maxHealth;
    }
}
