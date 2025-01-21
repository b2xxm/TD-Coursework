using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private List<Tile> path;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<EnemyData> enemyDatas;

    public GameObject enemyContainer;

    // <temp> not how schedule should be set
    [SerializeField] private SpawnSchedule schedule;

    private void SetPath(List<Tile> path)
    {
        // <note> check if schedule is running
        // - set if not running, otherwise something else?
        this.path = path;
    }

    public IEnumerator RunSchedule(List<Tile> path)
    {
        SetPath(path);

        // <note> should probably account for already running schedule edgecase
        // - check if a path has been set, set path to null if schedule fully finishes or is quit

        foreach (Wave wave in schedule.waves) {
            StartCoroutine(SpawnWave(wave));

            yield return new WaitForSeconds(wave.duration);
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        foreach (EnemyBatch batch in wave.batches) {
            // Using what I learnt from making the enemy traversal
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

        for (int count = 0; count < batch.count; count++) {
            GameObject newEnemy = Instantiate(enemyPrefab, enemyContainer.transform); // Creates a new game object, using the prefab as a template
            Enemy enemy = newEnemy.GetComponent<Enemy>(); // Gets the Enemy class component of the game object

            enemy.SetData(enemyData);
            StartCoroutine(enemy.Traverse(path)); // Moves the enemy along the pathway

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