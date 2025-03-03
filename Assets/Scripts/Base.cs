using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Base : MonoBehaviour
{
    [SerializeField] private Field grid;
    [SerializeField] private int maxHealth;
    [SerializeField] private int startingCash;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private TMP_Text cashDisplay;

    private int health;
    private int cash;

    public int Health {
        get => health;
        private set {
            if (health == value)
                return;

            health = value;

            slider.value = health;
            healthDisplay.SetText($"{health} / {maxHealth}");
        }
    }

    public int Cash {
        get => cash;
        private set
        {
            if (cash == value)
                return;

            cash = value;

            cashDisplay.SetText($"${cash}");
        }
    }

    public void Awake()
    {
        ResetBase();

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        Health = Mathf.Max(Health - amount, 0);

        if (Health == 0) {
            grid.End(false);
        }
    }

    public void AddCash(int amount)
    {
        Cash += amount;
    }

    public void ResetBase()
    {
        Health = maxHealth;
        Cash = startingCash;
    }
}
