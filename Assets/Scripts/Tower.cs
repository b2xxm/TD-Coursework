using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private List<Enemy> targets;
    private Coroutine attackCoroutine;
    private WaitForSeconds waitInterval;
    private Enemy target;

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
        /* <note> possibly optimize by predicting first enemy
         * if next entered enemy has greater move speed, cache enemy and calculate time taken until surpass
         *      possibly a dictionary first enemy for each type
         * recalculate every exit
         */

        Enemy firstEnemy = null;
        float highestTravelled = 0;

        foreach (Enemy enemy in targets) {
            if (enemy.Travelled > highestTravelled) {
                firstEnemy = enemy;
                highestTravelled = enemy.Travelled;
            }
        }

        return firstEnemy;
    }

    public IEnumerator AttackLoop()
    {
        while (true) {
            target = FindFirstEnemy();

            if (target == null) {
                attackCoroutine = null;

                yield break;
            }

            target.TakeDamage(Attack);

            yield return waitInterval;
        }
    }

    public void Update()
    {
        if (target != null) {
            Vector3 direction = target.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Subtracts 90 as 0 is north, while the calculated angle assumes 0 is east
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
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