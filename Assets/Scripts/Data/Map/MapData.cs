using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Map_New",menuName = "Game/MapData")]
public class MapData : MonoBehaviour
{
    public string mapId;
    public string mapName;
    public string sceneName;
    public int startingGold;
    public int startingLives;
    public WaveConfig[] waveConfigs;
}
