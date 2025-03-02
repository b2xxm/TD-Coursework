using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Field grid;

    [SerializeField] private BoxCollider2D collider2d;
    [SerializeField] private Slider healthSlider;

    public int Health { get; private set; }
    public int Reward { get; private set; }
    public float Speed { get; private set; }
    public float Travelled { get; private set; }

    public void Initialise(Field grid, EnemyData data)
    {
        this.grid = grid;

        Health = data.health;
        Speed = data.speed;
        Reward = data.reward;
        Travelled = 0;

        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.color = data.color;

        transform.localScale *= data.scale;
        collider2d.size *= data.scale;
        healthSlider.maxValue = data.health;
        healthSlider.value = data.health;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        healthSlider.value = Health;

        if (Health <= 0) {
            grid.EndBase.AddCash(Reward);

            Destroy(gameObject);
        }
    }

    public IEnumerator Traverse(List<Tile> path, Action finishCallback)
    {
        for (int index = 0; index < path.Count; index++) {
            Tile tile = path[index];

            if (index == 0) {
                Vector3 tilePosition = tile.transform.position;
                transform.position = new(tilePosition.x, tilePosition.y, -1);

                continue;
            }

            yield return StartCoroutine(MoveTo(tile));
        }

        finishCallback();
    }

    private IEnumerator MoveTo(Tile tile)
    {
        Vector3 start = transform.position;
        Vector3 end = tile.transform.position;

        float totalTime = 1 / Speed;
        float elapsed = 0;
        float initialTravelled = Travelled;

        while (true) {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, totalTime);

            float alpha = elapsed / totalTime;

            Vector3 position = Vector3.Lerp(start, end, alpha);
            transform.position = new(position.x, position.y, -1);

            Travelled = initialTravelled + alpha;

            if (alpha == 1) {
                break;
            }

            yield return null;
        }
    }
}

public enum EnemyType
{
    Normal,
    Fast,
    Slow
}