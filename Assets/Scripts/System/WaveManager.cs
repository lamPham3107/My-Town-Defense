using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using UnityEngine;
public enum WaveState
{
    Pending,
    InProgress,
    Completed
}
public class WaveManager : MonoBehaviour
{
    public static WaveManager instance { get; private set; }
    [SerializeField] private MapData _mapData;
    [SerializeField] private ZombieController[] _zombiePrefabMap;

    private int _currentWaveIndex = 0;
    private int _aliveZombies = 0;

    public WaveState _currentWaveState { get; private set; } = WaveState.Pending;

    public System.Action<int, int> OnWaveStarted;   // (currentWave, totalWaves)
    public System.Action<int> OnWaveCompleted; // (waveIndex)
    public System.Action OnNextWaveAvailable;
    public System.Action OnAllWavesCompleted;

    private Vector3[] _waypoints;
    private readonly Dictionary<string, ZombieController> _prefabLookup = new();


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        foreach (var prefab in _zombiePrefabMap)
        {
            _prefabLookup[prefab.Data.id] = prefab;
        }
    }

    public void Init(MapData mapData , PathHolder path)
    {
        _mapData = mapData;
        _waypoints = path.GetWaypoints();
        ResourceManager.Instance.Init(_mapData.startingGold, _mapData.startingLives);
    }

    public void StartNextWave()
    {
        if(_currentWaveIndex >= _mapData.waveConfigs.Length)
        {
            return;
        }

        StartCoroutine(RunWave(_mapData.waveConfigs[_currentWaveIndex]));
    }

    private IEnumerator RunWave(WaveConfig config)
    {
        _currentWaveState = WaveState.InProgress;
        OnWaveStarted?.Invoke(_currentWaveIndex + 1, _mapData.waveConfigs.Length);
        yield return new WaitForSeconds(config.delayBeforeWave);

        foreach ( var group in config.zombieGroups)
        {
            Debug.Log("Group: " + group.ZombieData.id + " count: " + group.count);
            for (int i = 0; i < group.count; i++)
            {
                Debug.Log("Spawning Zombie: " + group.ZombieData.id);
                SpawnZombie(group.ZombieData);
                _aliveZombies++;
                yield return new WaitForSeconds(config.spawnDelay);
            }
        }
        _currentWaveIndex++;
        if(_currentWaveIndex < _mapData.waveConfigs.Length)
        {
            float waitTime = _mapData.waveConfigs[_currentWaveIndex].delayBeforeWave;
            Debug.Log($"Wave {_currentWaveIndex} completed. Next wave in {waitTime} seconds.");
            StartCoroutine(NotifyNextWaveReady(waitTime));
        }
        else
        {
            // Hết wave → chờ zombie cuối chết rồi kết thúc
            yield return new WaitUntil(() => _aliveZombies <= 0);
            OnAllWavesCompleted?.Invoke();
        }

    }

    private void SpawnZombie(ZombieData data)
    {
        if(!_prefabLookup.TryGetValue(data.id, out var prefab))
        {
            Debug.LogError($"No prefab found for Zombie ID: {data.id}");
            _aliveZombies--;
            return;
        }

        var zombie = PoolManager.instance.GetZombie(prefab, _waypoints[0]);
        zombie.Init(data, _waypoints);
        zombie.OnDeath += OnZombieDied;
        zombie.OnReachEnd += OnZombieReachedEnd;
    }
    private void OnZombieDied(ZombieController zombie)
    {
        zombie.OnDeath -= OnZombieDied;
        zombie.OnReachEnd -= OnZombieReachedEnd;
        ResourceManager.Instance.AddGold(zombie.Data.reward);
        _aliveZombies--;
    }

    private void OnZombieReachedEnd(ZombieController zombie)
    {
        zombie.OnDeath -= OnZombieDied;
        zombie.OnReachEnd -= OnZombieReachedEnd;
        ResourceManager.Instance.LoseLives(zombie.Data.livesOnReach);
        _aliveZombies--;
    }

    private IEnumerator NotifyNextWaveReady(float delay)
    {
        yield return new WaitForSeconds(delay);
        UIManager.Instance.ShowButtonStartWave();
    }
}
