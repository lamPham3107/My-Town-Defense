using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Game/WaveConfig", order = 1)]
public class WaveConfig : ScriptableObject
{
    public int waveIndex;
    public ZombieGroups[] zombieGroups;
    public float spawnDelay;
    public float delayBeforeWave;
    public int waveReward;
}
