using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Game/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string displayName;

    [Header("Cost")]
    public int buildCost;
    public int[] upgradeCost;

    [Header("Combat")]
    public float damage;
    public float attackSpeed;
    public float range;
    public DamageType damageType;

    [Header("Special")]
    public float splashRadius; // 0 = single target
    public EffectType statusEffect;

    [Header("Targeting")]
    public TargetPriority targetPriority;

    [Header("Visual")]
    public GameObject projectilePrefab;
    public Sprite towerSprite;
}

public enum TargetPriority
{
    First,
    Last,
    Strongest,
    Weakest,
    Nearest,
}

public enum DamageType
{
    Physical,
    Magical

}