using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Base : MonoBehaviour
{
    // Variables are set in the inspector
    [SerializeField] private Field grid;
    [SerializeField] private int maxHealth;
    [SerializeField] private int startingCash;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private TMP_Text cashDisplay;

    private int health;
    private int cash;

    // Custom private setters (can only be set within the class)
    // Getter acts as a proxy, returning the variables that are privately managed

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

    // Function is called when the game starts
    public void Awake()
    {
        ResetBase();

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    // Takes given amount of damage off of the base
    public void TakeDamage(int amount)
    {
        Health = Mathf.Max(Health - amount, 0);

        // When health drops to 0, end the game on a loss
        if (Health == 0) {
            grid.End(false);
        }
    }

    // Setter method to edit cash variable
    public void AddCash(int amount)
    {
        Cash += amount;
    }

    // Sets the base back to default value
    public void ResetBase()
    {
        Health = maxHealth;
        Cash = startingCash;
    }
}
