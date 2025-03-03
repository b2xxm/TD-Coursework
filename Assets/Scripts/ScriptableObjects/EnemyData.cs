using UnityEngine;

// Creates a scriptable object, makes designing enemy data intuitive and easy
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyType type;
    public int health;
    public float speed;
    public int reward; // Cash amount rewarded when destroyed

    public Color color;
    public float scale;
}