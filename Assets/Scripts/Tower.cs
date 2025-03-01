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

    public TowerType Type { get; private set; }
    public int Attack { get; private set; }
    public float Range { get; private set; }
    public float FireRate { get; private set; }

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

    public Enemy FindFirstEnemy()
    {
        Enemy firstEnemy = null;
        float highestTravelled = 0;

        foreach (Enemy enemy in targets) {
            if (enemy.Travelled > highestTravelled)
                firstEnemy = enemy;
        }

        return firstEnemy;
    }

    public IEnumerator AttackLoop()
    {
        while (true) {
            Enemy enemy = FindFirstEnemy();

            if (enemy == null) {
                attackCoroutine = null;

                yield break;
            }

            enemy.TakeDamage(Attack);

            yield return waitInterval;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) {
            Enemy enemy = collision.GetComponent<Enemy>();
            targets.Add(enemy);

            if (attackCoroutine != null)
                return;

            attackCoroutine = StartCoroutine(AttackLoop());
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) {
            Enemy enemy = collision.GetComponent<Enemy>();
            targets.Remove(enemy);
        }
    }
}

public enum TowerType
{
    Basic,
    Ranged,
    Quick
}