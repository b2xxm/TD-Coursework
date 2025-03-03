using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnSchedule", menuName = "ScriptableObjects/SpawnSchedule")]
public class SpawnSchedule : ScriptableObject
{
    public List<Wave> waves;
}

[Serializable]
public class Wave
{
    public List<EnemyBatch> batches;
    public int duration; // Wave duration
    public int waveReward; // Cash after wave finish
}

[Serializable]
public class EnemyBatch
{
    public EnemyType type; // The type of enemy that this batch will be
    public int count; // Amount of enemies to spawn
    public int offsetNext; // Amount of time until the next batch can start
    public float rate; // The rate at which enemies spawn
}

// Using classes as they're serializable (editable within inspector), where dictionaries aren't