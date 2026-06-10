using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StatusEffectData", menuName = "Game/StatusEffectData", order = 1)]
public class StatusEffectData : ScriptableObject
{
    public EffectType effectType;
    public float value; // muc do hieu ung
    public float duration; // thoi gian hieu ung
    public float tickInterval; // chu ky gay damage (dung cho effect co tick)
}

public enum EffectType
{
    Slow,
    Bleed,
    None
}
