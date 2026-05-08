using System.Collections;
using System.Collections.Generic;
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
    }
}
