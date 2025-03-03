using UnityEngine;
using UnityEngine.UI;

// Button class for each tower button (purchase menu)
public class TowerButton : MonoBehaviour
{
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private Image image;

    public TowerType type;

    // Initialises the image color on the button
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
