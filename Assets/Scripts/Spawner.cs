using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    // Static, enables us to change the variable across scenes
    public static SpawnSchedule schedule;

    private readonly WaitForSeconds waitSecond = new(1);

    [SerializeField] private Field grid;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<EnemyData> enemyDatas;
    [SerializeField] private TMP_Text waveCountText;
    [SerializeField] private TMP_Text durationText;

    public void Begin()
    {
        StartCoroutine(RunSchedule(schedule));
    }

    private IEnumerator WaitDuration(int duration)
    {
        for (int currentTime = duration; currentTime >= 0; currentTime--) {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            string formatted = timeSpan.ToString(@"mm\:ss"); // @"mm\:ss" allows the string to interpret special characters literally

            durationText.SetText(formatted);

            yield return waitSecond;
        }
    }

    private IEnumerator RunSchedule(SpawnSchedule schedule)
    {
        int waveCount = 0;

        foreach (Wave wave in schedule.waves) {
            waveCountText.SetText($"{++waveCount}");

            StartCoroutine(SpawnWave(wave));

            yield return StartCoroutine(WaitDuration(wave.duration));
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
                endBase.TakeDamage(enemy.Health);

                Destroy(newEnemy);
            };

            enemy.Initialise(grid, enemyData);
            StartCoroutine(enemy.Traverse(grid.PathObject.Pathway, callback)); // Moves the enemy along the pathway

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