using System;
using System.Collections.Generic;
using UnityEngine;

// Creates a scriptable object, makes designing a schedule intuitive and easy
// Using classes as they're serializable (editable within inspector), where dictionaries aren't
[CreateAssetMenu(fileName = "SpawnSchedule", menuName = "ScriptableObjects/SpawnSchedule")]
public class SpawnSchedule : ScriptableObject
{
    public List<Wave> waves;
}

[Serializable]
public class Wave
{
    public List<EnemyBatch> batches;
    public int duration;
    public int waveReward;
}

[Serializable]
public class EnemyBatch
{
    public EnemyType type;
    public int count; // Amount of enemies to spawn
    public int offsetNext; // Amount of time until the next batch can start
    public float rate;
}