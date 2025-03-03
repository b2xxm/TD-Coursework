using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Field grid;

    // Variables are set within inspector
    [SerializeField] private BoxCollider2D collider2d;
    [SerializeField] private Slider healthSlider;

    // Can only be get publicly, and set privately
    public int Health { get; private set; }
    public int Reward { get; private set; }
    public float Speed { get; private set; }
    public float Travelled { get; private set; }

    // Initialises the enemy with the given enemy data
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

    // Takes a given amount of damage off of the enemy health
    public void TakeDamage(int amount)
    {
        Health -= amount;
        healthSlider.value = Health;

        // Player is rewarded for destroying the enemy
        if (Health <= 0) {
            grid.EndBase.AddCash(Reward);

            Destroy(gameObject);
        }
    }

    // Starts the path traversal of an enemy
    public IEnumerator Traverse(List<Tile> path, Action finishCallback)
    {
        // Iterates through each tile in the pathway
        for (int index = 0; index < path.Count; index++) {
            Tile tile = path[index];

            // Enemy starts at the first tile, so set position and skip to next iteration
            if (index == 0) {
                Vector3 tilePosition = tile.transform.position;
                transform.position = new(tilePosition.x, tilePosition.y, -1);

                continue;
            }

            // Moves the enemy to the next tile
            // Yields until the move to is finished
            yield return StartCoroutine(MoveTo(tile));
        }

        // The given callback is called once the enemy reaches the end of the path
        finishCallback();
    }

    // Moves the enemy from the current position to the given tile
    private IEnumerator MoveTo(Tile tile)
    {
        Vector3 start = transform.position;
        Vector3 end = tile.transform.position;

        float totalTime = 1 / Speed;
        float elapsed = 0;
        float initialTravelled = Travelled;

        // Loops and updates the enemy position each frame, for smooth enemy movement
        while (true) {
            // Ensures the elapsed time doesn't surpass the total time
            elapsed = Mathf.Min(elapsed + Time.deltaTime, totalTime);

            // The alpha value is a fraction, between the start and end position
            float alpha = elapsed / totalTime;

            Vector3 position = Vector3.Lerp(start, end, alpha);
            transform.position = new(position.x, position.y, -1);

            Travelled = initialTravelled + alpha;

            // Means that the enemy is at the end tile
            if (alpha == 1) {
                break;
            }

            // yield return null yields for a frame until resuming
            yield return null;
        }
    }

    // Destroy logic for the enemy, so i won't have repeated code at when I destroy an enemy
    public void OnDestroy()
    {
        Spawner spawner = grid.Spawner;
        spawner.ActiveEnemies.Remove(this);

        // If the spawner is currently waiting until the next wave starts,
        // and all enemies are destroyed, then the next wave will start automatically
        if (spawner.ActiveEnemies.Count == 0 && spawner.IsSpawning == false) {
            spawner.SkipWaveDuration();
        }
    }
}

// Enemy type enum
public enum EnemyType
{
    Normal,
    Fast,
    Slow
}