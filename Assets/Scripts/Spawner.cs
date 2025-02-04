using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Static, enables us to change the variable across scenes
    public static SpawnSchedule schedule;

    private List<Tile> pathway;
    private bool active;

    [SerializeField] private Field grid;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Awake()
    {
        active = false;
    }

    public void Begin()
    {
        // More validation, to ensure code doesn't fail
        if (active)
            return;

        Path path = grid.Path;
        bool success = SetPath(path.Pathway);

        if (!success)
            return;

        active = true;

        StartCoroutine(RunSchedule(schedule));
    }

    private bool SetPath(List<Tile> pathway)
    {
        // Added edgecases in case this scenario causes failure
        if (pathway.Count == 0)
            return false;

        this.pathway = pathway;

        return true;
    }

    private IEnumerator RunSchedule(SpawnSchedule schedule)
    {
        foreach (Wave wave in schedule.waves) {
            StartCoroutine(SpawnWave(wave));

            yield return new WaitForSeconds(wave.duration);
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        foreach (EnemyBatch batch in wave.batches) {
            // Waits until a batch finishes spawning, then spawns the next batch
            yield return StartCoroutine(SpawnBatch(batch));
        }
    }

    private IEnumerator SpawnBatch(EnemyBatch batch)
    {
        EnemyType type = batch.type;
        EnemyData enemyData = GetDataFromType(type);

        // Rate is defined as enemies per second
        // Inverting this into (1 / R) gives seconds per enemy
        float interval = 1 / batch.rate;

        // Create object outside of loop, as I don't need to create it multiple times
        WaitForSeconds waitInterval = new(interval);
        Transform enemyContainer = grid.transform.Find("EnemyContainer");

        for (int count = 0; count < batch.count; count++) {
            GameObject newEnemy = Instantiate(enemyPrefab, enemyContainer); // Creates a new game object, using the prefab as a template
            Enemy enemy = newEnemy.GetComponent<Enemy>(); // Gets the Enemy class component of the game object

            // Creating a callback, so that when the enemy reaches the end, the base will be damaged
            void callback()
            {
                Base endBase = grid.EndBase;
                endBase.TakeDamage(enemy.health);

                Destroy(newEnemy);
            };

            enemy.SetData(enemyData);
            StartCoroutine(enemy.Traverse(pathway, callback)); // Moves the enemy along the pathway

            // Suspends execution until given interval has passed
            yield return waitInterval;
        }

        // Suspends execution until the batch offset has passed
        yield return new WaitForSeconds(batch.offsetNext);
    }

    // Gets the data correlating to the given type
    private EnemyData GetDataFromType(EnemyType type)
    {
        return enemyDatas.Find(data => data.type == type);
    }
}