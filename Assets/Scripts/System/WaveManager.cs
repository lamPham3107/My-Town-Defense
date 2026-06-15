using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using TMPro;
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
    public GameObject _wave_Bar;
    public TextMeshProUGUI Txt_wave;

    public System.Action<int, int> OnWaveStarted;   // (currentWave, totalWaves)
    public System.Action<int> OnWaveCompleted; // (waveIndex)
    public System.Action OnNextWaveAvailable;
    public System.Action OnAllWavesCompleted;
    private Coroutine _autoStartCoroutine;

    private Vector3[][] _allWayPoints;
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

    public void Init(MapData mapData , PathHolder[] paths)
    {
        _mapData = mapData;
        _allWayPoints = new Vector3[paths.Length][];
        for (int i = 0; i < paths.Length; i++)
        {
            _allWayPoints[i] = paths[i].GetWaypoints();
        }

        ResourceManager.Instance.Init(_mapData.startingGold, _mapData.startingLives);

        UpdateWaveText();
        UIManager.Instance.ShowButtonStartWave();
    }

    public void StartNextWave()
    {
        Debug.Log("Current Wave Index: " + _currentWaveIndex);
        if (_currentWaveIndex >= _mapData.waveConfigs.Length)
        {
            return;
        }
        // Hủy auto-start nếu player tự bấm
        if (_autoStartCoroutine != null)
        {
            StopCoroutine(_autoStartCoroutine);
            _autoStartCoroutine = null;
        }

        StartCoroutine(RunWave(_mapData.waveConfigs[_currentWaveIndex]));
    }

    private IEnumerator RunWave(WaveConfig config)
    {
        _currentWaveState = WaveState.InProgress;
        UIManager.Instance.HideButtonStartWave();
        UpdateWaveText();
        ShowWaveBar();  
        OnWaveStarted?.Invoke(_currentWaveIndex + 1, _mapData.waveConfigs.Length);

        foreach ( var group in config.zombieGroups)
        {
            Debug.Log("Group: " + group.ZombieData.id + " count: " + group.count);
            for (int i = 0; i < group.count; i++)
            {
                Debug.Log("Spawning Zombie: " + group.ZombieData.id);
                SpawnZombie(group.ZombieData, group.pathIndex);
                _aliveZombies++;
                yield return new WaitForSeconds(config.spawnDelay);
            }
        }
        _currentWaveIndex++;
        //_currentWaveState = WaveState.Completed;
        //OnWaveCompleted?.Invoke(_currentWaveIndex - 1);
        if (_currentWaveIndex < _mapData.waveConfigs.Length)
        {
            float waitTime = _mapData.waveConfigs[_currentWaveIndex].delayBeforeWave;
            Debug.Log($"Wave {_currentWaveIndex} completed. Next wave in {waitTime} seconds.");
            StartCoroutine(NotifyNextWaveReady(waitTime));
        }
        else
        {
            // Hết wave → chờ zombie cuối chết rồi kết thúc
            yield return new WaitUntil(() => _aliveZombies <= 0);
            yield return new WaitForSeconds(1f);
            OnAllWavesCompleted?.Invoke();
        }

    }

    private void SpawnZombie(ZombieData data, int pathIndex)
    {
        if(!_prefabLookup.TryGetValue(data.id, out var prefab))
        {
            Debug.LogError($"No prefab found for Zombie ID: {data.id}");
            _aliveZombies--;
            return;
        }
        int idx = Mathf.Clamp(pathIndex, 0, _allWayPoints.Length - 1);
        Vector3[] waypoints = _allWayPoints[idx];

        var zombie = PoolManager.instance.GetZombie(prefab, waypoints[0]);
        zombie.Init(data, waypoints);
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

        // Auto start sau 15s (chỉ từ wave 2 trở đi)
        _autoStartCoroutine = StartCoroutine(AutoStartAfterDelay(15f));
    }
    private IEnumerator AutoStartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }

    private void UpdateWaveText()
    {
            Txt_wave.text = $"{_currentWaveIndex + 1} / {_mapData.waveConfigs.Length}";
    }
    private void HideWaveBar()
    {
        _wave_Bar.SetActive(false);
    }
    private void ShowWaveBar()
    {
        _wave_Bar.SetActive(true);
    }
}
