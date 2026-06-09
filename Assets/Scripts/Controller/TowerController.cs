using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    //[SerializeField] private TowerData towerData;
    public TowerData _towerData;
    [SerializeField] private Animator _animator;
    private bool _isShooting = false;

    private int _level = 0;

    public float Damage => _towerData.damage * Mathf.Pow(1.2f, _level);
    public float Range => _towerData.range * Mathf.Pow(1.1f, _level);

    public float AttackSpeed => _towerData.attackSpeed * Mathf.Pow(1.05f, _level);

    private float _attackTimer;
    private ZombieController _currentTarget;
    [SerializeField] private Transform _characterPoint;
    [SerializeField] private Transform _projectPoint;

    //[SerializeField] private Transform _characterPoint;


    private void Update()
    {

        _attackTimer += Time.deltaTime;
        // tim target moi neu target cu null hoac da chet hoac ra ngoai vung
        if (_currentTarget == null || _currentTarget._isDead  
            || Vector3.Distance(transform.position,_currentTarget.transform.position) > Range)
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
        float projectileAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        if(_characterPoint != null)
        {
            _characterPoint.rotation = Quaternion.Euler(0, 0, angle);
        }
        _projectPoint.rotation = Quaternion.Euler(0, 0, projectileAngle);

        if (_attackTimer > 1 / AttackSpeed)
        {
            _attackTimer = 0f;
            StartCoroutine(ShootAnimation());
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

    private IEnumerator ShootAnimation()
    {
        _isShooting = true;
        _animator.SetTrigger("Shoot");
        yield return null;
        float aniLenght = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(aniLenght * 0.5f); // Đợi đến giữa animation để bắn
        Shoot();
        yield return new WaitForSeconds(aniLenght * 0.5f); // Đợi nốt phần còn lại của animation
        _isShooting = false;
    }

    private void Shoot()
    {
        if (_towerData.projectilePrefab == null || _currentTarget == null) return;

        // Lấy component IProjectile từ prefab (bất kể loại nào)
        var prefabComp = _towerData.projectilePrefab.GetComponent<IProjectile>();
        if (prefabComp == null)
        {
            Debug.LogError("Prefab không có IProjectile component!");
            return;
        }

        var mb = PoolManager.instance.GetProjectTile(
            prefabComp as MonoBehaviour,
            _projectPoint.position,
            _projectPoint.rotation);

        // Cast về IProjectile để gọi Init
        (mb as IProjectile)?.Init(this, _currentTarget);
    }   

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
    public bool CanUpgrade() => _towerData.nextLevel != null;

    public int GetUpgradeCost()
    {
        if (!CanUpgrade()) return 0;
        int cost = _towerData.upgradeCost;
        return cost;
    }
    public bool Upgrade()
    {
        if (!CanUpgrade()) return false;
        int cost = GetUpgradeCost();

        if (!ResourceManager.Instance.SpendGold(cost))
        {
            Debug.Log("Not enough gold to upgrade!");
            return false;
        }
        Transform parent = transform.parent;
        Vector3 currentPos = transform.position;
        TowerData nextTowerData = _towerData.nextLevel;

        Destroy(gameObject);


        var newTowerObj = Instantiate(nextTowerData.towerPrefab, currentPos, Quaternion.identity, parent);
        TowerPanelMenu.Instance.Hide();
        return true;
    }

    public int GetSellValue()
    {
        return _towerData.sellValue;
    }

    public void Sell()
    {
        ResourceManager.Instance.AddGold(GetSellValue());
        TowerPanelMenu.Instance.Hide();
        Destroy(gameObject);
    }


}
