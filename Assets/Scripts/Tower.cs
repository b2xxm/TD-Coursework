using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private List<Enemy> targets;
    private Coroutine attackCoroutine;
    private WaitForSeconds waitInterval;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CircleCollider2D collider2d;

    // Makes the variable get only publicly, and can only be set within the class
    public TowerType Type { get; private set; }
    public int Attack { get; private set; }
    public float Range { get; private set; }
    public float FireRate { get; private set; }

    // Initialises tower with given data
    public void Initialise(TowerData data)
    {
        Type = data.type;
        Range = data.range;
        Attack = data.attack;
        FireRate = data.fireRate;

        collider2d.radius = Range;
        spriteRenderer.color = data.color;

        targets = new();
        waitInterval = new(1 / FireRate);
    }

    // Finds the first (not first spawned) enemy
    public Enemy FindFirstEnemy()
    {
        Enemy firstEnemy = null;
        float highestTravelled = 0;

        // Check each enemy in range
        foreach (Enemy enemy in targets) {
            if (enemy.Travelled > highestTravelled) {
                firstEnemy = enemy;
                highestTravelled = enemy.Travelled;
            }
        }

        return firstEnemy;
    }

    // Attack enemies inside range at a specified rate
    public IEnumerator AttackLoop()
    {
        while (true) {
            Enemy enemy = FindFirstEnemy();

            // If no enemy is found, it stops the attack loop
            if (enemy == null) {
                attackCoroutine = null;

                yield break;
            }

            enemy.TakeDamage(Attack);

            // Yields until the specific wait interval for this tower is over
            yield return waitInterval;
        }
    }

    // Update is called every frame
    public void Update()
    {
        Enemy enemy = FindFirstEnemy();

        if (enemy != null) {
            Vector3 direction = enemy.transform.position - transform.position;
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Mathf.Atan2.html
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Subtracts 90 as 0 is north, while the calculated angle assumes 0 is east
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    // Is triggered when a collider (with rigid body component) collides with this collider
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks that the collision is from an enemy
        if (collision.CompareTag("Enemy")) {
            Enemy enemy = collision.GetComponent<Enemy>();
            targets.Add(enemy);

            // Starts attack loop, if it's not attacking
            if (attackCoroutine != null)
                return;

            attackCoroutine = StartCoroutine(AttackLoop());
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) {
            // Removes enemy from target if they exit the range
            Enemy enemy = collision.GetComponent<Enemy>();
            targets.Remove(enemy);
        }
    }
}

// Tower type enum
public enum TowerType
{
    Basic,
    Ranged,
    Quick
}