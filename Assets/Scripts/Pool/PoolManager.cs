using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance { get; private set; }
    [SerializeField] private ZombieController[] _zombiePrefab;
    [SerializeField] private int _zombiePoolSize = 10;
    private Dictionary<string, ObjectPool<ZombieController>> _zombiePools = new();
    private Dictionary<string , ObjectPool<MonoBehaviour>> _projectTilePools = new();
    private Transform zombieParent;
    private Transform projectTileParent;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        zombieParent = new GameObject("ZombiePool").transform;
        for(int i = 0; i < _zombiePrefab.Length; i++)
        {
            RegisterZombie(_zombiePrefab[i]);
        }
        projectTileParent = new GameObject("ProjectTilePool").transform;

    }

    private void RegisterZombie(ZombieController prefab)
    {
        string key = prefab.Data.id;
        if (!_zombiePools.ContainsKey(key))
        {
            _zombiePools[key] = new ObjectPool<ZombieController>(prefab, _zombiePoolSize, zombieParent);
        }
    }

    public ZombieController GetZombie(ZombieController prefab , Vector3 pos)
    {
        string key = prefab.Data.id;

        if(!_zombiePools.ContainsKey(key))
        {
            RegisterZombie(prefab);
        }
        return _zombiePools[key].Get(pos, Quaternion.identity);
    }

    public void ReturnZombie(ZombieController zombie)
    {
        _zombiePools[zombie.Data.id].ReturnToPool(zombie);
    }


    public void RegisterProjectTile(MonoBehaviour prefab , int size = 20)
    {
        string key = prefab.name.Replace("(Clone)", "").Trim();
        if (!_projectTilePools.ContainsKey(key))
        {
            _projectTilePools[key] = new ObjectPool<MonoBehaviour>(prefab, size, projectTileParent);
        }
    }

    public MonoBehaviour GetProjectTile(MonoBehaviour prefab, Vector3 pos, Quaternion rot)
    {
        string key = prefab.name.Replace("(Clone)", "").Trim();
        if (!_projectTilePools.ContainsKey(key))
        {
            RegisterProjectTile(prefab);
        }
        return _projectTilePools[key].Get(pos, rot);
    }

    public void ReturnProjectTile(MonoBehaviour projectTile)
    {
        string key = projectTile.name.Replace("(Clone)", "").Trim();
        _projectTilePools[key].ReturnToPool(projectTile);
    }

}
