using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    // Static, enables us to change the variable across scenes
    public static SpawnSchedule schedule;
    private const int waveCooldown = 5;

    // Variables are set in the inspector
    [SerializeField] private Field grid;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<EnemyData> enemyDatas;
    [SerializeField] private TMP_Text waveCountText;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private SpawnSchedule defaultSchedule;

    private readonly WaitForSeconds waitSecond = new(1);
    private bool shouldSkip;

    // Public get only, set only works within the class
    public bool IsSpawning { get; private set; }
    public List<Enemy> ActiveEnemies { get; private set; }

    // Method is called when the game starts, initialises variables
    public void Awake()
    {
        shouldSkip = false;
        IsSpawning = false;
        ActiveEnemies = new();
    }

    // Starts running a schedule
    public void Begin()
    {
        // For testing purposes, switching scenes to select a schedule took too much time
        // It will default to the default schedule when I run the game from the Game scene
        if (schedule == null)
            schedule = defaultSchedule;

        StartCoroutine(RunSchedule(schedule));
    }

    // Toggles should skip to true
    public void SkipWaveDuration()
    {
        shouldSkip = true;
    }

    // Loop countdown, starting from given duration
    private IEnumerator WaitDuration(int duration)
    {
        for (int currentTime = duration; currentTime >= 0; currentTime--) {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            // @"mm\:ss" allows the string to interpret special characters literally
            //      https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/verbatim
            // timeSpan.ToString(string) allows us to format the time span to how I want it to format
            //      https://learn.microsoft.com/en-us/dotnet/api/system.timespan.tostring?view=net-9.0#system-timespan-tostring(system-string)
            string formatted = timeSpan.ToString(@"mm\:ss");

            durationText.SetText(formatted);

            yield return waitSecond;

            // Stops the countdown if it should skip
            if (shouldSkip == true) {
                shouldSkip = false;

                yield break;
            }
        }
    }

    // Runs the given spawn schedule
    private IEnumerator RunSchedule(SpawnSchedule schedule)
    {
        int waveCount = 0; // Counter variable, only used for display

        foreach (Wave wave in schedule.waves) {
            IsSpawning = true;

            yield return StartCoroutine(WaitDuration(waveCooldown));

            waveCountText.SetText($"{++waveCount}"); // ++waveCount makes it increment first, then used
            StartCoroutine(SpawnWave(wave));

            // waves[^1] the ^1 gets the last index of a list
            // The duration doesn't matter if it's the last wave
            if (wave == schedule.waves[^1]) {
                durationText.SetText("99:99");

                break;
            }

            // Yields until the wave duration finishes, then the next wave starts
            yield return StartCoroutine(WaitDuration(wave.duration));

            grid.EndBase.AddCash(wave.waveReward);
        }

        // WaitUntil(delegate), where thread is resumed when the delegate evaluates to true
        yield return new WaitUntil(() => ActiveEnemies.Count == 0);

        // Ensures that the player didn't fail
        if (grid.EndBase.Health == 0) {
            grid.End(false);

            yield break;
        }

        grid.End(true);
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        // Iterates through each batch within a wave
        foreach (EnemyBatch batch in wave.batches) {
            bool isLast = false;

            // isLast flag is used to determine the IsSpawning flag
            if (batch == wave.batches[^1])
                isLast = true;

            // Waits until a batch finishes spawning, then spawns the next batch
            yield return StartCoroutine(SpawnBatch(batch, isLast));
        }
    }

    private IEnumerator SpawnBatch(EnemyBatch batch, bool isLast)
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
            if (isLast && count + 1 == batch.count)
                IsSpawning = false; // Ensures that this state won't toggle off after last enemy is destroyed

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
            ActiveEnemies.Add(enemy);
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