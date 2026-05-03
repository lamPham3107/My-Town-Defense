using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Zombie Data", menuName = "Game/ZombieData")]
public class ZombieData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string displayName;
    public float maxHp;
    public float speed;
    public float armor;
    public float magicResist;
    public int reward;
    public int livesOnReach;

    [Header("Visual")]
    public Sprite sprite;

    [Header("Effect")]
    public bool isSlow;
    public bool isBleed;
}
