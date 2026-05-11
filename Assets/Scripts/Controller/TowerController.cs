using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    //[SerializeField] private TowerData towerData;
    public TowerData _towerData;

    private int _level = 0;

    public float Damage => _towerData.damage * Mathf.Pow(1.2f, _level);
    public float Range => _towerData.range * Mathf.Pow(1.1f, _level);

    public float AttackSpeed => _towerData.attackSpeed * Mathf.Pow(1.05f, _level);

    private float _attackTimer;
    private ZombieController _currentTarget;
    [SerializeField] private Transform characterTransform;
    //[SerializeField] private Transform _shootPoint;


    private void Update()
    {

        _attackTimer += Time.deltaTime;
        // tim target moi neu target cu null hoac da chet
        if (_currentTarget == null || _currentTarget._isDead)
        {
            Debug.Log("Finding new target...");
            _currentTarget = FindTaget();
        }
        if(_currentTarget == null)
        {
            return;
        }

        // Quay tower về phía target (2D)
        Vector3 dir = (_currentTarget.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 135f;
        characterTransform.rotation = Quaternion.Euler(0, 0, angle);

        if (_attackTimer > 1 / AttackSpeed)
        {
            _attackTimer = 0f;
        }


    }

    private ZombieController FindTaget()
    {
        var _zombiesInRange = Physics2D
            .OverlapCircleAll(transform.position, Range, LayerMask.GetMask("Enemy"))
            .Select(c => c.GetComponent<ZombieController>())
            .Where(z => z != null && !z._isDead)
            .ToList();
        Debug.Log($"Found {_zombiesInRange.Count} zombies in range.");
        if (_zombiesInRange.Count == 0) return null;


        return _towerData.targetPriority switch
        {
            TargetPriority.First => _zombiesInRange.OrderByDescending(z => z._currentWaypointIndex).First(),
            TargetPriority.Last => _zombiesInRange.OrderBy(z => z._currentWaypointIndex).First(),
            TargetPriority.Strongest => _zombiesInRange.OrderByDescending(z => z._currentHp).First(),
            TargetPriority.Weakest => _zombiesInRange.OrderBy(z => z._currentHp).First(),
            TargetPriority.Nearest => _zombiesInRange.OrderBy(z => Vector3.Distance(transform.position, z.transform.position)).First(),
            _ => _zombiesInRange[0]
        };
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
