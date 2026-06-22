using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private MapData _mapData;
    [SerializeField] private PathHolder[] _pathHolder;

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
        GameAudioSource.Instance.PlayBGM(GameAudioSource.Instance.bgmGamePlay);
        ResourceManager.Instance.OnGameOver += HandleGameOver;
        WaveManager.instance.OnAllWavesCompleted += HandleVictory;
    }
    public void StartGame()
    {

    }
    private void HandleGameOver()
    {
        Time.timeScale = 0f;
        UIManager.Instance.ShowPanelLose();
    }

    private void HandleVictory()
    {
        int stars = CalcStars(ResourceManager.Instance.Lives, _mapData.startingLives);
        if(stars == 3)
        {
            UIManager.Instance.ShowStar3();
        }
        else if (stars == 2)
        {
            UIManager.Instance.ShowStar2();
        }
        else
        {
            UIManager.Instance.ShowStar1();
        }
        UIManager.Instance.ShowPanelWin();
        GameMaster.SaveLevelResult(int.Parse(_mapData.mapId), stars);
        Time.timeScale = 0f;
    }

    private int CalcStars(int currentLives, int startLives)
    {
        float ratio = (float)currentLives / startLives;
        if (ratio >= 0.9f) return 3;
        if (ratio >= 0.5f) return 2;
        return 1;
    }

}
