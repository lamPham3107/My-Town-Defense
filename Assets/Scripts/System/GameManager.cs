using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private MapData _mapData;
    [SerializeField] private PathHolder _pathHolder;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        WaveManager.instance.Init(_mapData, _pathHolder);
        ResourceManager.Instance.OnGameOver += HandleGameOver;
        WaveManager.instance.OnAllWavesCompleted += HandleVictory;
    }
    public void StartGame()
    {

    }
    private void HandleGameOver()
    {
        Time.timeScale = 0f;
    }

    private void HandleVictory()
    {
        int stars = CalcStars(ResourceManager.Instance.Lives, _mapData.startingLives);
        SaveProgress(stars);
        Time.timeScale = 0f;
    }

    private int CalcStars(int currentLives, int startLives)
    {
        float ratio = (float)currentLives / startLives;
        if (ratio >= 0.9f) return 3;
        if (ratio >= 0.5f) return 2;
        return 1;
    }

    private void SaveProgress(int stars)
    {
        string mapId = _mapData.mapId;
        int prev = PlayerPrefs.GetInt($"stars_{mapId}", 0);
        if (stars > prev)
            PlayerPrefs.SetInt($"stars_{mapId}", stars);
        PlayerPrefs.Save();
    }
}
