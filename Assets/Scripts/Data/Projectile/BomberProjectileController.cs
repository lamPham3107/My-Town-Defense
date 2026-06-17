using UnityEngine;

public class BomberProjectileController : MonoBehaviour, IProjectile
{
    private float _arcHeight = 2.5f;
    private float _speed = 1f;

    private TowerController _owner;
    private Vector3 _startPos;
    private Vector3 _targetPos;  // snapshot cố định khi Init
    private float _progress;
    private bool _initialized;

    public void Init(TowerController owner, ZombieController target)
    {
        _owner = owner;
        _startPos = transform.position;
        _targetPos = target.transform.position; // chụp 1 lần, không đổi
        _progress = 0f;
        _initialized = true;
    }

    // Reset khi trả về pool
    void OnDisable()
    {
        _initialized = false;
        _progress = 0f;
    }

    void Update()
    {
        if (!_initialized) return;

        _progress += Time.deltaTime * _speed;
        _progress = Mathf.Clamp01(_progress);

        // Vị trí trên parabol
        Vector3 flatPos = Vector3.Lerp(_startPos, _targetPos, _progress);
        flatPos.y += Mathf.Sin(_progress * Mathf.PI) * _arcHeight;
        transform.position = flatPos;

        // Xoay theo hướng bay
        float nextProgress = Mathf.Clamp01(_progress + 0.01f);
        Vector3 nextPos = Vector3.Lerp(_startPos, _targetPos, nextProgress);
        nextPos.y += Mathf.Sin(nextProgress * Mathf.PI) * _arcHeight;

        Vector3 dir = (nextPos - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // Đến nơi thì nổ
        if (_progress >= 1f)
            OnHit();
    }

    private void OnHit()
    {
        var data = _owner._towerData;

        var hits = Physics2D.OverlapCircleAll(
            transform.position,
            data.splashRadius > 0 ? data.splashRadius : 0.5f,
            LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            var zombie = hit.GetComponent<ZombieController>();
            if (zombie != null && !zombie._isDead)
                zombie.TakeDamage(data.damage, data.damageType);
        }

        PoolManager.instance.ReturnProjectTile(this);
    }
}